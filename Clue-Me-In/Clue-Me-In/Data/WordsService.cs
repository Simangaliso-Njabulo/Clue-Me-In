using System.Text.Json;

namespace Data
{
    public class WordsService: IWordsService
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
                var jsonFilePath = "data/Words.json";

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

            if (_wordCategories.TryGetValue(category, out var words))
            {
                return words;
            }

            return new List<string>(); // Return empty list if category doesn't exist
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            // If word categories are empty, load them from the JSON file
            if (_wordCategories == null || _wordCategories.Count == 0)
            {
                await LoadWordsFromJsonAsync();
            }

            return _wordCategories.Keys.ToList(); // Return the list of category names
        }
    }
}
