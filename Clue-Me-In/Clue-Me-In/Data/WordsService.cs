using System.Text.Json;

namespace Data
{
    public class WordsService
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, List<string>> _wordCategories = [];

        public WordsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Initiate async method to load the words from JSON
            _ = LoadWordsFromJsonAsync();
        }

        private async Task LoadWordsFromJsonAsync()
        {
            try
            {
                var jsonFilePath = "Words.json";

                var jsonData = await _httpClient.GetStringAsync(jsonFilePath);

                if (!string.IsNullOrWhiteSpace(jsonData))
                {
                    _wordCategories = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonData) ?? [];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON: {ex.Message}");
                _wordCategories = [];
            }
        }

        public async Task<List<string>> GetWordsAsync(string category)
        {
            // If word categories are empty, load them from the JSON file
            if (_wordCategories == null || _wordCategories.Count == 0)
            {
                await LoadWordsFromJsonAsync();
            }

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (_wordCategories.TryGetValue(category, out var words))
            {
                return words;
            }
            #pragma warning restore CS8602 // Dereference of a possibly null reference.

            return new List<string>(); // Return empty list if category doesn't exist
        }
    }
}