using Data;
using FluentAssertions;
using Managers;
using Microsoft.AspNetCore.Components;
using NSubstitute;


namespace Clue_Me_In.Tests
{
    [TestFixture]
    public class GameManagerTests
    {
        private IWordsService _wordsServiceSub;
        private NavigationManager _navManagerSub;
        private GameManager _gameManager;

        [SetUp]
        public void SetUp()
        {
            _wordsServiceSub = Substitute.For<IWordsService>();
            _navManagerSub = Substitute.For<NavigationManager>();
            _gameManager = new GameManager(_wordsServiceSub, _navManagerSub);
        }

        [Test]
        public async Task GetCategoriesAsync_ShouldReturnCategoriesFromService()
        {
            // Arrange
            var categories = new List<string> { "Category1", "Category2" };
            _wordsServiceSub.GetCategoriesAsync().Returns(categories);

            // Act
            var result = await _gameManager.GetCategoriesAsync();

            // Assert
            result.Should().BeEquivalentTo(categories);
        }


        [Test]
        public async Task InitializeAsync_ShouldCallGetWordsAsyncOnWordsService()
        {
            // Arrange
            var words = new List<string> { "WordA" };
            _wordsServiceSub.GetWordsAsync(Arg.Any<string>()).Returns(words);

            // Act
            await _gameManager.InitializeAsync("AnyCategory");

            // Assert
            await _wordsServiceSub.Received(1).GetWordsAsync("AnyCategory");
        }


        [Test]
        public async Task InitializeAsync_ShouldLoadWordsAndSetFirstWord()
        {
            // Arrange
            var words = new List<string> { "Apple", "Banana" };
            _wordsServiceSub.GetWordsAsync(Arg.Any<string>()).Returns(words);

            // Act
            await _gameManager.InitializeAsync("Fruit");

            // Assert
            _gameManager.CurrentWord.Should().NotBeNullOrEmpty();
            _gameManager.CurrentWord.Should().NotBe("No words in this category choose another one");
        }

        [Test]
        public void IncreaseTime_ShouldAdd30Seconds()
        {
            // Act
            _gameManager.IncreaseTime();

            // Assert
            _gameManager.TimerDisplay.Should().Be("01:30");
        }

        [Test]
        public void DecreaseTime_ShouldNotGoBelowMinimum()
        {
            // Arrange
            for (int i = 0; i < 10; i++)
            {
                _gameManager.DecreaseTime(); // Keep decreasing time
            }

            // Assert
            _gameManager.TimerDisplay.Should().Be("00:30"); // Min time is 30 seconds
        }

        [Test]
        public void IncreaseTime_ShouldNotExceedMaximum()
        {
            // Arrange
            for (int i = 0; i < 10; i++)
            {
                _gameManager.IncreaseTime(); // Keep increasing time
            }

            // Assert
            _gameManager.TimerDisplay.Should().Be("05:00"); // Max time is 5 minutes
        }

        [Test]
        public async Task StartTimer_ShouldStartTimerAndSetHasGameStarted()
        {
            // Arrange
            var words = new List<string> { "TestWord1", "TestWord1" };
            _wordsServiceSub.GetWordsAsync(Arg.Any<string>()).Returns(words);
            await _gameManager.InitializeAsync("TestCategory");

            // Act
            _gameManager.StartTimer();

            // Assert
            _gameManager.IsTimerRunning.Should().BeTrue();
            _gameManager.HasGameStarted.Should().BeTrue();
        }

        [Test]
        public void StopTimer_ShouldStopTimerAndSetIsTimerRunningFalse()
        {
            // Arrange
            _gameManager.StartTimer();

            // Act
            _gameManager.StopTimer();

            // Assert
            _gameManager.IsTimerRunning.Should().BeFalse();
        }

        private void SetupGameManagerWithWords(List<string> words)
        {
            _wordsServiceSub.GetWordsAsync(Arg.Any<string>()).Returns(words);
            _gameManager.InitializeAsync("AnyCategory").Wait();
        }

        [Test]
        public void MarkAsCorrect_ShouldAddCurrentWordToCorrectWords()
        {
            // Arrange
            SetupGameManagerWithWords(new List<string> { "TestWord" });

            // Act
            _gameManager.MarkAsCorrect();

            // Assert
            _gameManager.CorrectWords.Should().Contain("TestWord");
        }

        [Test]
        public void MarkAsSkipped_ShouldAddCurrentWordToSkippedWords()
        {
            // Arrange
            SetupGameManagerWithWords(new List<string> { "SkippedWord" });

            // Act
            _gameManager.MarkAsSkipped();

            // Assert
            _gameManager.SkippedWords.Should().Contain("SkippedWord");
        }


        [Test]
        public void ResetGame_ShouldResetStateAndClearWordLists()
        {
            // Arrange
            SetupGameManagerWithWords(new List<string> { "Word1" });
            _gameManager.MarkAsCorrect();
            _gameManager.MarkAsSkipped();
            _gameManager.StartTimer();

            // Act
            _gameManager.ResetGame();

            // Assert
            _gameManager.HasGameStarted.Should().BeFalse();
            _gameManager.IsTimerRunning.Should().BeFalse();
            _gameManager.TimerDisplay.Should().Be("01:00");
            _gameManager.CorrectWords.Should().BeEmpty();
            _gameManager.SkippedWords.Should().BeEmpty();
        }
    }
}