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

    public enum Space
    {
        Void,
        Box,
        Robot,
        Wall
    }

    public char[][] Map;
    public (int, int) RobotPosition;

    public Sokoban(string[] map)
    {
        Map = map.Select(x => x.ToCharArray()).ToArray();
        RobotPosition = FindRobotPosition();
    }

    // Move the robot in the given direction
    public bool Move(Direction direction)
    {
        var move = Move(RobotPosition, direction);
        RobotPosition = FindRobotPosition();
        return move;
    }

    public int GetSumGPS()
    {
        var gps = 0;
        for (var y = 0; y < Map.Length; y++)
        for (var x = 0; x < Map[y].Length; x++)
            if (GetSpace((y, x)) == Space.Box)
                gps += 100 * y + x;

        return gps;
    }

    // Perform a move for what's at the given position in the given direction
    // Return false if no move can be completed
    private bool Move((int, int) position, Direction direction)
    {
        var adjacentPosition = GetAdjacentPosition(position, direction);

        // Cannot move (out of bound)
        if (adjacentPosition == null)
            return false;

        switch (GetSpace(adjacentPosition.Value))
        {
            case Space.Wall:
                // Cannot move into a wall
                return false;
            case Space.Void:
                // Move into a void!
                Map[adjacentPosition.Value.Item1][adjacentPosition.Value.Item2] = Map[position.Item1][position.Item2];
                Map[position.Item1][position.Item2] = '.';
                return true;
            case Space.Box:
                // Need to recursively check if we can move the box
                var recursiveMove = Move(adjacentPosition.Value, direction);
                if (!recursiveMove)
                    return false;
                Map[adjacentPosition.Value.Item1][adjacentPosition.Value.Item2] = Map[position.Item1][position.Item2];
                Map[position.Item1][position.Item2] = '.';
                return true;
            default:
                throw new ArgumentException("Illegal move");
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
            if (GetSpace((y, x)) == Space.Robot)
                return (y, x);

        throw new ArgumentException("Unable to find robot initial position");
    }

    private Space GetSpace((int, int) location)
    {
        var (y, x) = location;
        switch (Map[y][x])
        {
            case '.':
                return Space.Void;
            case '@':
                return Space.Robot;
            case '#':
                return Space.Wall;
            case 'O':
                return Space.Box;
            default:
                throw new ArgumentException("Unknown space");
        }
    }

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

    public static (Sokoban, List<Direction>) ParseInput(string input)
    {
        var map = new List<string>();
        List<Direction> directions = new();
        Sokoban? sokoban = null;

        foreach (var line in File.ReadLines(input))
            // parse map
            if (sokoban == null)
            {
                if (string.IsNullOrEmpty(line))
                    sokoban = new Sokoban(map.ToArray());
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