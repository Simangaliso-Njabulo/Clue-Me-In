namespace Data
{
    public interface IWordsService
    {
        Task<List<string>> GetWordsAsync(string category);
        Task<List<string>> GetCategoriesAsync();
    }
}