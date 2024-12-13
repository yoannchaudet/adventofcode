namespace day11;

public class StoneAnalyzer
{
    private readonly Dictionary<(long, int), long> _cache = new();

    public StoneAnalyzer(string input)
    {
        Stones = File.ReadLines(input).FirstOrDefault("").Split(" ").Select(long.Parse).ToList();
    }

    private List<long> Stones { get; }

    public long GetStonesAfterBlinks(int blinkCount)
    {
        _cache.Clear();
        long stones = 0;
        foreach (var stone in Stones) stones += GetStonesCountAfterBlinks(stone, blinkCount);
        return stones;
    }

    // Rewritten recursive for part 2, but with a cache to speed up the resolution
    public long GetStonesCountAfterBlinks(long stone, int blinkCount)
    {
        // Exit
        if (blinkCount == 0) return 1;

        // Check cache
        var key = (stone, blinkCount);
        if (_cache.TryGetValue(key, out var blinks))
            return blinks;

        if (stone == 0)
        {
            var next = GetStonesCountAfterBlinks(1, blinkCount - 1);
            _cache.Add(key, next);
            return next;
        }

        var digits = Digits(stone);
        if (digits % 2 == 0)
        {
            var (left, right) = Split(stone, digits);
            var next = GetStonesCountAfterBlinks(left, blinkCount - 1) +
                       GetStonesCountAfterBlinks(right, blinkCount - 1);
            _cache.Add(key, next);
            return next;
        }

        {
            var next = GetStonesCountAfterBlinks(stone * 2024, blinkCount - 1);
            _cache.Add(key, next);
            return next;
        }
    }

    private static long Digits(long number)
    {
        return (long)Math.Floor(Math.Log10(number)) + 1;
    }

    private static (long, long) Split(long number, long digits)
    {
        var scale = digits / 2;
        var left = (long)(number / Math.Pow(10, scale));
        var right = (long)(number % Math.Pow(10, scale));
        return (left, right);
    }
}