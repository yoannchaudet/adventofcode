using System.Text.RegularExpressions;

const string input = "inputs/input.txt";
var cards = File.ReadLines(input).Select(line => new Card(line)).ToList();

// Part 1
Console.WriteLine($"Part 1 = {cards.Sum(card => GetPart1Score(card.Wins))}");

// Part 2
var totalCards = 0;
for (var i = 0; i < cards.Count; i++)
{
    if (cards[i].Wins > 0)
        for (var j = i + 1; j <= i + cards[i].Wins; j++)
            if (j < cards.Count)
                cards[j].Copies += cards[i].Copies;
    totalCards += cards[i].Copies;
}

Console.WriteLine($"Part 2 = {totalCards}");

int GetPart1Score(int wins)
{
    var score = 0;
    for (var win = 1; win <= wins; win++)
        if (score == 0)
            score = 1;
        else
            score *= 2;
    return score;
}

internal class Card
{
    public Card(string line)
    {
        var pattern = new Regex(@"Card\s+([0-9]+)\: (.+)");
        var match = pattern.Match(line);
        if (!match.Success) throw new Exception($"Unable to parse line: {line}");

        var winningNumbers = match.Groups[2].Value.Split(" | ")[0].Split(" ").Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(int.Parse).ToList();
        YourNumbers = match.Groups[2].Value.Split(" | ")[1].Split(" ").Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(int.Parse).ToList();
        Wins = YourNumbers.Count(i => winningNumbers.Contains(i));
        Copies = 1;
    }

    private List<int> YourNumbers { get; }
    public int Wins { get; }
    public int Copies { get; set; }
}