using Microsoft.AspNetCore.Components;

namespace ViewModels
{
    public interface IPlayViewModel
    {
        List<string> Categories { get; }
        string SelectedCategory { get; set; }

        string CurrentWord { get; }
        string TimerDisplay { get; }
        bool IsTimerRunning { get; }
        bool HasGameStarted { get; }

        List<string> CorrectWords { get; }
        List<string> SkippedWords { get; }

        event Action? StateChanged;

        Task InitializeAsync();
        Task OnCategoryChanged(ChangeEventArgs e);

        void OnCorrect();
        void OnSkip();
        void ToggleTimer();
        void IncreaseTime();
        void DecreaseTime();
    }
}