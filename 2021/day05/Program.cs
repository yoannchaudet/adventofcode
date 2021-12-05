// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var inputPath = "./inputs/input.txt";
var coordinates = File.ReadAllLines(inputPath);
var segments = coordinates.Select(c => new Segment(c)).ToList();

// Create diagram
int maxX = segments.SelectMany(s => s.PointsInSegment).Max(p => p.X);
int maxY = segments.SelectMany(s => s.PointsInSegment).Max(p => p.Y);
int[][] diagram = new int[maxX + 1][];
for (int x = 0; x <= maxX; x++)
{
  diagram[x] = new int[maxY + 1];
  for (int y = 0; y <= maxY; y++)
  {
    diagram[x][y] = 0;
  }
}
segments.SelectMany(s => s.PointsInSegment).ToList().ForEach(p => diagram[p.X][p.Y]++);
// PrintDiagram(diagram);
var overlapCounts = diagram.SelectMany(row => row).Where(c => c >= 2).Count();
Console.WriteLine("Result (part1) = {0}", overlapCounts);

static void PrintDiagram(int[][] diagram)
{
  for (int x = 0; x < diagram.Length; x++)
  {
    for (int y = 0; y < diagram[x].Length; y++)
    {
      if (diagram[x][y] == 0)
      {
        Console.Write(".");
      }
      else
      {
        Console.Write(diagram[x][y]);
      }
    }
    Console.WriteLine();
  }
}

class Point
{
  public int X { get; private set; }
  public int Y { get; private set; }

  public Point(string coordinates)
  {
    var parts = coordinates.Split(',');
    X = int.Parse(parts[0]);
    Y = int.Parse(parts[1]);
  }
}

class Segment
{
  public Point Start { get; private set; }
  public Point End { get; private set; }

  public List<Point> PointsInSegment { get; private set; }

  public Segment(string coordinates)
  {
    var parts = coordinates.Split("->");
    Start = new Point(parts[0]);
    End = new Point(parts[1]);
    PointsInSegment = InitializePointsInSegment();
  }

  private List<Point> InitializePointsInSegment()
  {
    var pointsInSegment = new List<Point>();

    // Horizontal
    if (Start.X == End.X)
    {
      for (int y = Math.Min(Start.Y, End.Y); y <= Math.Max(Start.Y, End.Y); y++)
      {
        pointsInSegment.Add(new Point($"{Start.X},{y}"));
      }
    }

    // Vertical
    else if (Start.Y == End.Y)
    {
      for (int x = Math.Min(Start.X, End.X); x <= Math.Max(Start.X, End.X); x++)
      {
        pointsInSegment.Add(new Point($"{x},{Start.Y}"));
      }
    }

    return pointsInSegment;
  }
}