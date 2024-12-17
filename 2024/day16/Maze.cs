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

    private readonly (int, int, Direction) _currentPosition;

    private readonly char[][] _map;

    public Maze(string input)
    {
        // Init
        _map = File.ReadLines(input).Select(line => line.ToCharArray()).ToArray();
        _currentPosition = GetStartPosition();
    }

    // Return the shortest path's score  and the number of tiles
    public (int, int) GetBestPath()
    {
        // Dijkstra's algorithm (no priority queue)
        // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

        var dist = new Dictionary<(int, int, Direction), int>();
        var prev = new Dictionary<(int, int, Direction), List<(int, int, Direction)>?>();
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
            if (q.Count % 1000 == 0) Console.WriteLine("Remaining nodes: " + q.Count);

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
                    prev[v] = new List<(int, int, Direction)>();
                    prev[v]!.Add(u);
                }
                else if (alt == dist[v])
                {
                    prev[v]!.Add(u);
                }
            }
        }

        return BacktrackPaths(dist, prev);
    }

    private (int, int) BacktrackPaths(Dictionary<(int, int, Direction), int> dist,
        Dictionary<(int, int, Direction), List<(int, int, Direction)>?> prev)
    {
        // List of potential targets
        var targets = new List<(int, int, Direction)>();
        var distance = int.MaxValue;
        foreach (var target in GetEndPosition())
            if (dist[target] < distance)
            {
                targets.Add(target);
                distance = dist[target];
            }
            else if (dist[target] == distance)
            {
                targets.Add(target);
            }

        var allPaths = new List<List<(int, int, Direction)>>();
        foreach (var target in targets)
        {
            var path = new List<(int, int, Direction)> { target };
            allPaths.AddRange(BuildPath(target, path, prev));
        }

        var tiles = new HashSet<(int, int)>();
        foreach (var path in allPaths)
        foreach (var (y, x, _) in path)
            tiles.Add((y, x));
        return (distance, tiles.Count);
    }

    private IEnumerable<List<(int, int, Direction)>> BuildPath((int, int, Direction) current,
        List<(int, int, Direction)> path,
        Dictionary<(int, int, Direction), List<(int, int, Direction)>?> prev)
    {
        // Exit
        if (current == GetStartPosition())
            yield return path;

        var predecessors = prev[current];
        if (predecessors != null)
            foreach (var predecessor in predecessors)
            {
                var newPath = new List<(int, int, Direction)>(path);
                newPath.Add(predecessor);
                foreach (var possiblePath in BuildPath(predecessor, newPath, prev))
                    yield return possiblePath;
            }
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
                foreach (var d in Enum.GetValues<Direction>())
                    yield return (y, x, d);
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