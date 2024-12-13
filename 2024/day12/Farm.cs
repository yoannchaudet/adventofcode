namespace day12;

public class Farm
{
    public enum Direction
    {
        Top,
        Right,
        Bottom,
        Left
    }

    public Farm(string input)
    {
        Map = File.ReadLines(input).Select(line => line.ToCharArray()).ToArray();
    }

    public char[][] Map { get; }

    // Return the regions in the farm
    public List<Region> GetRegions()
    {
        // List of plots in the map
        var plots = new List<(int, int)>();
        for (var y = 0; y < Map.Length; y++)
        for (var x = 0; x < Map[y].Length; x++)
            plots.Add((y, x));

        // While we still have plots left to explore, find regions
        var regions = new List<Region>();
        while (plots.Count > 0)
        {
            var start = plots.First();
            var regionPlots = GetRegionPlots(start);
            regionPlots.ForEach(p => plots.Remove(p));
            var region = new Region(Map[start.Item1][start.Item2], regionPlots, GetPerimeter(regionPlots));
            regions.Add(region);
        }

        return regions;
    }

    // Compute the perimeter of a region
    private int GetPerimeter(List<(int, int)> regionPlots)
    {
        var perimeter = 0;
        foreach (var plot in regionPlots)
        foreach (var direction in Enum.GetValues<Direction>())
        {
            var adjacent = GetAdjacentPlot(plot, direction);
            if (adjacent == null || !regionPlots.Contains(adjacent.Value))
                perimeter++;
        }

        return perimeter;
    }

    // Find all plots in a region for a given starting plot
    private List<(int, int)> GetRegionPlots((int, int) start)
    {
        var visited = new List<(int, int)>();
        var plots = new List<(int, int)>();
        GetRegionPlots(start, start, visited, plots);
        return plots;
    }

    // Given a starting plot, return the plot delimiting a region
    private void GetRegionPlots((int, int) start, (int, int) current, List<(int, int)> visited, List<(int, int)> plots)
    {
        var startPlant = Map[start.Item1][start.Item2];
        var currentPlant = Map[current.Item1][current.Item2];

        // Exit condition
        if (visited.Contains(current))
            return;

        // Expand region
        if (!plots.Contains(current) && startPlant == currentPlant)
            plots.Add(current);

        // Or stop exploring
        else
            return;

        // Mark current plot as visited
        visited.Add(current);

        // Process neighbors
        foreach (var direction in Enum.GetValues<Direction>())
        {
            var adjacent = GetAdjacentPlot(current, direction);
            if (adjacent != null) GetRegionPlots(start, adjacent.Value, visited, plots);
        }
    }

    // Return the adgacent plot in the given direction
    private (int, int)? GetAdjacentPlot((int, int) reference, Direction direction)
    {
        var (y, x) = reference;
        switch (direction)
        {
            case Direction.Top:
                return y > 0 ? (y - 1, x) : null;
            case Direction.Right:
                return x < Map[y].Length - 1 ? (y, x + 1) : null;
            case Direction.Bottom:
                return y < Map.Length - 1 ? (y + 1, x) : null;
            case Direction.Left:
                return x > 0 ? (y, x - 1) : null;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Direction RotateDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Top => Direction.Right,
            Direction.Right => Direction.Bottom,
            Direction.Bottom => Direction.Left,
            Direction.Left => Direction.Top,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public class Region
    {
        public Region(char plant, List<( int, int)> plots, int perimeter)
        {
            Plant = plant;
            Plots = plots;
            Perimeter = perimeter;
        }

        public char Plant { get; }

        // Location of the plots in the region
        private List<(int, int)> Plots { get; }

        public int Area => Plots.Count;

        public int Perimeter { get; init; }
    }
}