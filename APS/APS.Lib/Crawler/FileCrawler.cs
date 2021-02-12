using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using APS.Lib.Helper;

namespace APS.Lib.Crawler
{
    public class FileCrawler
    {
        private string _rootDir;
        private string _crawlSubDir;
        private string _csvPath;
        private string _extensions;

        private List<CrawlerFile> _files;

        public void Initialize(string rootDir, string crawlSubDir, string csvPath, string extensions)
        {

            _rootDir = (rootDir ?? "").TrimEnd(System.IO.Path.DirectorySeparatorChar);
            if (System.IO.Directory.Exists(_rootDir))
            {
                _rootDir += System.IO.Path.DirectorySeparatorChar;
            }
            _crawlSubDir = crawlSubDir ?? "";
            _csvPath = csvPath ?? System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Crawler.csv");
            _extensions = extensions ?? "jpg;jpeg";
            _files = new List<CrawlerFile>();
        }

        public bool TestParameters()
        {
            bool success = true;
            Console.WriteLine("Parameters:");
            Console.WriteLine($" rootDir : {_rootDir}");
            Console.WriteLine($" crawlSubDir : {_crawlSubDir}");
            Console.WriteLine($" csvPath : {_csvPath}");
            Console.WriteLine($" extensions : {_extensions}");
            Console.WriteLine();

            if (!System.IO.Directory.Exists(_rootDir))
            {
                Console.WriteLine($"Error: rootDir \"{_rootDir}\" does not exist");
                success = false;
            }

            return success;
        }

        public void Crawl()
        {
            Console.WriteLine("Crawling started");
            _files.Clear();

            string path = System.IO.Path.Combine(_rootDir, _crawlSubDir);

            var crawlDI = new System.IO.DirectoryInfo(path);
            CrawlRecursive(crawlDI, 0, "");

            Console.WriteLine("Crawling finished");
            Console.WriteLine("Writing CSV file");
            try
            {
                using (var csvWriter = new CsvWriter(_csvPath.Replace("{datetime}", DateTime.Now.ToString("yyyyMMddHHmmss")), ";", "\r\n"))
                {
                    csvWriter.WriteLine(
                            "RelativePath",
                            "OriginalFileName",
                            "GeneratedFileName",
                            "PhotographerFullName",
                            "PhotographerFirstname",
                            "PhotographerLastname",
                            "PhotographerOrganisation",
                            "PhotoCreditError",
                            "CreationTime",
                            "LastWriteTime",
                            "Collection",
                            "phaidra Identifier"
                        );
                    foreach (var file in _files)
                    {
                        csvWriter.WriteLine(
                            file.RelativePath,
                            file.OriginalFileName,
                            file.GeneratedFileName,
                            file.PhotographerFullName,
                            file.PhotographerFirstname,
                            file.PhotographerLastname,
                            file.PhotographerOrganisation,
                            file.PhotoCreditError,
                            file.CreationTime,
                            file.LastWriteTime,
                            file.Collection,
                            ""
                            );
                    }
                }
                Console.WriteLine("Writing CSV file finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Writing CSF file failed. Error message: {ex.Message}");
            }
        }

        private void CrawlRecursive(DirectoryInfo crawlDI, int level, string collectionPath)
        {
            Console.WriteLine($"{"".PadLeft(level, ' ')}- {crawlDI.Name}");

            var photoCredit = GetPhotoCredit(crawlDI);

            string[] extensions = _extensions.Split(';');

            int fileIndex = 1;
            try
            {
                foreach (var extension in extensions)
                {
                    var files = crawlDI.GetFiles("*." + extension);
                    foreach (var file in files)
                    {
                        _files.Add(new CrawlerFile
                        {
                            OriginalFileName = file.Name,
                            CreationTime = file.CreationTimeUtc,
                            LastWriteTime = file.LastWriteTimeUtc,
                            GeneratedFileName = GenerateFileName(file, crawlDI, fileIndex, photoCredit),
                            PhotographerFullName = photoCredit.Fullname,
                            PhotographerOrganisation = photoCredit.Organisation,
                            PhotographerFirstname = photoCredit.Firstname,
                            PhotographerLastname = photoCredit.Lastname,
                            RelativePath = file.FullName.Substring(_rootDir.Length),
                            PhotoCreditError = photoCredit.Error,
                            Collection = GetCollectionName(file.Directory.FullName)
                        }) ;
                        fileIndex++;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            try
            {
                var subDIs = crawlDI.GetDirectories();
                foreach (var subDI in subDIs)
                {
                    CrawlRecursive(subDI, level + 1, (collectionPath != "" ? collectionPath + "/" : "") + subDI.Name);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private string GetCollectionName(string directoryName)
        {
            return directoryName.Substring(_rootDir.Length);
        }

        private string GenerateFileName(FileInfo fileFI, DirectoryInfo crawlDI, int fileIndex, PhotoCreditInfo photoCredit)
        {
            var photoCreditParts = new List<string>();
            if (!string.IsNullOrEmpty(photoCredit.FullnameForFilename))
            {
                photoCreditParts.Add(photoCredit.FullnameForFilename);
            }
            if (!string.IsNullOrEmpty(photoCredit.OrganisationForFilename))
            {
                photoCreditParts.Add(photoCredit.OrganisationForFilename);
            }
            return $"KUG_{fileIndex.ToString().PadLeft(3, '0')}_{crawlDI.Name}{(photoCreditParts.Count > 0 ? "_Foto_" + string.Join("_", photoCreditParts) : "")}{fileFI.Extension}";
        }

        private PhotoCreditInfo GetPhotoCredit(DirectoryInfo crawlDI)
        {
            var photoCredit = new PhotoCreditInfo();
            try
            {
                var photoCreditFiles = crawlDI.GetFiles("fotocredit_*.txt");

                if (photoCreditFiles.Length == 1)
                {
                    string photoCreditFileName = photoCreditFiles[0].FullName;
                    string rootPhotoCreditPath = System.IO.Path.Combine(_rootDir, photoCreditFiles[0].Name);
                    if (System.IO.File.Exists(rootPhotoCreditPath))
                    {
                        photoCredit.RootFileFound = true;
                        photoCreditFileName = rootPhotoCreditPath;
                    }
                    string photoCreditFileContent = System.IO.File.ReadAllText(photoCreditFileName);

                    if (!string.IsNullOrEmpty(photoCreditFileContent))
                    {
                        var photoCreditFileContentParts = photoCreditFileContent.Split(':');
                        if (photoCreditFileContentParts.Length == 2)
                        {
                            photoCredit.ContentFound = true;
                            var photoCreditParts = photoCreditFileContentParts[1].Split('/');
                            photoCredit.Fullname = photoCreditParts[0].Trim();
                            var nameParts = photoCredit.Fullname.Split(' ');
                            photoCredit.Lastname = nameParts.Last();
                            if (nameParts.Length > 1)
                            {
                                photoCredit.Firstname = string.Join(" ", nameParts.Take(nameParts.Length - 1));
                            }

                            if (photoCreditParts.Length >= 2)
                            {
                                photoCredit.Organisation = photoCreditParts[1];
                            }
                        }
                        else
                        {
                            photoCredit.Error = "'photocredit_*.txt' file has unexpected format";
                        }
                    }
                    else
                    {
                        photoCredit.Error = "'photocredit_*.txt' file empty";
                    }
                }
                else if (photoCreditFiles.Length > 1)
                {
                    photoCredit.Error = "More than one 'photocredit_*.txt' file found";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
            }

            return photoCredit;
        }
    }
}
