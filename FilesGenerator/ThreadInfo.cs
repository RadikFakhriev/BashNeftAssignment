namespace FilesGenerator
{
    public class ThreadInfo
    {
        public string FileName { get; set; }
        public string OutPath { get; set; }
        public int MaxLineLength { get; set; }
        public int MaxLineCount { get; set; }
        public CancellationToken CancelToken { get; set; }

        public ThreadInfo ShallowCopy()
        {
            return (ThreadInfo)this.MemberwiseClone();
        }
    }
}
