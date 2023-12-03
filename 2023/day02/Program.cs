using System.Text.RegularExpressions;

const string input = "inputs/input.txt";
var games = File.ReadLines(input).Select(line =>
{
    var pattern = new Regex(@"Game ([0-9]+)\:\s(.+)");
    var match = pattern.Match(line);
    if (!match.Success) Console.WriteLine($"Unable to parse line: {line}");
    return new Game(int.Parse(match.Groups[1].Value), match.Groups[2].Value);
}).ToList();
var configuration = new Dictionary<string, int> { { "red", 12 }, { "green", 13 }, { "blue", 14 } };
var part1 = games.Where(game => !game.Impossible(configuration)).Select(game => game.Id).Sum();
Console.WriteLine($"Part 1 = {part1}");
var part2 = games.Select(game => game.RedGreenBlueMinConfiguration())
    .Select(tuple => tuple.Item1 * tuple.Item2 * tuple.Item3).Sum();
Console.WriteLine($"Part 2 = {part2}");

internal class Game
{
    public Game(int id, string input)
    {
        Id = id;
        DieSet = input.Split(";").Select(set => new DieSet(set)).ToList();
    }

    public int Id { init; get; }
    private IEnumerable<DieSet> DieSet { init; get; }

    public bool Impossible(IDictionary<string, int> configuration)
    {
        return DieSet.Any(set =>
        {
            return set.Dies.Any(die => configuration.ContainsKey(die.Key) && die.Value > configuration[die.Key]);
        });
    }

    public (int, int, int) RedGreenBlueMinConfiguration()
    {
        var red = DieSet.Max(set =>
            set.Dies.Where(die => die.Key == "red").DefaultIfEmpty(new KeyValuePair<string, int>("", 0))
                .MaxBy(die => die.Value).Value);
        var green = DieSet.Max(set =>
            set.Dies.Where(die => die.Key == "green").DefaultIfEmpty(new KeyValuePair<string, int>("", 0))
                .MaxBy(die => die.Value).Value);
        var blue = DieSet.Max(set =>
            set.Dies.Where(die => die.Key == "blue").DefaultIfEmpty(new KeyValuePair<string, int>("", 0))
                .MaxBy(die => die.Value).Value);
        return (red, green, blue);
    }
}

internal class DieSet
{
    public DieSet(string input)
    {
        Dies = input.Split(",").Select(die => die.Trim().Split(" "))
            .ToDictionary(die => die[1], die => int.Parse(die[0].Trim()));
    }

    public IReadOnlyDictionary<string, int> Dies { init; get; }
}