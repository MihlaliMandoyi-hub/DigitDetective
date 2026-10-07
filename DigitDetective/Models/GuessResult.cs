// Group X | Surname, FirstName, StudentNumber | Surname, FirstName, StudentNumber | ...
namespace DigitDetective;

public enum DotStatus { Hit, Present, Absent }

public class GuessResult
{
    public int GuessNumber { get; set; }
    public string Digits { get; set; } = "";
    public int Hits { get; set; }
    public int Matches { get; set; }
    public DotStatus[] Dots { get; set; } = new DotStatus[3];

    public string Summary => $"{Hits} hit(s), {Matches} match(es)";
    public bool IsWin => Hits == 3;

    // One colour per position, used directly by the dots in the UI
    public Color Dot0Color => GetDotColor(Dots[0]);
    public Color Dot1Color => GetDotColor(Dots[1]);
    public Color Dot2Color => GetDotColor(Dots[2]);

    private static Color GetDotColor(DotStatus status) => status switch
    {
        DotStatus.Hit => Color.FromArgb("#2EE6C5"),      // green/mint
        DotStatus.Present => Color.FromArgb("#FF5D73"),  // pink/coral
        _ => Color.FromArgb("#B9B4D6")                   // grey
    };
}