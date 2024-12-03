// Parse input

const string input = "inputs/input.txt";

// Part 1
var histories = GetHistories();
var part1 = 0;
foreach (var h in histories) part1 += GetNextExtrapolation(DevelopHistory(h));
Console.WriteLine($"Part 1 = {part1}");

// Part 2 (it's like part 1 if you reverse everything!)
var reversedHistories = GetHistories();
foreach (var history in reversedHistories)
    history.Reverse();
var part2 = 0;
foreach (var h in reversedHistories) part2 += GetNextExtrapolation(DevelopHistory(h));
Console.WriteLine($"Part 2 = {part2}");

// Print a developed history
void Print(List<List<int>> developedHistory)
{
    foreach (var h in developedHistory) Console.WriteLine(string.Join(" ", h.Select(i => i.ToString())));

    Console.WriteLine();
}

// Develop an history line
List<List<int>> DevelopHistory(List<int> history)
{
    var developedHistory = new List<List<int>> { history };
    var currentHistory = history;
    while (currentHistory.Any(i => i != 0))
    {
        var nextHistory = new int[currentHistory.Count - 1];
        for (var i = 0; i < currentHistory.Count - 1; i++) nextHistory[i] = currentHistory[i + 1] - currentHistory[i];

        developedHistory.Add(nextHistory.ToList());
        currentHistory = developedHistory.Last();
    }

    return developedHistory;
}

// Get next extrapolated value
int GetNextExtrapolation(List<List<int>> developedHistory)
{
    for (var i = 0; i < developedHistory.Count; i++)
    {
        var extrapolatedValue = i == 0
            ? 0
            : developedHistory[developedHistory.Count - i].Last() +
              developedHistory[developedHistory.Count - 1 - i].Last();
        developedHistory[developedHistory.Count - 1 - i].Add(extrapolatedValue);
    }

    return developedHistory[0].Last();
}

List<List<int>> GetHistories()
{
    return File.ReadAllLines(input).Select(line => line.Split(" ").Select(int.Parse).ToList()).ToList();
}