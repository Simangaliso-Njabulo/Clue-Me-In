using Data;
using Microsoft.AspNetCore.Components;
using System.Numerics;

namespace Managers
{
    public class GameManager : IGameManager
    {
        private readonly WordsService _wordsService;
        private readonly NavigationManager _navigationManager;

        private List<string> _categories = new();
        private List<string> _words = new();
        private readonly TimeSpan _maxTime = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _minTime = TimeSpan.FromSeconds(30);
        private TimeSpan _remainingTime = TimeSpan.FromMinutes(1);

        private CancellationTokenSource _cts = new();

        public bool IsTimerRunning { get; private set; } = false;
        public bool HasGameStarted { get; private set; } = false;
        public string CurrentWord { get; private set; } = string.Empty;
        public string TimerDisplay { get; private set; } = "01:00";

        public List<string> CorrectWords { get; } = new();
        public List<string> SkippedWords { get; } = new();

        public event Action? StateChanged;

        public GameManager(WordsService wordsService, NavigationManager navigationManager)
        {
            _wordsService = wordsService;
            _navigationManager = navigationManager;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            _categories = await _wordsService.GetCategoriesAsync();
            return _categories;
        }

        public async Task InitializeAsync(string category)
        {
            _words = await _wordsService.GetWordsAsync(category);
            Shuffle(_words);
            GetNextWord();
            UpdateTimerDisplay();
            NotifyStateChanged();
        }

        public void StartTimer()
        {
            if (IsTimerRunning) return;

            HasGameStarted = true;
            IsTimerRunning = true;
            _cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (_remainingTime.TotalSeconds > 0 && !_cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                    _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
                    UpdateTimerDisplay();
                    NotifyStateChanged();
                }

                if (_remainingTime.TotalSeconds <= 0)
                {
                    EndGame();
                }

                StopTimer();
            });
        }

        public void StopTimer()
        {
            IsTimerRunning = false;
            _cts.Cancel();
            NotifyStateChanged();
        }

        public void ResetGame()
        {
            HasGameStarted = false;
            IsTimerRunning = false;
            _remainingTime = TimeSpan.FromMinutes(1);
            CorrectWords.Clear();
            SkippedWords.Clear();
            UpdateTimerDisplay();
            NotifyStateChanged();
        }

        public void MarkAsCorrect()
        {
            CorrectWords.Add(CurrentWord);
            GetNextWord();
            NotifyStateChanged();
        }

        public void MarkAsSkipped()
        {
            SkippedWords.Add(CurrentWord);
            GetNextWord();
            NotifyStateChanged();
        }

        public void IncreaseTime()
        {
            if (IsTimerRunning) return;

            _remainingTime = (_remainingTime + TimeSpan.FromSeconds(30)) > _maxTime
                ? _maxTime
                : _remainingTime + TimeSpan.FromSeconds(30);

            UpdateTimerDisplay();
            NotifyStateChanged();
        }

        public void DecreaseTime()
        {
            if (IsTimerRunning) return;

            _remainingTime = (_remainingTime - TimeSpan.FromSeconds(30)) < _minTime
                ? _minTime
                : _remainingTime - TimeSpan.FromSeconds(30);

            UpdateTimerDisplay();
            NotifyStateChanged();
        }

        private void GetNextWord()
        {
            if (_words.Count == 0)
            {
                SkippedWords.Add("No words available in last category please choose another category when playing again");
                EndGame();
                return;
            }

            var rand = new Random();
            int index = rand.Next(_words.Count);
            CurrentWord = _words[index];
            _words.RemoveAt(index);
        }

        private void Shuffle(List<string> list)
        {
            Random rng = new();
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        private void UpdateTimerDisplay()
        {
            TimerDisplay = _remainingTime.ToString(@"mm\:ss");
        }

        private void EndGame()
        {
            _navigationManager.NavigateTo("/results");
        }

        private void NotifyStateChanged() => StateChanged?.Invoke();

    }
}
