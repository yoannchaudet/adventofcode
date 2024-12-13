using day06;

var input = "i_puzzle.txt";

//
// Part 1
//

// List of explored positions
var exploredPositions = new List<(int, int)>();

{
    var map = ReadMap(input);
    var (guardDirection, guardPosition) = FindGuard(map);
    map[guardPosition.Item1][guardPosition.Item2] = 'X';
    while (WithinBounds(map, guardPosition))
    {
        var nextPosition = NextPosition(guardPosition, guardDirection);
        if (!WithinBounds(map, nextPosition)) break;

        // Just rotate
        if (map[nextPosition.Item1][nextPosition.Item2] == '#')
        {
            guardDirection = RotateDirection(guardDirection);
        }

        // Move
        else
        {
            guardPosition = nextPosition;
            map[guardPosition.Item1][guardPosition.Item2] = 'X';
            if (!exploredPositions.Contains(guardPosition))
                exploredPositions.Add(guardPosition);
        }
    }

    var part1 = 0;
    foreach (var row in map) part1 += row.Count(c => c == 'X');
    Console.WriteLine($"Part 1 = {part1}");
}

//
// Part 2
// 
// Try to block all positions that were expored by a guard, one by one, except the guard's starting position.
var part2 = 0;
foreach (var position in exploredPositions)
{
    var map = ReadMap(input);
    var (guardDirection, guardPosition) = FindGuard(map);
    if (position == guardPosition)
        continue;
    var loopDetector = new LoopDetector();
    // Add obstruction
    map[position.Item1][position.Item2] = 'O';

    while (true)
    {
        var nextPosition = NextPosition(guardPosition, guardDirection);
        if (!WithinBounds(map, nextPosition)) break;

        // Just rotate
        if (map[nextPosition.Item1][nextPosition.Item2] == '#' || map[nextPosition.Item1][nextPosition.Item2] == 'O')
        {
            guardDirection = RotateDirection(guardDirection);
            if (loopDetector.IsLoop(nextPosition))
            {
                part2++;
                break;
            }
        }

        // Move
        else
        {
            guardPosition = nextPosition;
            map[guardPosition.Item1][guardPosition.Item2] = 'X';
        }
    }
}

Console.WriteLine($"Part2 = {part2}");


static (Direction, (int, int)) FindGuard(char[][] map)
{
    for (var y = 0; y < map.Length; y++)
    for (var x = 0; x < map[y].Length; x++)
    {
        if (map[y][x] == '^') return (Direction.Top, (y, x));
        if (map[y][x] == '>') return (Direction.Right, (y, x));
        if (map[y][x] == 'v') return (Direction.Bottom, (y, x));
        if (map[y][x] == '<') return (Direction.Left, (y, x));
    }

    throw new Exception("Cannot find guard");
}

static bool WithinBounds(char[][] map, (int, int) position)
{
    var (y, x) = position;
    return x >= 0 && x < map[0].Length && y >= 0 && y < map.Length;
}

static (int, int) NextPosition((int, int) position, Direction direction)
{
    switch (direction)
    {
        case Direction.Top:
            return (position.Item1 - 1, position.Item2);
        case Direction.Bottom:
            return (position.Item1 + 1, position.Item2);
        case Direction.Left:
            return (position.Item1, position.Item2 - 1);
        case Direction.Right:
            return (position.Item1, position.Item2 + 1);
    }

    throw new Exception("Illegal move");
}

// Rotate a direction 90degrees right
static Direction RotateDirection(Direction direction)
{
    switch (direction)
    {
        case Direction.Top:
            return Direction.Right;
        case Direction.Right:
            return Direction.Bottom;
        case Direction.Bottom:
            return Direction.Left;
        case Direction.Left:
            return Direction.Top;
    }

    throw new Exception("Illegal rotation");
}

// Print the map
static void PrintMap(char[][] map)
{
    Console.WriteLine();
    foreach (var row in map) Console.WriteLine(row);
    Console.WriteLine();
}

// Read the map
static char[][] ReadMap(string input)
{
    return File.ReadAllLines(input).Select(x => x.ToCharArray()).ToArray();
}

internal enum Direction
{
    Top,
    Right,
    Bottom,
    Left
}