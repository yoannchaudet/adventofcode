using System.Text.RegularExpressions;

var input = "i_puzzle.txt";
var inputText = File.ReadAllText(input);

var part1Regex = new Regex(@"mul\(([0-9]+)\,([0-9]+)\)");
var part1 = part1Regex.Matches(inputText).Select(match =>
    int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value)
).Sum();
Console.WriteLine($"Part 1 = {part1}");

var part2Regex = new Regex(@"mul\(([0-9]+)\,([0-9]+)\)|do\(\)|don\'t\(\)");
var canMul = true;
var part2 = 0;
foreach (var match in part2Regex.Matches(inputText))
    if (match is Match m)
    {
        switch (m.Groups[0].Value)
        {
            case "do()":
                canMul = true;
                break;
            case "don't()":
                canMul = false;
                break;
            default:
            {
                var a = int.Parse(m.Groups[1].Value);
                var b = int.Parse(m.Groups[2].Value);
                if (canMul) part2 += a * b;
                break;
            }
        }
    }
Console.WriteLine($"Part 2 = {part2}");