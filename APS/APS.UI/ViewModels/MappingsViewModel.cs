using APS.Lib;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using APS.Lib.Helper;
using APS.UI.Helpers;

namespace APS.UI
{
    public class MappingsViewModel : VMBase
    {
        private PhaidraAttribute _selectedPhaidraAttribute;
        private PhaidraMapping _selectedMapping;
        private PhaidraAttributeField2CsvColumn2 _selectedMapping2;
        private Dictionary<int, string> _headers;
        private List<Dictionary<int, string>> _rows;
        private string _rootDir;
        private int _currentRowIndex;
        private ObservableCollection<PhaidraAttributeField2CsvColumn> _selectedMappingPhaidraAttributesField2CsvColumns;

        public string SelectedCsvPath { get; set; }
        public EncodingInfo SelectedEncoding { get; set; }

        private Window _parentWindow;

        public Command AddMappingCommand { get; set; }
        public Command DeleteSelectedMappingCommand { get; set; }
        public Command ResetMappingsCommand { get; set; }
        public Command OKCommand { get; set; }
        public Command PreviousRowCommand { get; set; }
        public Command NextRowCommand { get; set; }
        public Command SelectPhaidraAttributeFieldCommand { get; set; }
        public Command DuplicateCsvColumnCommand { get; set; }
        public Command DebugCommand { get; set; }

        public ObservableCollection<CsvColumn> CsvColumns { get; set; }
        public ObservableCollection<PhaidraAttribute> PhaidraAttributes { get; set; }
        //public ObservableCollection<PhaidraMapping> Mappings { get; set; }

        public ObservableCollection<PhaidraAttributeField2CsvColumn2> Mappings2 { get; set; }

        public ObservableCollection<PhaidraAttributeField> PhaidraAttributeFields { get; set; }

        public PhaidraAttribute SelectedPhaidraAttribute
        {
            get
            {
                return _selectedPhaidraAttribute;
            }
            set
            {
                _selectedPhaidraAttribute = value;
                RaisePropertyChanged(nameof(SelectedPhaidraAttribute));
                RevalidateCommands();
            }
        }

        public PhaidraMapping SelectedMapping
        {
            get
            {
                return _selectedMapping;
            }
            set
            {
                _selectedMapping = value;
                SelectedMappingPhaidraAttributesField2CsvColumns.Clear();
                if (SelectedMapping != null)
                {
                    foreach (var f2cMapping in SelectedMapping.PhaidraAttributeFields2CsvColumns)
                    {
                        SelectedMappingPhaidraAttributesField2CsvColumns.Add(f2cMapping);
                    }
                }
                RaisePropertyChanged(nameof(SelectedMapping));
                //if (SelectedMapping != null)
                //{
                //    //foreach (var f2cMapping in SelectedMapping.PhaidraAttributeFields2CsvColumns)
                //    //{
                //    //    f2cMapping.RaisePropertyChanged();
                //    //}
                //}
                RevalidateCommands();
            }
        }

        public PhaidraAttributeField2CsvColumn2 SelectedMapping2
        {
            get
            {
                return _selectedMapping2;
            }
            set
            {
                _selectedMapping2 = value;
                RaisePropertyChanged(nameof(SelectedMapping2));

                RevalidateCommands();
            }
        }

        public ObservableCollection<PhaidraAttributeField2CsvColumn> SelectedMappingPhaidraAttributesField2CsvColumns
        {
            get { return _selectedMappingPhaidraAttributesField2CsvColumns; }
            set
            {
                _selectedMappingPhaidraAttributesField2CsvColumns = value;
                RaisePropertyChanged(nameof(SelectedMappingPhaidraAttributesField2CsvColumns));
            }
        }

        public bool DialogResult { get; private set; }
        public Action CloseAction { get; internal set; }

        public string RowSelectionStatusText
        {
            get
            {
                if (_rows != null)
                {
                    return $"{_currentRowIndex + 1}/{_rows.Count}";
                }
                return "";
            }
        }

        public MappingsViewModel(Dictionary<int, string> headers, List<Dictionary<int, string>> rows, string rootDir)
        {
            _headers = headers;
            _rows = rows;
            _rootDir = rootDir;
            _currentRowIndex = 0;
            AddMappingCommand = new Command(AddMappingExecute, AddMappingCanExecute);
            DeleteSelectedMappingCommand = new Command(RemoveSelectedMappingExecute, RemoveSelectedMappingCanExecute);
            OKCommand = new Command(OKExecute);
            ResetMappingsCommand = new Command(ResetMappingsExecute);
            PreviousRowCommand = new Command(PreviousRowExecute, PreviousRowCanExecute);
            NextRowCommand = new Command(NextRowExecute, NextRowCanExecute);
            SelectPhaidraAttributeFieldCommand = new Command(SelectPhaidraAttributeFieldExecute, SelectPhaidraAttributeFieldCanExecute);
            DuplicateCsvColumnCommand = new Command(DuplicateCsvColumnExecute);
            DebugCommand = new Command(DebugExecute);
            CsvColumns = new ObservableCollection<CsvColumn>();
            PhaidraAttributes = new ObservableCollection<PhaidraAttribute>();
            //Mappings = new ObservableCollection<PhaidraMapping>();
            Mappings2 = new ObservableCollection<PhaidraAttributeField2CsvColumn2>();
            PhaidraAttributeFields = new ObservableCollection<PhaidraAttributeField>();
            SelectedMappingPhaidraAttributesField2CsvColumns = new ObservableCollection<PhaidraAttributeField2CsvColumn>();
            CsvColumns.Add(CsvColumn.Empty);
            foreach (var header in _headers)
            {
                CsvColumns.Add(new CsvColumn
                {
                    Name = header.Value,
                    Header = header
                });
            }

            SetCsvColumnValues();

            var attributes = PhaidraAttribute.GetPhaidraAttributes();
            var attributeFields = new List<PhaidraAttributeField>();
            attributeFields.Add(PhaidraAttributeField.Empty);
            foreach (var attribute in attributes)
            {
                PhaidraAttributes.Add(attribute);
                var set = new PhaidraMetadataSet(); //marks belonging fields (used when duplicating field sets)
                foreach (var field in attribute.Fields)
                {
                    attributeFields.Add(new PhaidraAttributeField()
                    {
                        PhaidraAttribute = attribute,
                        Field = field,
                        Set = set
                    });
                }
            }

            foreach (var attributeField in attributeFields.OrderBy(i => i.DisplayName))
            {
                PhaidraAttributeFields.Add(attributeField);
            }

            //PhaidraAttributes.Add(new FilenamePhaidraAttribute());
            //PhaidraAttributes.Add(new CollectionPhaidraAttribute());
            //PhaidraAttributes.Add(new AssociationPhaidraAttribute());
            //PhaidraAttributes.Add(new AccessRestrictionPhaidraAttribute());
            //PhaidraAttributes.Add(new TitlePhaidraAttribute());
            //PhaidraAttributes.Add(new DescriptionPhaidraAttribute());
            //PhaidraAttributes.Add(new DateArrayPhaidraAttribute("Creation date", "Creation date", "dcterms:created"));
            //PhaidraAttributes.Add(new DateArrayPhaidraAttribute("Date of production", "Date of production", "rdau:P60071"));
            //PhaidraAttributes.Add(new RolePhaidraAttribute("Photographer", "role:pht"));
            //PhaidraAttributes.Add(new RolePhaidraAttribute("Author", "role:aut"));

            ResetMappings();
        }

        internal void SetParentWindow(MappingsWindow2 window)
        {
            _parentWindow = window;
        }

        private void ResetMappingsExecute(object obj)
        {
            ResetMappings();
        }

        private void NextRowExecute(object obj)
        {
            if (NextRowCanExecute(obj))
            {
                _currentRowIndex++;
                SetCsvColumnValues();
            }
        }

        private bool NextRowCanExecute(object arg)
        {
            return _currentRowIndex + 1 < _rows.Count;
        }

        private void PreviousRowExecute(object obj)
        {
            if (PreviousRowCanExecute(obj))
            {
                _currentRowIndex--;
                SetCsvColumnValues();
            }
        }

        private bool PreviousRowCanExecute(object arg)
        {
            return _currentRowIndex > 0;
        }

        private async void SelectPhaidraAttributeFieldExecute(object obj)
        {
            PhaidraFieldBrowserViewModel vm = new PhaidraFieldBrowserViewModel(obj as PhaidraAttributeField2CsvColumn2, PhaidraAttributeFields);
            PhaidraFieldBrowserWindow window = new PhaidraFieldBrowserWindow(vm);
            vm.CloseAction = () => { window.Close(); };
            await window.ShowDialog(_parentWindow);
        }

        private bool SelectPhaidraAttributeFieldCanExecute(object arg)
        {
            return true;
        }

        private void DuplicateCsvColumnExecute(object obj)
        {
            if (SelectedMapping2 != null)
            {
                int index = Mappings2.IndexOf(SelectedMapping2);

                Mappings2.Insert(index + 1, new PhaidraAttributeField2CsvColumn2
                {
                    CsvColumn = SelectedMapping2.CsvColumn,
                    Field = PhaidraAttributeField.Empty
                });
                SelectedMapping2 = Mappings2[index + 1];
            }
        }

        private async void DebugExecute(object obj)
        {
            var vm = new DebugViewModel();

            bool valid;
            List<string> messages;
            var mappings = GetMappingsWithMessages(out valid, out messages);
            if (valid)
            {
                var metadataGenerator = new PhaidraMetadataGenerator(mappings);

                PhaidraFile file = new PhaidraFile();
                file.BaseDirectory = _rootDir;
                if (_currentRowIndex < _rows.Count)
                {
                    StringBuilder debugSb = new StringBuilder();
                    metadataGenerator.SetMetadata(_rows[_currentRowIndex], file);
                    debugSb.AppendLine("Metadata: ");
                    debugSb.AppendLine(file.Metadata.ToString());

                    debugSb.AppendLine("Full file name: ");
                    debugSb.AppendLine(file.Fullfilename);
                    debugSb.AppendLine("Collection path: ");
                    debugSb.AppendLine(file.CollectionPath);
                    debugSb.AppendLine("Phaidra type: ");
                    debugSb.AppendLine(file.PhaidraType);
                    debugSb.AppendLine("Mime type: ");
                    debugSb.AppendLine(file.MimeType);
                    if (file.Errors.Count > 0)
                    {
                        debugSb.AppendLine("File Errors: ");
                        foreach (var fileError in file.Errors)
                        {
                            debugSb.AppendLine($"- {fileError}");
                        }
                    }
                    vm.DebugText = debugSb.ToString();
                }
            }
            else
            {
                StringBuilder errorSb = new StringBuilder();
                errorSb.AppendLine("Errors: ");

                foreach (var message in messages)
                {
                    errorSb.AppendLine($"- {message}");
                }
                vm.DebugText = errorSb.ToString();
            }

            DebugWindow window = new DebugWindow(vm);
            await window.ShowDialog(_parentWindow);
        }

        private void SetCsvColumnValues()
        {
            foreach (var csvColumn in CsvColumns)
            {
                csvColumn.SetValue(_rows[_currentRowIndex]);
            }
            PreviousRowCommand.RaiseCanExecuteChanged();
            NextRowCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(RowSelectionStatusText));
        }

        private void ResetMappings()
        {
            foreach (var csvColumn in CsvColumns)
            {
                if (csvColumn != CsvColumn.Empty)
                {
                    Mappings2.Add(new PhaidraAttributeField2CsvColumn2
                    {
                        CsvColumn = csvColumn,
                        Field = PhaidraAttributeField.Empty
                    });
                }
            }




            //foreach (var mapping in Mappings.ToList())
            //{
            //    RemoveMapping(mapping);
            //}

            ////Mappings.Clear();
            //foreach (var attribute in PhaidraAttributes.ToList())
            //{
            //    if (attribute.IsRequired)
            //    {
            //        AddMapping(attribute);
            //    }
            //}
        }

        private void OKExecute(object obj)
        {
            bool allMappingsValid = true;

            bool isValid;
            var mappings = GetMappings(out isValid);
            allMappingsValid &= isValid;

            if (mappings != null)
            {
                foreach (var m in mappings)
                {
                    var mappingsValid = m.PhaidraAttribute.ValidateMappings(m);
                    if (!mappingsValid)
                    {
                        Logger.LogW($"Mappings of Attribute {m.PhaidraAttribute.Name} invalid");
                    }
                    allMappingsValid &= mappingsValid;
                }
            }
            if (allMappingsValid)
            {
                DialogResult = true;
                CloseAction?.Invoke();
            }
            else
            {
                DebugCommand.Execute(null);
            }
        }

        public List<PhaidraMapping> GetMappings(out bool isValid)
        {
            return GetMappingsWithMessages(out isValid, out _);
        }

        private List<PhaidraMapping> GetMappingsWithMessages(out bool isValid, out List<string> messages)
        {
            messages = new List<string>();
            isValid = true;

            var usedAttributeFields = new List<PhaidraAttributeField>();
            var usedAttributes = new List<PhaidraAttribute>();

            foreach (var mapping in this.Mappings2)
            {
                if (mapping.Field != PhaidraAttributeField.Empty)
                {
                    if (!usedAttributes.Contains(mapping.Field.PhaidraAttribute))
                    {
                        usedAttributes.Add(mapping.Field.PhaidraAttribute);
                    }

                    if (!usedAttributeFields.Contains(mapping.Field))
                    {
                        usedAttributeFields.Add(mapping.Field);
                    }
                    else
                    {
                        isValid = false;
                        string message = $"Field '{mapping.Field.DisplayName}' mapped multiple times";
                        messages.Add(message);
                        Logger.LogW(message);
                        return null;
                    }
                }
            }

            var mappingDict = new Dictionary<PhaidraMetadataSet, PhaidraMapping>();

            foreach (var mapping2 in this.Mappings2)
            {
                if (mapping2.Field != PhaidraAttributeField.Empty)
                {
                    if (!mappingDict.ContainsKey(mapping2.Field.Set))
                    {
                        mappingDict.Add(mapping2.Field.Set, new PhaidraMapping());
                        mappingDict[mapping2.Field.Set].PhaidraAttribute = mapping2.Field.PhaidraAttribute;
                    }

                    mappingDict[mapping2.Field.Set].PhaidraAttributeFields2CsvColumns.Add(new PhaidraAttributeField2CsvColumn
                    {
                        SelectedCsvColumn = mapping2.CsvColumn,
                        Field = mapping2.Field.Field
                    });
                }
            }

            foreach (var attribute in PhaidraAttributes)
            {
                if (attribute.IsRequired && !usedAttributes.Contains(attribute))
                {
                    isValid = false;
                    string message = $"Required Attribute '{attribute.Name}' is not mapped";
                    messages.Add(message);
                    Logger.LogW(message);
                }
            }

            return mappingDict.Values.ToList();
        }

        private void AddMappingExecute(object obj)
        {
            if (!AddMappingCanExecute(obj)) return;

            var phaidraAttribute = SelectedPhaidraAttribute;

            AddMapping(phaidraAttribute);

            SelectedPhaidraAttribute = null;
            RevalidateCommands();
        }

        private void AddMapping(PhaidraAttribute phaidraAttribute)
        {
            //if (phaidraAttribute.OnlyOneAllowed)
            //{
            //    if (Mappings.Any(i => i.PhaidraAttribute == phaidraAttribute))
            //    {
            //        return;
            //    }
            //    PhaidraAttributes.Remove(phaidraAttribute);
            //}

            //Mappings.Add(new PhaidraMapping
            //{
            //    PhaidraAttribute = phaidraAttribute,
            //    PhaidraAttributeFields2CsvColumns = new ObservableCollection<PhaidraAttributeField2CsvColumn>(phaidraAttribute.Fields.Select(field => new PhaidraAttributeField2CsvColumn() { Field = field, SelectedCsvColumn = CsvColumn.Empty }).ToList())
            //});
        }

        private bool AddMappingCanExecute(object arg)
        {
            return false;// return SelectedPhaidraAttribute != null;
        }

        private void RemoveSelectedMappingExecute(object obj)
        {
            //if (!RemoveSelectedMappingCanExecute(obj)) return;

            //if (Mappings.Contains(SelectedMapping))
            //{
            //    RemoveMapping(SelectedMapping);
            //}
        }

        private void RemoveMapping(PhaidraMapping mapping)
        {
            //int selectedIndex = Mappings.IndexOf(mapping);
            //var phaidraAttribute = mapping.PhaidraAttribute;
            //if (phaidraAttribute.OnlyOneAllowed)
            //{
            //    PhaidraAttributes.Add(phaidraAttribute);
            //}
            //mapping.PhaidraAttribute = null;
            //foreach (var paf2csv in mapping.PhaidraAttributeFields2CsvColumns)
            //{
            //    paf2csv.SelectedCsvColumn = null;
            //}
            //mapping.PhaidraAttributeFields2CsvColumns.Clear();
            //Mappings.Remove(mapping);
            //SelectedMapping = Mappings.Count == 0 ? null : Mappings[selectedIndex >= Mappings.Count ? Mappings.Count - 1 : selectedIndex];
        }

        private bool RemoveSelectedMappingCanExecute(object arg)
        {
            return false;// return SelectedMapping != null && SelectedMapping.PhaidraAttribute != null && !SelectedMapping.PhaidraAttribute.IsRequired;
        }

        private void RevalidateCommands()
        {
            AddMappingCommand.RaiseCanExecuteChanged();
            DeleteSelectedMappingCommand.RaiseCanExecuteChanged();
        }
    }
}
