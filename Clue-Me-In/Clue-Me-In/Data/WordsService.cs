using System.Text.Json;

namespace Data
{
    public class WordsService
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, List<string>> _wordCategories;

        public WordsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Initiate async method to load the words from JSON
            _ = LoadWordsFromJsonAsync();
            _wordCategories = new Dictionary<string, List<string>>();
        }

        private async Task LoadWordsFromJsonAsync()
        {
            try
            {
                // Replace this with the actual path of your JSON file
                var jsonFilePath = "Words.json";

                var jsonData = await _httpClient.GetStringAsync(jsonFilePath);

                if (!string.IsNullOrWhiteSpace(jsonData))
                {
                    _wordCategories = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON: {ex.Message}");
                _wordCategories = new Dictionary<string, List<string>>();
            }
        }

        public async Task<List<string>> GetWordsAsync(string category)
        {
            // If word categories are empty, load them from the JSON file
            if (_wordCategories == null || _wordCategories.Count == 0)
            {
                await LoadWordsFromJsonAsync();
            }

            if (_wordCategories.TryGetValue(category, out var words))
            {
                return words;
            }

            return new List<string>(); // Return empty list if category doesn't exist
        }
    }
}
