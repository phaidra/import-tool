using APS.Lib;
using APS.Lib.Helper;
using APS.Lib.Import;
using Avalonia.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using APS.UI.Helpers;

namespace APS.UI
{
    public class MainWindowViewModel : VMBase
    {
        private Window _parentWindow;
        private string _path;
        private string _rootDir;
        private bool _mappingsConfigured;
        private int _progressMaximum;
        private int _progressValue;
        private string _progressText;

        private LoginViewModel _loginViewModel;
        private MappingsViewModel _mappingsViewModel;
        private bool _isBusy;
        private volatile bool _cancellationPending;

        public string CsvPath
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(nameof(CsvPath));
                ConfigureMappingsCommand.RaiseCanExecuteChanged();
            }
        }

        private EncodingInfo _selectedEncodingInfo;

        public EncodingInfo SelectedEncodingInfo
        {
            get
            {
                return _selectedEncodingInfo;
            }
            set
            {
                _selectedEncodingInfo = value;
                RaisePropertyChanged(nameof(SelectedEncodingInfo));
            }
        }

        public Encoding SelectedEncoding
        {
            get
            {
                return _selectedEncodingInfo?.GetEncoding();
            }
        }

        private List<EncodingInfo> _encodings;

        public List<EncodingInfo> EncodingInfos
        {
            get
            {
                return _encodings;
            }
            set
            {
                _encodings = value;
                RaisePropertyChanged(nameof(EncodingInfos));
            }
        }

        public async Task Login()
        {
            _loginViewModel = new LoginViewModel();
            LoginWindow loginWindow = new LoginWindow(_loginViewModel);

            _loginViewModel.CloseAction = () => { loginWindow.Close(); };

            await loginWindow.ShowDialog(_parentWindow);
            if (!_loginViewModel.DialogResult)
            {
                CloseAction?.Invoke();
            }
        }

        public string RootDir
        {
            get { return _rootDir; }
            set
            {
                _rootDir = value;
                RaisePropertyChanged(nameof(RootDir));
            }
        }

        public bool MappingsConfigured
        {
            get { return _mappingsConfigured; }
            set
            {
                _mappingsConfigured = value;
                RaisePropertyChanged(nameof(MappingsConfigured));
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public int ProgressMaximum
        {
            get { return _progressMaximum; }
            set
            {
                _progressMaximum = value;
                RaisePropertyChanged(nameof(ProgressMaximum));
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
                RaisePropertyChanged(nameof(IsEnabled));
                RaisePropertyChanged(nameof(StartCommandText));
            }
        }

        public bool IsEnabled => !IsBusy;

        public string StartCommandText => IsEnabled ? "Start" : "Stop";

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }

        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                _progressText = value;
                RaisePropertyChanged(nameof(ProgressText));
            }
        }

        public Command StartCommand { get; set; }
        public Command ConfigureMappingsCommand { get; set; }
        public Action CloseAction { get; internal set; }
        public FileInfo CsvFI { get; private set; }

        public MainWindowViewModel(Window window)
        {
            _parentWindow = window;
            ConfigureMappingsCommand = new Command(ConfigureMappingsExecute, ConfigureMappingsCanExecute);
            StartCommand = new Command(StartExecute, StartCanExecute);
            ProgressValue = 0;
            ProgressMaximum = 1;
            ProgressText = "Not started";
#if DEBUG
            //CsvPath = @"C:\GutlebenSystems\PHAIDRA-Schnittstelle\APS\APS.Lib\TestFiles\import-01.csv";
            CsvPath = @"N:\Kunden\Kunst Universität Graz\Testdateien\2020-01-14\test_paths_replaced.csv";
            RootDir = @"N:\Kunden\Kunst Universität Graz\Testdateien\2020-01-14\Files";
#endif

            EncodingInfos = Encoding.GetEncodings().ToList();
            SelectedEncodingInfo = EncodingInfos.FirstOrDefault(encoding => encoding.Name == "utf-8");
        }

        private async void ConfigureMappingsExecute(object obj)
        {
            CsvPath = (CsvPath ?? "").Replace("\"", "");
            RootDir = (RootDir ?? "").Replace("\"", "");
            if (System.IO.File.Exists(CsvPath))
            {
                Dictionary<int, string> headers = null;
                List<Dictionary<int, string>> rows = null;
                Encoding.GetEncodings();
                try
                {
                    using (var csvReader = new CsvReader(CsvPath, ";", SelectedEncoding))
                    {
                        headers = await csvReader.ReadHeaders();
                        rows = await csvReader.ReadRows();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogE($"Error in ConfigureMappingsExecute: {ex.ToString()}");
                    ProgressText = $"Error: {ex.Message}";
                    return;
                }

                if (_mappingsViewModel != null)
                {
                    if (_mappingsViewModel.SelectedCsvPath != CsvPath || _mappingsViewModel.SelectedEncoding != SelectedEncodingInfo)
                    {
                        _mappingsViewModel = null;
                    }
                }

                if (_mappingsViewModel == null)
                {
                    if (rows.Count == 0)
                    {
                        ProgressText = $"CSV file could not be read";
                        return;
                    }
                    _mappingsViewModel = new MappingsViewModel(headers, rows, RootDir);
                    _mappingsViewModel.SelectedCsvPath = CsvPath;
                    _mappingsViewModel.SelectedEncoding = SelectedEncodingInfo;
                }
                MappingsWindow2 mappingsWindow = new MappingsWindow2(_mappingsViewModel);
                _mappingsViewModel.SetParentWindow(mappingsWindow);
                _mappingsViewModel.CloseAction = () => { mappingsWindow.Close(); };
                await mappingsWindow.ShowDialog(_parentWindow);
                MappingsConfigured = true;
            }
            else
            {
                ProgressText = $"CSV file could not be found";
            }
        }

        private bool ConfigureMappingsCanExecute(object arg)
        {
            return IsEnabled && !string.IsNullOrEmpty(CsvPath);
        }

        private async void StartExecute(object obj)
        {
            if (IsBusy)
            {
                _cancellationPending = true;
                return;
            }
            try
            {
                IsBusy = true;
                await ProcessImport();
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in StartExecute: {ex.ToString()}");
            }
            finally
            {
                IsBusy = false;
                _cancellationPending = false;
            }
        }

        private async Task ProcessImport()
        {
            try
            {
                List<Dictionary<int, string>> rows = null;
                Dictionary<int, string> headers = null;
                CsvFI = new System.IO.FileInfo(CsvPath);
                try
                {
                    using (var csvReader = new CsvReader(CsvPath, ";", SelectedEncoding))
                    {
                        ProgressText = "Reading CSV file";
                        headers = await csvReader.ReadHeaders();
                        rows = await csvReader.ReadRows();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogE($"Error in ProcessImport: {ex.ToString()}");
                    ProgressText = $"Error: {ex.Message}";
                }

                PhaidraClient client = new PhaidraClient(_loginViewModel.Url, _loginViewModel.SearchEngineUrl, _loginViewModel.Username, _loginViewModel.Password);

                ProgressValue = 0;
                ProgressMaximum = rows.Count;
                bool mappingsValid;
                var mappings = _mappingsViewModel.GetMappings(out mappingsValid);
                
                PhaidraMetadataGenerator metadataGenerator = new PhaidraMetadataGenerator(mappings);
                for (int i = 0; i < rows.Count; i++)
                {
                    PhaidraFile file = new PhaidraFile() { BaseDirectory = RootDir };
                    try
                    {
                        ProgressValue = i + 1;
                        ProgressText = $"Processing file {i + 1}/{ProgressMaximum}";

                        var row = rows[i];
                        Logger.LogI($"Processing file {i + 1}/{ProgressMaximum} \r\n({string.Join(";", row)})");
                        file.CsvRow = row;
                        Logger.LogI($"Creating Phaidra attributes");
                        file.Metadata = new JObject();

                        metadataGenerator.SetMetadata(row, file);

                        //foreach (PhaidraMapping mapping in mappings)
                        //{
                        //    var values = new Dictionary<string, string>();
                        //    foreach (var col in mapping.PhaidraAttributeFields2CsvColumns)
                        //    {
                        //        if (col.SelectedCsvColumn != CsvColumn.Empty)
                        //        {
                        //            values[col.Field.Key] = row[col.SelectedCsvColumn.Header.Key];
                        //        }
                        //    }
                        //    mapping.PhaidraAttribute.WriteAttributeContent(file, values);
                        //}


                        Logger.LogI($"Creating Phaidra attributes completed");
                        if (!string.IsNullOrEmpty(file.PhaidraId))
                        {
                            Logger.LogI($"file {file.PhaidraId} already uploaded");
                            // file already uploaded
                            continue;
                        }

                        if (PhaidraConfig.Instance.SimulateUpload == false)
                        {
                            string collectionPath = file.CollectionPath;
                            Logger.LogI($"current collection: {collectionPath}");

                            if (!string.IsNullOrEmpty(PhaidraConfig.Instance.CollectionRoot))
                            {
                                Logger.LogI($"using collection root: {PhaidraConfig.Instance.CollectionRoot}");
                                collectionPath = PhaidraConfig.Instance.CollectionRoot + "/" + collectionPath;
                            }
                            Logger.LogI($"used collection: {collectionPath}");
                            var createCollectionPathResult = await client.CreateCollectionPath(collectionPath);

                            if (createCollectionPathResult != null)
                            {
                                foreach (var createdCollection in createCollectionPathResult.CreatedCollections)
                                {
                                    Logger.LogI($"created collection: {createdCollection.Value} ({createdCollection.Key})");
                                }
                            }

                            Logger.LogI($"Uploading file");

                            var uploadResult = await client.UploadFileWithJsonMetadata(file.Fullfilename, file.Metadata.ToString(), file.MimeType, file.PhaidraType);
                            if (uploadResult.Success)
                            {
                                file.PhaidraId = uploadResult.CreatedPid;
                                Logger.LogI($"Upload successful. PID od uploaded file: {file.PhaidraId}");
                                var addCollectionMemberResult = await client.AddCollectionMembers(createCollectionPathResult.CollectionPathPid, new List<string> { uploadResult.CreatedPid });
                                if (addCollectionMemberResult.Success)
                                {
                                    Logger.LogI($"Add member of file PID {uploadResult.CreatedPid} to collection PID {createCollectionPathResult.CollectionPathPid} successful");
                                }
                                else
                                {
                                    Logger.LogE($"Add member of file PID {uploadResult.CreatedPid} to collection PID {createCollectionPathResult.CollectionPathPid} failed");
                                }
                            }
                            else
                            {
                                Logger.LogE($"Add member of file PID {uploadResult.CreatedPid} to collection PID {createCollectionPathResult.CollectionPathPid} failed");
                            }
                        }
                        else
                        {
                            file.PhaidraId = $"gen-id:{i}";
                            Logger.LogI($"Simulate upload (PID: {file.PhaidraId})");
                        }

                        metadataGenerator.WriteAttributeContentToCSV(file);

                        //foreach (PhaidraMapping mapping in mappings)
                        //{
                        //    var values = new Dictionary<string, string>();
                        //    mapping.PhaidraAttribute.WriteAttributeContentToCSV(file, values);
                        //    foreach (var col in mapping.PhaidraAttributeFields2CsvColumns)
                        //    {
                        //        if (col.SelectedCsvColumn != CsvColumn.Empty)
                        //        {
                        //            if (values.TryGetValue(col.Field.Key, out string value))
                        //            {
                        //                int colIndex = col.SelectedCsvColumn.Header.Key;
                        //                file.CsvRow[colIndex] = value;
                        //            }
                        //        }
                        //    }
                        //}

                        CsvReader.WriteCSVToDisk(CsvPath, headers, rows, ";", SelectedEncoding);

                        if (_cancellationPending)
                        {
                            Logger.LogI("Import cancelled");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogE($"Error in ProcessImport 2: {ex.ToString()}");
                        file.Errors.Add($"An unexpected exception has occurred: {ex.ToString()}");
                    }
                    WriteDebugFile(file, i);
                }
                ProgressText = "Processing finished";
            }
            catch(Exception ex)
            {
                Logger.LogE($"Unhandled Exception: {ex.ToString()}");
            }
        }

        private void WriteDebugFile(PhaidraFile file, int index)
        {
            if (file.Errors.Count > 0)
            {
                StringBuilder sbErr = new StringBuilder();
                sbErr.AppendLine("File has Errors: ");

                foreach (var error in file.Errors)
                {
                    sbErr.AppendLine($" - {error}");
                }

                Logger.LogE(sbErr.ToString());
            }

            if (!PhaidraConfig.Instance.WriteDebugFiles)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Method: {file.PhaidraType}");
            sb.AppendLine($"Full file name: {file.Fullfilename}");
            sb.AppendLine($"Mime type: {file.MimeType}");
            sb.AppendLine($"Collection path: {file.CollectionPath}");
            sb.AppendLine($"Errors: \r\n{string.Join("\r\n", file.Errors)}\r\n");
            sb.AppendLine($"Meta data: \r\n\r\n{file.Metadata.ToString()}");

            string filename = "";
            if (!string.IsNullOrEmpty(file.Fullfilename))
            {
                filename = "_" + Path.GetFileName(file.Fullfilename);
            }

            var fi = new FileInfo(Path.Combine(CsvFI.Directory.FullName, "DebugFiles", (index.ToString().PadLeft(4, '0')) + filename + ".txt"));

            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            File.WriteAllText(fi.FullName, sb.ToString());
        }

        private bool StartCanExecute(object arg)
        {
            return MappingsConfigured && !string.IsNullOrEmpty(CsvPath) && SelectedEncodingInfo != null;
        }

        private bool ValidateInput()
        {
            string csvPath = CsvPath ?? "";
            string a = RootDir ?? "";

            return true;
        }
    }
}
