namespace DigitDetective;

public enum Difficulty { Easy, Normal, Hard }

/// <summary>Tracks the player's chosen difficulty and their earned hint balance.</summary>
public class ProgressionService
{
    private const string DifficultyKey = "Difficulty";
    private const string HintBalanceKey = "HintBalance";

    public Difficulty CurrentDifficulty
    {
        get
        {
            string saved = Preferences.Default.Get(DifficultyKey, nameof(Difficulty.Normal));
            return Enum.TryParse<Difficulty>(saved, out var d) ? d : Difficulty.Normal;
        }
        set => Preferences.Default.Set(DifficultyKey, value.ToString());
    }

    public int HintBalance
    {
        get => Preferences.Default.Get(HintBalanceKey, 0);
        private set => Preferences.Default.Set(HintBalanceKey, value);
    }

    public bool UseHint()
    {
        if (HintBalance <= 0) return false;
        HintBalance -= 1;
        return true;
    }

    /// <summary>Call after a win. Awards a hint if the guess count matches the current difficulty's range.</summary>
    public bool CheckAndAwardHint(int guesses)
    {
        bool earned = CurrentDifficulty switch
        {
            Difficulty.Hard => guesses <= 3,
            Difficulty.Normal => guesses is >= 4 and <= 5,
            Difficulty.Easy => guesses is >= 6 and <= 7,
            _ => false
        };

        if (earned)
            HintBalance += 1;

        return earned;
    }
}