namespace DigitDetective;

public class GameEngine
{
    private int[] _secret = new int[3];
    private readonly Random _rng = new();
    private readonly HashSet<int> _hintedPositions = new();

    public int GuessCount { get; private set; }
    public string SecretText => string.Join(" ", _secret);

    public void NewGame()
    {
        int first = _rng.Next(1, 10);
        var rest = Enumerable.Range(0, 10)
                             .Where(d => d != first)
                             .OrderBy(_ => _rng.Next())
                             .Take(2);
        _secret = new[] { first }.Concat(rest).ToArray();
        GuessCount = 0;
        _hintedPositions.Clear();
    }

    public GuessResult Evaluate(IList<int> guess)
    {
        GuessCount++;
        int hits = 0, matches = 0;
        var dots = new DotStatus[3];

        for (int i = 0; i < 3; i++)
        {
            if (guess[i] == _secret[i])
            {
                hits++;
                dots[i] = DotStatus.Hit;
            }
            else if (_secret.Contains(guess[i]))
            {
                matches++;
                dots[i] = DotStatus.Present;
            }
            else
            {
                dots[i] = DotStatus.Absent;
            }
        }

        return new GuessResult
        {
            GuessNumber = GuessCount,
            Digits = string.Join("  ", guess),
            Hits = hits,
            Matches = matches,
            Dots = dots
        };
    }

    public HintResult RevealHint()
    {
        var available = Enumerable.Range(0, 3).Where(i => !_hintedPositions.Contains(i)).ToList();

        if (available.Count == 0)
            available = Enumerable.Range(0, 3).ToList();

        int position = available[_rng.Next(available.Count)];
        _hintedPositions.Add(position);

        return new HintResult { Position = position, Digit = _secret[position] };
    }
}