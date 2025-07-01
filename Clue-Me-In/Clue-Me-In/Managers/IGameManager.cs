namespace Managers
{
    public interface IGameManager
    {
        bool IsTimerRunning { get; }
        bool HasGameStarted { get; }
        string CurrentWord { get; }
        string TimerDisplay { get; }

        List<string> CorrectWords { get; }
        List<string> SkippedWords { get; }

        Task<List<string>> GetCategoriesAsync();
        Task InitializeAsync(string category);

        void StartTimer();
        void StopTimer();
        void ResetGame();
        void MarkAsCorrect();
        void MarkAsSkipped();
        void IncreaseTime();
        void DecreaseTime();

        event Action StateChanged;
    }
}

