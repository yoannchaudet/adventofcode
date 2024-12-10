namespace day10;

public class TrailBlazer
{
    public TrailBlazer(string input)
    {
        Map = File.ReadLines(input).Select(l =>
                l.ToCharArray().Select(c => char.IsDigit(c) ? int.Parse(c.ToString()) : -1).ToArray())
            .ToArray();
    }

    private int[][] Map { get; }

    public int GetPart1()
    {
        var part1 = 0;
        var trailheadsCandidates = GetTraildeadsCandidates().ToList();
        foreach (var trailhead in trailheadsCandidates)
        {
            var exploredLocations = new HashSet<(int, int)>();
            part1 += ExploreTrailheadCandidate(exploredLocations, trailhead, 0);
        }

        return part1;
    }

    public int GetPart2()
    {
        var part2 = 0;
        var trailheadsCandidates = GetTraildeadsCandidates().ToList();
        foreach (var trailhead in trailheadsCandidates)
            part2 += ExploreTrailheadRating(new List<(int, int)>(), trailhead, 0).Count();

        return part2;
    }

    // Recursively explore all trails starting from a given trailhead and return the trailhead score
    private int ExploreTrailheadCandidate(ISet<( int, int)> exploredLocations, (int, int) currentLocation,
        int currentHeight)
    {
        var score = 0;

        foreach (var direction in Enum.GetValues<Direction>())
        {
            var nextLocation = GetNextLocation(currentLocation, direction);

            // Out of bond
            if (nextLocation == null)
                continue;

            // Already explored
            if (exploredLocations.Contains(nextLocation.Value))
                continue;

            // Not a gradual slope
            var nextHeight = Map[nextLocation.Value.Item1][nextLocation.Value.Item2];
            if (nextHeight != currentHeight + 1)
                continue;

            // Mark as explored
            exploredLocations.Add(nextLocation.Value);

            // Exit
            if (nextHeight == 9)
                score += 1;
            else
                // Continue exploring
                score += ExploreTrailheadCandidate(exploredLocations, nextLocation.Value, nextHeight);
        }

        return score;
    }

    // Recursively explore all trails starting from a given trailhead and return the different paths leading to a 9 elevation
    private IEnumerable<List<(int, int)>> ExploreTrailheadRating(List<( int, int)> path, (int, int) currentLocation,
        int currentHeight)
    {
        foreach (var direction in Enum.GetValues<Direction>())
        {
            var nextLocation = GetNextLocation(currentLocation, direction);

            // Out of bond
            if (nextLocation == null)
                continue;

            // Already explored
            if (path.Contains(nextLocation.Value))
                continue;

            // Not a gradual slope
            var nextHeight = Map[nextLocation.Value.Item1][nextLocation.Value.Item2];
            if (nextHeight != currentHeight + 1)
                continue;

            // Mark as explored
            var nextPath = new List<(int, int)>(path) { nextLocation.Value };

            // Exit
            if (nextHeight == 9)
                yield return nextPath;
            else
                // Continue exploring
                foreach (var p in ExploreTrailheadRating(nextPath, nextLocation.Value, nextHeight))
                    yield return p;
        }
    }

    // Get candidate trailheads
    private IEnumerable<(int, int)> GetTraildeadsCandidates()
    {
        for (var y = 0; y < Map.Length; y++)
        for (var x = 0; x < Map[y].Length; x++)
            if (Map[y][x] == 0)
                yield return (y, x);
    }

    private (int, int)? GetNextLocation((int, int) location, Direction direction)
    {
        var (y, x) = location;
        return direction switch
        {
            Direction.Up => y > 0 ? (y - 1, x) : null,
            Direction.Right => x < Map[y].Length - 1 ? (y, x + 1) : null,
            Direction.Down => y < Map.Length - 1 ? (y + 1, x) : null,
            Direction.Left => x > 0 ? (y, x - 1) : null,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}