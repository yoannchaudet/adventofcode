var input = "i_puzzle.txt";
var reports = File.ReadLines(input).Select(line => line.Split(" ").Select(int.Parse).ToArray()).ToList();

var part1 = reports.Count(r => IsSafe(r) == -1);
Console.WriteLine($"Part 1: {part1}");
var part2 = reports.Count(IsSafePart2);
Console.WriteLine($"Part 2: {part2}");

int IsSafe(int[] levels)
{
    // assume more than 1 level
    var increasing = levels[0] < levels[1];
    for (var i = 1; i < levels.Length; i++)
    {
        // Check direction
        if (increasing && levels[i - 1] > levels[i]) return i;
        if (!increasing && levels[i - 1] < levels[i]) return i;

        // Check distance
        var dist = Math.Abs(levels[i - 1] - levels[i]);
        if (dist is > 3 or < 1)
            return i;
    }

    return -1;
}

bool IsSafePart2(int[] levels)
{
    var isSafe = IsSafe(levels);
    if (isSafe == -1)
        return true;

    // Check again but removing 1 level each time
    for (var i = 0; i < levels.Length; i++)
    {
        var newLevel = levels
            .Where((value, index) => index != i)
            .ToArray();
        if (IsSafe(newLevel) == -1) return true;
    }

    return false;
}