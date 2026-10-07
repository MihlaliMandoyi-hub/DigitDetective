namespace DigitDetective;

/// <summary>Turns a guess count into a rank title, based on how efficiently the player won.</summary>
public static class RankHelper
{
    public static string GetRankTitle(int guesses)
    {
        if (guesses <= 1) return "Champion";
        if (guesses == 2) return "Master";
        if (guesses == 3) return "Expert";
        if (guesses <= 6) return "Amateur";
        return "Rookie";
    }
}