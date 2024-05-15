namespace LetterCounter
{
    public class ThreadInfo
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public CancellationToken CancelToken { get; set; }

        public ThreadInfo ShallowCopy()
        {
            return (ThreadInfo)this.MemberwiseClone();
        }
    }
}
