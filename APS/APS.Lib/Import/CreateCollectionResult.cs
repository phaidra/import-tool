namespace APS.Lib.Import
{
    public class CreateCollectionResult
    {
        public bool Success { get; set; }
        public string CreatedPid { get; internal set; }
        public long Status { get; internal set; }
    }
}