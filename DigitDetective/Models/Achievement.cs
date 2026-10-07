// Group X | Surname, FirstName, StudentNumber | Surname, FirstName, StudentNumber | ...
namespace DigitDetective;

public class AchievementDefinition
{
    public string Id { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

public class AchievementStatus
{
    public AchievementDefinition Definition { get; set; } = new();
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedOn { get; set; }

    public string Icon => Definition.Icon;
    public string Title => Definition.Title;
    public string Description => Definition.Description;
    public string StatusText => IsUnlocked
        ? $"Unlocked {UnlockedOn:dd MMM yyyy}"
        : "Locked - keep playing!";

    // Used in the UI to dim locked achievements, without needing a converter
    public double CardOpacity => IsUnlocked ? 1.0 : 0.45;
}