using Microsoft.AspNetCore.Components;

namespace ViewModels
{
    public interface IPlayViewModel
    {
        List<string> Categories { get; }
        string SelectedCategory { get; set; }

        List<string> CorrectWords { get; }
        List<string> SkippedWords { get; }
        string CurrentWord { get; }
        string TimerDisplay { get; }
        bool IsTimerRunning { get; }
        bool HasGameStarted { get; }

        event Action? StateChanged;
        Action? NotifyUI { get; set; }

        Task InitializeAsync();
        Task LoadWordsAsync();
        Task OnCategoryChanged(ChangeEventArgs e);
        void OnCorrect();
        void OnSkip();
        void IncreaseTime();
        void DecreaseTime();
        void ToggleTimer();
        Task ResetGameAsync();
    }
}