using Data;
using Microsoft.AspNetCore.Components;

namespace ViewModels
{
    public class PlayViewModel
    {
        private readonly WordsService _wordsService;
        private readonly ResultsService _resultsService;
        private readonly NavigationManager _navigationManager;

        public List<string> Categories { get; private set; } = new();
        public string SelectedCategory { get; set; } = string.Empty;

        public List<string> CorrectWords { get; } = new();
        public List<string> SkippedWords { get; } = new();
        public string CurrentWord { get; private set; } = string.Empty;
        public string TimerDisplay { get; private set; } = "01:00";
        public bool IsTimerRunning { get; private set; } = false;
        public bool HasGameStarted { get; private set; } = false;

        private List<string> _words = new();
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly TimeSpan _maxTime = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _minTime = TimeSpan.FromSeconds(30);
        private TimeSpan _remainingTime = TimeSpan.FromMinutes(0.1);

        public event Action? StateChanged;
        public Action? NotifyUI { get; set; }

        public PlayViewModel(WordsService wordsService, ResultsService resultsService, NavigationManager navigationManager)
        {
            _wordsService = wordsService;
            _resultsService = resultsService;
            _navigationManager = navigationManager;
        }

        public async Task InitializeAsync()
        {
            Categories = await _wordsService.GetCategoriesAsync();
            if (Categories.Count > 0)
            {
                SelectedCategory = Categories[0];
                await LoadWordsAsync();
            }
            UpdateTimerDisplay();
            NotifyStateChanged();
        }

        public async Task LoadWordsAsync()
        {
            _words = await _wordsService.GetWordsAsync(SelectedCategory);
            Shuffle(_words);
            GetNextWord();
        }

        public async Task OnCategoryChanged(ChangeEventArgs e)
        {
            SelectedCategory = e.Value?.ToString() ?? string.Empty;
            await LoadWordsAsync();
        }

        public void OnCorrect()
        {
            CorrectWords.Add(CurrentWord);
            GetNextWord();
            NotifyStateChanged();
        }

        public void OnSkip()
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

        public void ToggleTimer()
        {
            if (IsTimerRunning)
                StopCountdown();
            else
                StartCountdown();
        }

        private void StartCountdown()
        {
            HasGameStarted = true;
            IsTimerRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (_remainingTime.TotalSeconds > 0 && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                    _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
                    UpdateTimerDisplay();
                    NotifyUI?.Invoke();
                }

                if (_remainingTime.TotalSeconds <= 0)
                {
                    await ResetGameAsync();
                    NotifyUI?.Invoke();
                    await GoToResultsPage();
                }

                StopCountdown();
            });
        }

        private void StopCountdown()
        {
            IsTimerRunning = false;
            _cancellationTokenSource.Cancel();
            NotifyStateChanged();
        }

        private void GetNextWord()
        {
            if (_words.Count == 0)
            {
                CurrentWord = "No words available. Please choose another category or reload page";
                StopCountdown();
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

        private async Task GoToResultsPage()
        {
            _resultsService.CorrectWords = CorrectWords;
            _resultsService.SkippedWords = SkippedWords;
            _navigationManager.NavigateTo("/results");
            await Task.CompletedTask;
        }

        public async Task ResetGameAsync()
        {
            HasGameStarted = false;
            IsTimerRunning = false;

            _remainingTime = TimeSpan.FromMinutes(1);
            UpdateTimerDisplay();

            await LoadWordsAsync();
            NotifyStateChanged();
        }


        private void UpdateTimerDisplay()
        {
            TimerDisplay = _remainingTime.ToString(@"mm\:ss");
        }

        private void NotifyStateChanged() => StateChanged?.Invoke();
    }
}

