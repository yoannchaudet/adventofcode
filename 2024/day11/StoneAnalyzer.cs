namespace day11;

public class StoneAnalyzer
{
    public List<long> Stones { get; }

    public StoneAnalyzer(string input)
    {
        Stones = File.ReadLines(input).FirstOrDefault("").Split(" ").Select(long.Parse).ToList();
    }
    
    public long GetStonesAfterBlinks(int blinkCount)
    {
        long stones = 0;
        foreach (var stone in Stones)
        {
            stones += GetStonesCountAfterBlinks(stone, blinkCount);
        }
        return stones;
    }
    
    // For part 2, let's get recursive
    public long GetStonesCountAfterBlinks(long stone, int blinkCount)
    {
        // Exit
        if (blinkCount == 0)
        {
            return 1;
        }
        
        if (stone == 0 )
        {
            return GetStonesCountAfterBlinks(1, blinkCount - 1);
        }
        
        var digits = Digits(stone);
        if (digits % 2 == 0)
        {
            var (left, right) = Split(stone, digits);
            return GetStonesCountAfterBlinks(left, blinkCount - 1) + GetStonesCountAfterBlinks(right, blinkCount - 1);
        }

        return GetStonesCountAfterBlinks(stone * 2024, blinkCount - 1);
    }
    
    public static long Digits(long number)
    {
        return (long) Math.Floor( Math.Log10(number)) + 1;
    }

    public static (long, long) Split(long number, long digits)
    {
        var scale = digits / 2;
        var left = (long) (number /  Math.Pow(10, scale));
        var right = (long) (number % Math.Pow(10, scale));
        return (left, right);
    }
}