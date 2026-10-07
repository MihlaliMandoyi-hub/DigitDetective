namespace DigitDetective;

public partial class AchievementsPage : ContentPage
{
    private readonly AchievementService _achievements;

    public AchievementsPage(AchievementService achievements)
    {
        InitializeComponent();
        _achievements = achievements;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var all = _achievements.GetAllWithStatus();
        AchievementsList.ItemsSource = all;

        int unlockedCount = all.Count(a => a.IsUnlocked);
        ProgressLabel.Text = $"{unlockedCount} of {all.Count} unlocked";
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}