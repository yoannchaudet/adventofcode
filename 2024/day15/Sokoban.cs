namespace day15;

public class Sokoban
{
    public enum Direction
    {
        Top,
        Right,
        Bottom,
        Left
    }

    private enum Space
    {
        Void,
        SimpleBox,
        Robot,
        Wall,
        LargeBoxLeft,
        LargeBoxRight
    }

    private char[][] Map;
    private (int, int) RobotPosition;

    private Sokoban(string[] map, bool part2 = false)
    {
        var actualMap = map.Select(x => x.ToCharArray()).ToArray();
        if (!part2)
        {
            Map = actualMap;
        }
        else
        {
            // Resize the map
            Map = new char[actualMap.Length][];
            for (var y = 0; y < Map.Length; y++)
            {
                Map[y] = new char[actualMap[0].Length * 2];
                for (var x = 0; x < actualMap[y].Length; x++)
                    switch (GetSpace(actualMap[y][x]))
                    {
                        case Space.Robot:
                            Map[y][2 * x] = '@';
                            Map[y][2 * x + 1] = '.';
                            break;
                        case Space.Void:
                            Map[y][2 * x] = '.';
                            Map[y][2 * x + 1] = '.';
                            break;
                        case Space.Wall:
                            Map[y][2 * x] = '#';
                            Map[y][2 * x + 1] = '#';
                            break;
                        case Space.SimpleBox:
                            Map[y][2 * x] = '[';
                            Map[y][2 * x + 1] = ']';
                            break;
                    }
            }
        }


        RobotPosition = FindRobotPosition();
    }

    // Move the robot in the given direction
    public void Move(Direction direction)
    {
        Move(RobotPosition, direction);
        RobotPosition = FindRobotPosition();
    }

    public int GetSumGps()
    {
        var gps = 0;
        for (var y = 0; y < Map.Length; y++)
        for (var x = 0; x < Map[y].Length; x++)
            if (GetSpace(Map[y][x]) == Space.SimpleBox || GetSpace(Map[y][x]) == Space.LargeBoxLeft)
                gps += 100 * y + x;
        return gps;
    }

    // Check if a move is possible (recursively)
    private bool CanMove((int, int) position, Direction direction)
    {
        var adjacentPosition = GetAdjacentPosition(position, direction);
        if (adjacentPosition == null)
            return false;

        var space = GetSpace(Map[adjacentPosition.Value.Item1][adjacentPosition.Value.Item2]);
        if (space == Space.Wall)
            return false;
        if (space == Space.Void)
            return true;
        if (space == Space.SimpleBox || ((space == Space.LargeBoxLeft || space == Space.LargeBoxRight) &&
                                         (direction == Direction.Left || direction == Direction.Right)))
            return CanMove(adjacentPosition.Value, direction);
        if (space == Space.LargeBoxLeft || space == Space.LargeBoxRight)
        {
            var dd = space == Space.LargeBoxLeft ? Direction.Right : Direction.Left;
            var otherAdjacentPosition = GetAdjacentPosition(
                (adjacentPosition.Value.Item1, adjacentPosition.Value.Item2),
                dd);
            return CanMove(adjacentPosition.Value, direction) && otherAdjacentPosition.HasValue &&
                   CanMove(otherAdjacentPosition.Value, direction);
        }

        throw new ArgumentException(
            $"Illegal move (position {position.Item1}, {position.Item2} in direction {direction}");
    }

    // Perform a move for what's at the given position in the given direction
    // Return false if no move can be completed
    private void Move((int, int) position, Direction direction)
    {
        if (!CanMove(position, direction))
            return;

        var adjacentPosition = GetAdjacentPosition(position, direction)!.Value;
        var space = GetSpace(Map[adjacentPosition.Item1][adjacentPosition.Item2]);
        if (space == Space.Void)
        {
            Map[adjacentPosition.Item1][adjacentPosition.Item2] = Map[position.Item1][position.Item2];
            Map[position.Item1][position.Item2] = '.';
        }
        else if (space == Space.SimpleBox || ((space == Space.LargeBoxLeft || space == Space.LargeBoxRight) &&
                                              (direction == Direction.Left || direction == Direction.Right)))
        {
            Move(adjacentPosition, direction);
            Map[adjacentPosition.Item1][adjacentPosition.Item2] = Map[position.Item1][position.Item2];
            Map[position.Item1][position.Item2] = '.';
        }
        else if (space == Space.LargeBoxLeft || space == Space.LargeBoxRight)
        {
            var dd = space == Space.LargeBoxLeft ? Direction.Right : Direction.Left;
            var otherAdjacentPosition = GetAdjacentPosition(
                (adjacentPosition.Item1, adjacentPosition.Item2),
                dd)!.Value;
            Move(otherAdjacentPosition, direction);
            Move(adjacentPosition, direction);
            Map[adjacentPosition.Item1][adjacentPosition.Item2] = Map[position.Item1][position.Item2];
            Map[position.Item1][position.Item2] = '.';
        }
    }

    public void PrintMap()
    {
        for (var y = 0; y < Map.Length; y++)
        {
            for (var x = 0; x < Map[y].Length; x++)
                Console.Write(Map[y][x]);
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    private (int, int) FindRobotPosition()
    {
        for (var y = 0; y < Map.Length; y++)
        for (var x = 0; x < Map[y].Length; x++)
            if (GetSpace(Map[y][x]) == Space.Robot)
                return (y, x);

        throw new ArgumentException("Unable to find robot initial position");
    }

    private Space GetSpace(char space)
    {
        switch (space)
        {
            case '.':
                return Space.Void;
            case '@':
                return Space.Robot;
            case '#':
                return Space.Wall;
            case 'O':
                return Space.SimpleBox;
            case '[':
                return Space.LargeBoxLeft;
            case ']':
                return Space.LargeBoxRight;
            default:
                throw new ArgumentException("Unknown space");
        }
    }

    // Return the adjacent position in the given direction
    private (int, int)? GetAdjacentPosition((int, int) position, Direction direction)
    {
        var (y, x) = position;
        switch (direction)
        {
            case Direction.Bottom:
                return y + 1 < Map.Length ? (y + 1, x) : null;
            case Direction.Top:
                return y - 1 >= 0 ? (y - 1, x) : null;
            case Direction.Left:
                return x - 1 >= 0 ? (y, x - 1) : null;
            case Direction.Right:
                return x + 1 < Map[y].Length ? (y, x + 1) : null;
        }

        throw new ArgumentException("Invalid direction");
    }

    private static Direction ParseDirection(char c)
    {
        switch (c)
        {
            case '>':
                return Direction.Right;
            case '<':
                return Direction.Left;
            case '^':
                return Direction.Top;
            case 'v':
                return Direction.Bottom;
            default:
                throw new ArgumentException("Unknown direction");
        }
    }

    public static (Sokoban, List<Direction>) ParseInput(string input, bool part2 = false)
    {
        var map = new List<string>();
        List<Direction> directions = new();
        Sokoban? sokoban = null;

        foreach (var line in File.ReadLines(input))
            // parse map
            if (sokoban == null)
            {
                if (string.IsNullOrEmpty(line))
                    sokoban = new Sokoban(map.ToArray(), part2);
                else
                    map.Add(line);
            }

            // parse directions
            else
            {
                directions.AddRange(line.Select(ParseDirection));
            }

        return (sokoban, directions)!;
    }
}