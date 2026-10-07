using System.Text.Json;

namespace DigitDetective;

/// <summary>Defines, checks, unlocks and persists achievements.</summary>
public class AchievementService
{
    private const string UnlockedKey = "UnlockedAchievements";
    private const string TotalWinsKey = "TotalWins";

    public static readonly List<AchievementDefinition> Catalog = new()
    {
        new AchievementDefinition { Id = "first_guess_champ", Icon = "🎯", Title = "First Guess Champion", Description = "Crack the code on your very first guess." },
        new AchievementDefinition { Id = "quick_thinker", Icon = "⚡", Title = "Quick Thinker", Description = "Win the game in 3 guesses or fewer." },
        new AchievementDefinition { Id = "speed_demon", Icon = "⏱️", Title = "Speed Demon", Description = "Win the game in under 30 seconds." },
        new AchievementDefinition { Id = "marathon_mind", Icon = "🧠", Title = "Marathon Mind", Description = "Win after 15 guesses or more. Persistence pays off!" },
        new AchievementDefinition { Id = "dedicated_detective", Icon = "🕵️", Title = "Dedicated Detective", Description = "Win 10 games in total." },
        new AchievementDefinition { Id = "leaderboard_legend", Icon = "👑", Title = "Leaderboard Legend", Description = "Reach #1 on the leaderboard." },
    };

    private Dictionary<string, DateTime> GetUnlockedDict()
    {
        string json = Preferences.Default.Get(UnlockedKey, string.Empty);
        if (string.IsNullOrWhiteSpace(json)) return new Dictionary<string, DateTime>();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, DateTime>>(json) ?? new();
        }
        catch (Exception)
        {
            return new Dictionary<string, DateTime>();
        }
    }

    private void SaveUnlockedDict(Dictionary<string, DateTime> dict)
    {
        string json = JsonSerializer.Serialize(dict);
        Preferences.Default.Set(UnlockedKey, json);
    }

    public int GetTotalWins() => Preferences.Default.Get(TotalWinsKey, 0);

    /// <summary>Call once after every win. Returns any achievements newly unlocked by this win.</summary>
    public List<AchievementDefinition> CheckForNewUnlocks(int guesses, int timeSeconds, bool isTopOfLeaderboard)
    {
        int totalWins = GetTotalWins() + 1;
        Preferences.Default.Set(TotalWinsKey, totalWins);

        var unlocked = GetUnlockedDict();
        var newlyUnlocked = new List<AchievementDefinition>();

        void TryUnlock(string id)
        {
            if (unlocked.ContainsKey(id)) return;

            unlocked[id] = DateTime.Now;
            newlyUnlocked.Add(Catalog.First(a => a.Id == id));
        }

        if (guesses == 1) TryUnlock("first_guess_champ");
        if (guesses <= 3) TryUnlock("quick_thinker");
        if (timeSeconds < 30) TryUnlock("speed_demon");
        if (guesses >= 15) TryUnlock("marathon_mind");
        if (totalWins >= 10) TryUnlock("dedicated_detective");
        if (isTopOfLeaderboard) TryUnlock("leaderboard_legend");

        SaveUnlockedDict(unlocked);
        return newlyUnlocked;
    }

    public List<AchievementStatus> GetAllWithStatus()
    {
        var unlocked = GetUnlockedDict();

        return Catalog.Select(def => new AchievementStatus
        {
            Definition = def,
            IsUnlocked = unlocked.ContainsKey(def.Id),
            UnlockedOn = unlocked.TryGetValue(def.Id, out var date) ? date : null
        }).ToList();
    }
}