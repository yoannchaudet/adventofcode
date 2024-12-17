namespace day16;

public class Maze
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    private readonly char[][] _map;

    private readonly (int, int, Direction) _currentPosition;

    public Maze(string input)
    {
        // Init
        _map = File.ReadLines(input).Select(line => line.ToCharArray()).ToArray();
        _currentPosition = GetStartPosition();
    }

    public int ShortestPath()
    {
        // Dijkstra's algorithm (no priority queue)
        // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

        var dist = new Dictionary<(int, int, Direction), int>();
        var prev = new Dictionary<(int, int, Direction), (int, int, Direction)?>();
        var q = new HashSet<(int, int, Direction)>();
        foreach (var v in GetVertices())
        {
            dist.Add(v, int.MaxValue);
            prev.Add(v, null);
            q.Add(v);
        }

        dist[_currentPosition] = 0;

        while (q.Count > 0)
        {
            if (q.Count % 1000 == 0)
            {
                Console.WriteLine("Remaining nodes: " + q.Count);
            }
            
            var u = q.OrderBy(v => dist[v]).First();
            q.Remove(u);

            if (_map[u.Item1][u.Item2] == 'E')
                break;

            foreach (var v in GetNeighbours(u))
            {
                var alt = dist[u] + (u.Item3 == v.Item3 ? 1 : 1000);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        return GetEndPosition().Min(position => dist[position]);
    }

    private (int, int, Direction) GetStartPosition()
    {
        for (var y = 0; y < _map.Length; y++)
        for (var x = 0; x < _map[y].Length; x++)
            if (_map[y][x] == 'S')
                return (y, x, Direction.East);

        throw new Exception("Unable to find start position");
    }

    private IEnumerable<(int, int, Direction)> GetEndPosition()
    {
        for (var y = 0; y < _map.Length; y++)
        for (var x = 0; x < _map[y].Length; x++)
            if (_map[y][x] == 'E')
            {
                foreach (var d in Enum.GetValues<Direction>())
                    yield return (y, x, d);
            }
    }

    private IEnumerable<(int, int, Direction)> GetVertices()
    {
        for (var y = 0; y < _map.Length; y++)
        for (var x = 0; x < _map[y].Length; x++)
            foreach (var d in Enum.GetValues<Direction>())
                yield return (y, x, d);
    }

    private IEnumerable<(int, int, Direction)> GetNeighbours((int, int, Direction) location)
    {
        var (y, x, direction) = location;

        // 90 degree turns
        switch (direction)
        {
            case Direction.East:
                yield return (y, x, Direction.North);
                yield return (y, x, Direction.South);
                break;
            case Direction.South:
                yield return (y, x, Direction.East);
                yield return (y, x, Direction.West);
                break;
            case Direction.West:
                yield return (y, x, Direction.South);
                yield return (y, x, Direction.North);
                break;
            case Direction.North:
                yield return (y, x, Direction.West);
                yield return (y, x, Direction.East);
                break;
        }

        // Forward
        var (dy, dx) = direction switch
        {
            Direction.North => (-1, 0),
            Direction.East => (0, 1),
            Direction.South => (1, 0),
            Direction.West => (0, -1),
            _ => throw new ArgumentOutOfRangeException()
        };
        if (y + dy >= 0 && y + dy < _map.Length && x + dx >= 0 && x + dx < _map[y].Length)
        {
            var mapChar = _map[y + dy][x + dx];
            if (mapChar == '.' || mapChar == 'E' || mapChar == 'S')
                yield return (y + dy, x + dx, direction);
        }
    }
}