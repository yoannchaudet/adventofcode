const string input = "inputs/input.txt";
Console.WriteLine($"Part 1: {ParseInput1(input).Sum()}");
Console.WriteLine($"Part 2: {ParseInput2(input).Sum()}");
return;

IEnumerable<int> ParseInput1(string inputFile)
{
    return File.ReadLines(inputFile).Select(line =>
    {
        var first = line.First(c => c is >= '1' and <= '9');
        var last = line.Last(c => c is >= '1' and <= '9');
        return int.Parse(first.ToString()) * 10 + int.Parse(last.ToString());
    });
}

IEnumerable<int> ParseInput2(string inputFile)
{
    var keys = new Dictionary<string, int>
    {
        { "0", 0 },
        { "1", 1 },
        { "2", 2 },
        { "3", 3 },
        { "4", 4 },
        { "5", 5 },
        { "6", 6 },
        { "7", 7 },
        { "8", 8 },
        { "9", 9 },
        { "zero", 0 },
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
        { "eight", 8 },
        { "nine", 9 }
    };

    return File.ReadLines(inputFile).Select(line =>
    {
        var firstKey = keys.Keys.Select(key => (key, line.IndexOf(key, StringComparison.Ordinal)))
            .Where(t => t.Item2 != -1).MinBy(t => t.Item2);
        var lastKey = keys.Keys.Select(key => (key, line.LastIndexOf(key, StringComparison.Ordinal)))
            .Where(t => t.Item2 != -1)
            .MaxBy(t => t.Item2);
        return keys[firstKey.key] * 10 + keys[lastKey.key];
    });
}