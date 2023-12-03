using System.Text.RegularExpressions;

const string input = "inputs/input.txt";
var schematic = File.ReadLines(input).ToArray();
var parts = schematic.SelectMany((line, y) =>
{
    var pattern = new Regex(@"([0-9]+)([^0-9]*|$)");
    return pattern.Matches(line).Select(match => new Part(y, match.Index, match.Groups[1].Value, schematic));
}).ToList();

var part1 = parts.Where(part => part.Symbol.HasValue).Sum(part => part.Number);
Console.WriteLine($"Part 1: {part1}");

var part2 = parts.Where(part => part.Symbol.HasValue && part.Symbol.Value == '*').GroupBy(part => part.SymbolPosition)
    .Select(
        group =>
        {
            if (group.Count() == 2) return group.First().Number * group.Last().Number;
            return 0;
        }).Sum();
Console.WriteLine($"Part 2: {part2}");

internal class Part
{
    public Part(int y, int x, string number, string[] schematic)
    {
        Y = y;
        X = x;
        Number = int.Parse(number);
        Length = number.Length;
        FindSymbol(schematic);
    }

    public int Number { init; get; }
    private int Y { init; get; }
    private int X { init; get; }
    private int Length { init; get; }
    public char? Symbol { get; private set; }
    public (int, int)? SymbolPosition { get; private set; }

    private void FindSymbol(string[] schematic)
    {
        for (var yy = Y - 1; yy <= Y + 1; yy++)
        for (var xx = X - 1; xx <= X + Length; xx++)
        {
            var symbol = GetSymbol(yy, xx, schematic);
            if (symbol.HasValue && !(symbol.Value is >= '0' and <= '9') && symbol.Value != '.')
            {
                Symbol = symbol;
                SymbolPosition = (yy, xx);
                return;
            }
        }
    }

    private static char? GetSymbol(int y, int x, string[] schematic)
    {
        if (y >= 0 && y < schematic.Length && x >= 0 && x < schematic[y].Length) return schematic[y][x];
        return null;
    }
}