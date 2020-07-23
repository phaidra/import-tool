namespace APS.Lib.Crawler
{
    internal class PhotoCreditInfo
    {
        public string Lastname { get; internal set; }
        public string Firstname { get; internal set; }
        public string Organisation { get; internal set; }
        public string Fullname { get; internal set; }
        public bool RootFileFound { get; internal set; }
        public bool ContentFound { get; internal set; }

        public string FullnameForFilename => (Fullname ?? "").Replace(" ", "_");
        public string OrganisationForFilename => (Organisation ?? "").Replace(" ", "_");

        public string Error { get; set; }
    }
}