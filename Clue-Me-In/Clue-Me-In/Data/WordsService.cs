namespace Data
{
    public class WordsService
    {
        // Store the words in a list or retrieve from a database, API, etc.
        private List<string> _words = new List<string>
        {
            "Hello World",
            "Goodbye",
            "How are you?",
            "Blazor is amazing",
            "Fluent UI is sleek",
            "Design systems rock!"
        };

        // Method to get all words
        public List<string> GetWords()
        {
            return _words;
        }

        // Method to add a word to the list
        public void AddWord(string word)
        {
            if (!string.IsNullOrEmpty(word) && !_words.Contains(word))
            {
                _words.Add(word);
            }
        }
    }
}
