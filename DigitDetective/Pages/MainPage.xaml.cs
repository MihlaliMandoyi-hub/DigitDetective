// Group X | Surname, FirstName, StudentNumber | Surname, FirstName, StudentNumber | ...
using System.Collections.ObjectModel;

namespace DigitDetective;

public partial class MainPage : ContentPage
{
    private const string BestKey = "BestScore";

    private readonly GameEngine _game = new();
    private readonly List<int> _current = new();
    private readonly ObservableCollection<GuessResult> _history = new();
    private readonly Label[] _slots;
    private readonly AudioService _audio;
    private readonly LeaderboardService _leaderboard;
    private readonly AchievementService _achievements;
    private readonly ProgressionService _progression;
    private IDispatcherTimer _timer = null!;
    private TimeSpan _elapsed;

    public MainPage(AudioService audio, LeaderboardService leaderboard,
        AchievementService achievements, ProgressionService progression)
    {
        InitializeComponent();
        _audio = audio;
        _leaderboard = leaderboard;
        _achievements = achievements;
        _progression = progression;

        _slots = new[] { Slot0, Slot1, Slot2 };
        HistoryList.ItemsSource = _history;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;

        StartNewGame();

        SoundSwitch.IsToggled = _audio.IsSoundOn;
        _ = _audio.SetupLoopingMusicAsync("game_music.wav");

        RefreshDifficultyButtons();
        RefreshHintButton();
    }

    private void StartNewGame()
    {
        _game.NewGame();
        _history.Clear();
        _current.Clear();
        _elapsed = TimeSpan.Zero;

        GuessesLabel.Text = "0";
        TimerLabel.Text = "00:00";
        ShowBestScore();
        RefreshSlots();
        SetFeedback("Tap three different digits, then press Guess.", Colors.Aquamarine);

        _timer.Start();
    }

    private void ShowBestScore()
    {
        int best = Preferences.Default.Get(BestKey, 0);
        BestLabel.Text = best == 0 ? "-" : best.ToString();
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < 3; i++)
            _slots[i].Text = i < _current.Count ? _current[i].ToString() : "?";
    }

    private void SetFeedback(string text, Color colour)
    {
        FeedbackLabel.Text = text;
        FeedbackLabel.TextColor = colour;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _elapsed += TimeSpan.FromSeconds(1);
        TimerLabel.Text = _elapsed.ToString(@"mm\:ss");
    }

    private async void OnDigitClicked(object? sender, EventArgs e)
    {
        int digit = int.Parse(((Button)sender!).Text);

        if (_current.Count >= 3)
        {
            SetFeedback("Three digits chosen - press Guess or Clear.", Colors.Orange);
            return;
        }
        if (_current.Contains(digit))
        {
            SetFeedback("Each digit must be different!", Colors.Salmon);
            await ShakeSlotsAsync();
            return;
        }

        _current.Add(digit);
        RefreshSlots();
        Buzz();
        SetFeedback(_current.Count == 3 ? "Ready - press Guess!" : "Keep going...", Colors.Aquamarine);
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        _current.Clear();
        RefreshSlots();
        SetFeedback("Cleared. Pick three different digits.", Colors.Aquamarine);
    }

    private async void OnGuessClicked(object? sender, EventArgs e)
    {
        if (_current.Count < 3)
        {
            SetFeedback("You need three digits first.", Colors.Salmon);
            await ShakeSlotsAsync();
            return;
        }

        GuessResult result = _game.Evaluate(_current);
        _history.Insert(0, result);
        GuessesLabel.Text = _game.GuessCount.ToString();

        if (result.IsWin)
        {
            await HandleWinAsync();
            return;
        }

        SetFeedback(result.Summary, Colors.Gold);
        _current.Clear();
        RefreshSlots();
    }

    private async Task HandleWinAsync()
    {
        _timer.Stop();
        Buzz(isLong: true);

        int guesses = _game.GuessCount;
        int timeSeconds = (int)_elapsed.TotalSeconds;

        int best = Preferences.Default.Get(BestKey, 0);
        bool newRecord = best == 0 || guesses < best;
        if (newRecord) Preferences.Default.Set(BestKey, guesses);

        string rankTitle = RankHelper.GetRankTitle(guesses);

        await SlotsRow.ScaleTo(1.25, 200, Easing.CubicOut);
        await SlotsRow.ScaleTo(1.0, 200, Easing.BounceOut);

        await DisplayAlert("You cracked it!",
            $"Code: {_game.SecretText}\nGuesses: {guesses}\nTime: {_elapsed:mm\\:ss}\nRank: {rankTitle}" +
            (newRecord ? "\nNew personal best!" : ""),
            "Nice!");

        string name = await DisplayPromptAsync("Save your score",
            "Enter your name for the leaderboard:", "Save", "Skip",
            maxLength: 16, initialValue: "Player");

        bool isTopOfLeaderboard = false;

        if (!string.IsNullOrWhiteSpace(name))
        {
            var entry = new LeaderboardEntry
            {
                PlayerName = name.Trim(),
                Guesses = guesses,
                TimeSeconds = timeSeconds,
                Rank = rankTitle
            };

            _leaderboard.AddEntry(entry);

            var top = _leaderboard.GetTopEntries();
            isTopOfLeaderboard = top.Count > 0
                && top[0].PlayerName == entry.PlayerName
                && top[0].AchievedOn == entry.AchievedOn;
        }

        var newAchievements = _achievements.CheckForNewUnlocks(guesses, timeSeconds, isTopOfLeaderboard);

        foreach (var achievement in newAchievements)
        {
            await DisplayAlert($"{achievement.Icon} Achievement unlocked!",
                $"{achievement.Title}\n{achievement.Description}", "Nice!");
        }

        bool earnedHint = _progression.CheckAndAwardHint(guesses);
        if (earnedHint)
        {
            RefreshHintButton();
            await DisplayAlert("?? Hint earned!",
                $"You won in {guesses} guesses on {_progression.CurrentDifficulty} difficulty - here's a hint for your next game!",
                "Nice!");
        }

        StartNewGame();
    }

    private void OnNewGameClicked(object? sender, EventArgs e) => StartNewGame();

    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        bool sure = await DisplayAlert("Give up?", "Reveal the code and end this game?", "Yes", "No");
        if (!sure) return;

        _timer.Stop();
        await DisplayAlert("The code was", _game.SecretText, "OK");
        StartNewGame();
    }

    private void OnSoundToggled(object? sender, ToggledEventArgs e)
    {
        _audio.SetMusicOn(e.Value);
    }

    private async void OnLeaderboardClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LeaderboardPage");
    }

    
    private async void OnAboutClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AboutPage");
    }

    // ---------- Difficulty selection ----------
    private void OnEasyClicked(object? sender, EventArgs e) => SetDifficulty(Difficulty.Easy);
    private void OnNormalClicked(object? sender, EventArgs e) => SetDifficulty(Difficulty.Normal);
    private void OnHardClicked(object? sender, EventArgs e) => SetDifficulty(Difficulty.Hard);

    private void SetDifficulty(Difficulty difficulty)
    {
        _progression.CurrentDifficulty = difficulty;
        RefreshDifficultyButtons();
    }

    private void RefreshDifficultyButtons()
    {
        var current = _progression.CurrentDifficulty;

        void Style(Button button, bool selected, Color accent)
        {
            button.BackgroundColor = selected ? accent : Colors.Transparent;
            button.TextColor = selected ? Color.FromArgb("#1B1464") : accent;
            button.BorderColor = accent;
        }

        Style(EasyButton, current == Difficulty.Easy, Color.FromArgb("#2EE6C5"));
        Style(NormalButton, current == Difficulty.Normal, Color.FromArgb("#FFD23F"));
        Style(HardButton, current == Difficulty.Hard, Color.FromArgb("#FF5D73"));
    }

    // ---------- Hints ----------
    private void RefreshHintButton()
    {
        HintButton.Text = $"?? Hint ({_progression.HintBalance})";
    }

    private async void OnHintClicked(object? sender, EventArgs e)
    {
        if (!_progression.UseHint())
        {
            await DisplayAlert("No hints available",
                "Win a game within your difficulty's guess range to earn a hint!", "OK");
            return;
        }

        RefreshHintButton();

        HintResult hint = _game.RevealHint();
        await DisplayAlert("?? Hint",
            $"The {hint.PositionName} digit of the code is {hint.Digit}.", "Thanks!");
    }

    private async Task ShakeSlotsAsync()
    {
        foreach (double x in new double[] { 12, -12, 8, -8, 0 })
            await SlotsRow.TranslateTo(x, 0, 45);
    }

    private static void Buzz(bool isLong = false)
    {
        try
        {
            HapticFeedback.Default.Perform(isLong ? HapticFeedbackType.LongPress : HapticFeedbackType.Click);
        }
        catch (Exception) { }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _audio.PauseMusic();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _audio.ResumeMusicIfOn();
    }
}