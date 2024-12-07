namespace day07;

public class Equation
{
    public Equation(string line)
    {
        var splits = line.Split(":");
        Total = long.Parse(splits[0].Trim());
        Numerands = splits[1].Trim().Split(" ").Select(long.Parse).ToList();
    }

    public long Total { get; }
    private List<long> Numerands { get; }

    public override string ToString()
    {
        return $"{Total} : {string.Join(" ", Numerands)}";
    }

    public bool Resolves(bool withConcatenation = false)
    {
        return Resolves(Numerands[0], 1, withConcatenation);
    }

    private bool Resolves(long currentTotal, int currentIndex, bool withContatenation = false)
    {
        // Exit condition
        if (currentIndex == Numerands.Count) return currentTotal == Total;

        // Small optimization
        if (currentTotal > Total) return false;

        // Addition and multiplication operators
        var resolves = Resolves(currentTotal + Numerands[currentIndex], currentIndex + 1, withContatenation) ||
                       Resolves(currentTotal * Numerands[currentIndex], currentIndex + 1, withContatenation);

        // Add concatenation operator
        if (withContatenation)
            resolves |= Resolves(long.Parse($"{currentTotal}{Numerands[currentIndex]}"), currentIndex + 1,
                withContatenation);

        return resolves;
    }
}