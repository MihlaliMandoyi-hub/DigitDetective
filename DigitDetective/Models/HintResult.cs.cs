// Group X | Surname, FirstName, StudentNumber | Surname, FirstName, StudentNumber | ...
namespace DigitDetective;

public class HintResult
{
    public int Position { get; set; }
    public int Digit { get; set; }

    public string PositionName => Position switch
    {
        0 => "first",
        1 => "second",
        2 => "third",
        _ => "unknown"
    };
}