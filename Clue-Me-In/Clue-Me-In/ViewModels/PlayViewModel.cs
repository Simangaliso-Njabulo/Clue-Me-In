using Managers;
using Microsoft.AspNetCore.Components;

namespace ViewModels
{
    public class PlayViewModel : IPlayViewModel
    {
        private readonly IGameManager _gameManager;

        public List<string> Categories { get; private set; } = new();
        public string SelectedCategory { get; set; } = string.Empty;

        public string CurrentWord => _gameManager.CurrentWord;
        public string TimerDisplay => _gameManager.TimerDisplay;
        public bool IsTimerRunning => _gameManager.IsTimerRunning;
        public bool HasGameStarted => _gameManager.HasGameStarted;

        public List<string> CorrectWords => _gameManager.CorrectWords;
        public List<string> SkippedWords => _gameManager.SkippedWords;

        public event Action? StateChanged;

        public PlayViewModel(IGameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.StateChanged += () => StateChanged?.Invoke();
        }

        public async Task InitializeAsync()
        {
            Categories = await _gameManager.GetCategoriesAsync();
            SelectedCategory = Categories.FirstOrDefault() ?? string.Empty;
            await _gameManager.InitializeAsync(SelectedCategory);
        }

        public async Task OnCategoryChanged(ChangeEventArgs e)
        {
            SelectedCategory = e.Value?.ToString() ?? string.Empty;
            await _gameManager.InitializeAsync(SelectedCategory);
        }

        public void OnCorrect() => _gameManager.MarkAsCorrect();
        public void OnSkip() => _gameManager.MarkAsSkipped();
        public void ToggleTimer()
        {
            if (_gameManager.IsTimerRunning)
                _gameManager.StopTimer();
            else
                _gameManager.StartTimer();
        }

        public void IncreaseTime() => _gameManager.IncreaseTime();
        public void DecreaseTime() => _gameManager.DecreaseTime();
    }
}
