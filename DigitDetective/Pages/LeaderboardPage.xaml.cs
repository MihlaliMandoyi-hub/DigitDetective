namespace DigitDetective;

public partial class LeaderboardPage : ContentPage
{
    private readonly LeaderboardService _leaderboard;
    private readonly AchievementService _achievements;

    public LeaderboardPage(LeaderboardService leaderboard, AchievementService achievements)
    {
        InitializeComponent();
        _leaderboard = leaderboard;
        _achievements = achievements;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadEntries();
        LoadAchievements();
    }

    private void LoadEntries()
    {
        var entries = _leaderboard.GetTopEntries();
        var ranked = entries.Select((e, i) => new RankedEntry(e, i + 1)).ToList();
        EntriesList.ItemsSource = ranked;
    }

    private void LoadAchievements()
    {
        var all = _achievements.GetAllWithStatus();
        AchievementsStrip.ItemsSource = all;

        int unlockedCount = all.Count(a => a.IsUnlocked);
        AchievementsProgressLabel.Text = $"Achievements - {unlockedCount} of {all.Count}";
    }

    private async void OnClearClicked(object? sender, EventArgs e)
    {
        bool sure = await DisplayAlert("Clear leaderboard?",
            "This removes all saved scores. This cannot be undone.", "Yes", "No");
        if (!sure) return;

        _leaderboard.ClearAll();
        LoadEntries();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}

public class RankedEntry
{
    public RankedEntry(LeaderboardEntry entry, int rank)
    {
        PlayerName = entry.PlayerName;
        Guesses = entry.Guesses;
        TimeDisplay = entry.TimeDisplay;
        DateDisplay = entry.DateDisplay;
        RankDisplay = $"#{rank}";
        PlayerRank = entry.Rank;
    }

    public string PlayerName { get; }
    public int Guesses { get; }
    public string TimeDisplay { get; }
    public string DateDisplay { get; }
    public string RankDisplay { get; }
    public string PlayerRank { get; }
}