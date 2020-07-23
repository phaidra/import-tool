namespace APS.Lib.Import
{
    public class UploadFileResult
    {
        public bool Success { get; internal set; }
        public long Status { get; internal set; }
        public string CreatedPid { get; internal set; }
    }
}