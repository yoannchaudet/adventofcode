var inputPath = "./inputs/input.txt";
ComputeSegments(inputPath, false);
ComputeSegments(inputPath, true);

static void ComputeSegments(string inputPath, bool computeDiagonals)
{
  var coordinates = File.ReadAllLines(inputPath);
  var segments = coordinates.Select(c => new Segment(c, computeDiagonals)).ToList();

  // Create diagram
  int maxX = segments.SelectMany(s => s.PointsInSegment).Max(p => p.X);
  int maxY = segments.SelectMany(s => s.PointsInSegment).Max(p => p.Y);
  int[][] diagram = new int[maxY + 1][];
  for (int y = 0; y <= maxY; y++)
  {
    diagram[y] = new int[maxX + 1];
    for (int x = 0; x <= maxX; x++)
    {
      diagram[y][x] = 0;
    }
  }
  segments.SelectMany(s => s.PointsInSegment).ToList().ForEach(p => diagram[p.Y][p.X]++);
  // Don't print diagram (too big in part 2)
  // PrintDiagram(diagram);
  var overlapCounts = diagram.SelectMany(row => row).Where(c => c >= 2).Count();
  Console.WriteLine("Result (compute diagonals = {1}) = {0}", overlapCounts, computeDiagonals);
}

static void PrintDiagram(int[][] diagram)
{
  for (int y = 0; y < diagram.Length; y++)
  {
    for (int x = 0; x < diagram[y].Length; x++)
    {
      if (diagram[y][x] == 0)
      {
        Console.Write(".");
      }
      else
      {
        Console.Write(diagram[y][x]);
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
  private bool computeDiagonals;

  public Point Start { get; private set; }
  public Point End { get; private set; }

  public List<Point> PointsInSegment { get; private set; }

  public Segment(string coordinates, bool computeDiagonals)
  {
    this.computeDiagonals = computeDiagonals;

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

    // Diagonal
    else if (computeDiagonals)
    {
      var dx = Start.X > End.X ? -1 : 1;
      var dy = Start.Y > End.Y ? -1 : 1;
      int x = Start.X, y = Start.Y;
      do
      {
        pointsInSegment.Add(new Point($"{x},{y}"));
        x += dx;
        y += dy;
      } while (x != End.X + dx && y != End.Y + dy);
    }

    return pointsInSegment;
  }
}