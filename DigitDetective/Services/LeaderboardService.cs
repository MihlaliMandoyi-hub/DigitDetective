using System.Text.Json;

namespace DigitDetective;

public class LeaderboardService
{
    private const string Key = "LeaderboardEntries";
    private const int MaxEntries = 10;

    public List<LeaderboardEntry> GetTopEntries()
    {
        string json = Preferences.Default.Get(Key, string.Empty);
        if (string.IsNullOrWhiteSpace(json))
            return new List<LeaderboardEntry>();

        try
        {
            var entries = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? new();
            return entries
                .OrderBy(e => e.Guesses)
                .ThenBy(e => e.TimeSeconds)
                .ToList();
        }
        catch (Exception)
        {
            return new List<LeaderboardEntry>();
        }
    }

    public void AddEntry(LeaderboardEntry entry)
    {
        var entries = GetTopEntries();
        entries.Add(entry);

        var trimmed = entries
            .OrderBy(e => e.Guesses)
            .ThenBy(e => e.TimeSeconds)
            .Take(MaxEntries)
            .ToList();

        string json = JsonSerializer.Serialize(trimmed);
        Preferences.Default.Set(Key, json);
    }

    public void ClearAll() => Preferences.Default.Remove(Key);
}