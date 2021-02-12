using System;

namespace APS.Lib.Crawler
{
    public class CrawlerFile
    {
        public string OriginalFileName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string GeneratedFileName { get; set; }
        public string PhotographerFullName { get; set; }
        public string PhotographerOrganisation { get; set; }
        public string PhotographerFirstname { get; set; }
        public string PhotographerLastname { get; set; }
        public string RelativePath { get; set; }

        public string Collection { get; set; }
        public string PhotoCreditError { get; set; }
    }
}