namespace Data
{
    public class ResultsService
    {
        public List<string> CorrectWords { get; set; } = new();
        public List<string> SkippedWords { get; set; } = new();

        public void Clear()
        {
            CorrectWords.Clear();
            SkippedWords.Clear();
        }
    }
}