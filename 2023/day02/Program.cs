using System.Text.RegularExpressions;

var input = "inputs/input.txt";
var games = File.ReadLines(input).Select(line =>
{
    var pattern = new Regex(@"Game ([0-9]+)\:\s(.+)");
    var match = pattern.Match(line);
    if (!match.Success) Console.WriteLine($"Unable to parse line: {line}");
    return new Game(int.Parse(match.Groups[1].Value), match.Groups[2].Value);
});
// 12 red cubes, 13 green cubes, and 14 blue cubes
var configuration = new Dictionary<string, int>() { { "red", 12 }, {"green", 13}, {"blue", 14}  };
var part1 = games.Where(game => !game.Impossible(configuration)).Select(game => game.Id).Sum();
Console.WriteLine($"Part 1 = {part1}");

internal class Game
{
    public int Id { init; get;  }
    public IEnumerable<DieSet> DieSet { init; get; }

    public Game(int id, string input)
    {
        Id = id;
        DieSet = input.Split(";").Select(set => new DieSet(set)).ToList();
    }

    public bool Impossible(IDictionary<string, int> configuration)
    {
        return DieSet.Any(set =>
        {
            return set.Dies.Any(die => configuration.ContainsKey(die.Key) && die.Value > configuration[die.Key]);
        });
    }
}

internal class DieSet
{
    public IReadOnlyDictionary<string, int> Dies { init; get; }

    public DieSet(string input)
    {
        Dies = input.Split(",").Select(die => die.Trim().Split(" ")).ToDictionary(die => die[1], die => int.Parse(die[0].Trim()));
    }
}