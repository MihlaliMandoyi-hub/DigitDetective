namespace DigitDetective;

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = "Player";
    public int Guesses { get; set; }
    public int TimeSeconds { get; set; }
    public string Rank { get; set; } = "";
    public DateTime AchievedOn { get; set; } = DateTime.Now;

    public string TimeDisplay => TimeSpan.FromSeconds(TimeSeconds).ToString(@"mm\:ss");
    public string DateDisplay => AchievedOn.ToString("dd MMM, HH:mm");
}