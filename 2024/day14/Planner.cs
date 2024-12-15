using System.Text;
using System.Text.RegularExpressions;

namespace day14;

// All coordinate tuples are x, y

public class Planner
{
    public Planner((int, int) space, string input)
    {
        Space = space;
        RobotsPerPosition = new int[space.Item2][];
        for (var i = 0; i < space.Item2; i++) RobotsPerPosition[i] = new int[space.Item1];

        // Parse input
        var regex = new Regex(@"p\=([0-9]+)\,([0-9]+) v\=(\-?[0-9]+)\,(\-?[0-9]+)");
        Robots = File.ReadLines(input).Select(line =>
        {
            var match = regex.Match(line);
            var position = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            var velocity = (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            return new Robot(position, velocity);
        }).ToList();
    }

    public (int, int) Space { get; }

    public List<Robot> Robots { get; }

    public int[][] RobotsPerPosition { get; }

    // Make all robots move one step
    public void Tick()
    {
        // Move all robots
        foreach (var robot in Robots)
        {
            var (x, y) = robot.Position;
            var (vx, vy) = robot.Velocity;

            robot.Position = (Bound(x + vx, Space.Item1), Bound(y + vy, Space.Item2));
            ;
        }

        // Recompute map
        for (var y = 0; y < Space.Item2; y++)
        for (var x = 0; x < Space.Item1; x++)
        {
            var robots = Robots.Count(r => r.Position == (x, y));
            RobotsPerPosition[y][x] = robots;
        }
    }
    
    public int GetSafetyFactor()
    {
        // 0 1
        // 2 3
        var quadrants = new[] { 0, 0, 0, 0 };

        var yy = Space.Item2 / 2;
        var xx = Space.Item1 / 2;

        for (var y = 0; y < RobotsPerPosition.Length; y++)
        for (var x = 0; x < RobotsPerPosition[y].Length; x++)
        {
            if (y < yy && x < xx)
                quadrants[0] += RobotsPerPosition[y][x];
            if (y < yy && x > xx)
                quadrants[1] += RobotsPerPosition[y][x];
            if (y > yy && x < xx)
                quadrants[2] += RobotsPerPosition[y][x];
            if (y > yy && x > xx)
                quadrants[3] += RobotsPerPosition[y][x];
        }

        return quadrants[0] * quadrants[1] * quadrants[2] * quadrants[3];
    }

    private int Bound(int value, int dimension)
    {
        if (value >= dimension) value %= dimension;

        if (value < 0) value = (dimension + value) % dimension;

        return value;
    }

    public string PrintMap()
    {
        var sb = new StringBuilder();
        for (var y = 0; y < Space.Item2; y++)
        {
            for (var x = 0; x < Space.Item1; x++)
            {
                var robots = RobotsPerPosition[y][x];
                sb.Append(robots == 0 ? "." : robots > 9 ? "#" : robots);
            }

            sb.AppendLine();
        }

        sb.AppendLine();
        return sb.ToString();
    }

    public class Robot((int, int) position, (int, int) velocity)
    {
        public (int, int) Position { get; set; } = position;
        public (int, int) Velocity { get; } = velocity;
    }
}