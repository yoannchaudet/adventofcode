var input = "i_puzzle.txt";
var grid = File.ReadLines(input).Select(line => line.ToArray()).ToArray();

var part1 = 0;
for (var y = 0; y < grid.Length; y++)
for (var x = 0; x < grid[y].Length; x++)
{
    var collected = new List<string>
    {
        Collect((y, x), Direction.Right, 4),
        Collect((y, x), Direction.Bottom, 4),
        Collect((y, x), Direction.BottomLeft, 4),
        Collect((y, x), Direction.BottomRight, 4)
    };
    part1 += collected.Count(c => c is "XMAS" or "SAMX");
}

Console.WriteLine($"Part 1 = {part1}");

var part2 = 0;
for (var y = 0; y < grid.Length; y++)
for (var x = 0; x < grid[y].Length; x++)
    if (
        ((Collect((y, x), Direction.TopLeft, 2) == "AM" && Collect((y, x), Direction.BottomRight, 2) == "AS") ||
         (Collect((y, x), Direction.TopLeft, 2) == "AS" && Collect((y, x), Direction.BottomRight, 2) == "AM")) &&
        ((Collect((y, x), Direction.TopRight, 2) == "AM" && Collect((y, x), Direction.BottomLeft, 2) == "AS") ||
         (Collect((y, x), Direction.TopRight, 2) == "AS" && Collect((y, x), Direction.BottomLeft, 2) == "AM")))
        part2++;

Console.WriteLine($"Part 2 = {part2}");


string Collect((int, int) position, Direction direction, int length)
{
    var y = position.Item1;
    var x = position.Item2;

    var result = "";
    for (var i = 0; i < length; i++)
    {
        result += grid[y][x];

        switch (direction)
        {
            case Direction.Bottom:
                y++;
                break;
            case Direction.Right:
                x++;
                break;
            case Direction.BottomLeft:
                y++;
                x--;
                break;
            case Direction.BottomRight:
                y++;
                x++;
                break;
            case Direction.Top:
                y--;
                break;
            case Direction.TopLeft:
                y--;
                x--;
                break;
            case Direction.TopRight:
                y--;
                x++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        if (y < 0 || y >= grid.Length || x < 0 || x >= grid[y].Length) break;
    }

    return result;
}

internal enum Direction
{
    Bottom,
    Right,
    BottomLeft,
    BottomRight,
    Top,
    TopLeft,
    TopRight
}