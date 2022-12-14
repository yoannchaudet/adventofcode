var input = "input.txt";
var paths = GetPoints(input).ToList();

// Collect all points in the completed paths
var points = new List<Point>();
foreach (var path in paths)
{
  var start = path.First();
  foreach (var end in path.Skip(1))
  {
    points.AddRange(GetLine(start, end));
    start = end;
  }
}

// Get the map
var (map, viewBoxA, viewBoxB) = GetMap(points);
int units = 0;
while (OneMoreUnitOfSand(map)) { units++; }
Print(map, viewBoxA, viewBoxB);
Console.WriteLine("Part 1: {0}", units);

// Let one particle of sand fall from the top, return true if it settles
bool OneMoreUnitOfSand(char[][] map)
{
  // Location of sand
  var x = 500;
  var y = 0;

  while (true)
  {

    // out of bound
    if (y >= map.Length - 1)
    {
      return false;
    }

    // drop down
    if (map[y + 1][x] == '.')
    {
      y++;
    }

    // slide on sand
    else if (map[y + 1][x] == '#' || map[y + 1][x] == 'o')
    {
      // out
      if (x < 1) return false;
      if (x > map[y].Length - 2) return false;
      if (y > map.Length - 2) return false;

      // slide left
      if (map[y + 1][x - 1] == '.')
      {
        x--;
        y++;
      }

      // move right
      else if (map[y + 1][x + 1] == '.')
      {
        x++;
        y++;
      }

      // stay here
      else
      {
        map[y][x] = 'o';
        return true;
      }
    }
  }

  return true;
}


// Print the map given a viewbox defined by a and b
static void Print(char[][] map, Point a, Point b, int margin = 5)
{
  for (var y = a.Y - margin; y <= b.Y + margin; y++)
  {
    for (var x = a.X - margin; x <= b.X + margin; x++)
    {
      if (y > 0 && y < map.Length && x > 0 && x < map[y].Length)
      {
        Console.Write(map[y][x]);
      }
      else
      {
        // extra air
        Console.Write('.');
      }

    }
    Console.WriteLine();
  }
}

// Return the map and its viewbox (max y, max x)
static (char[][], Point, Point) GetMap(List<Point> points)
{
  // Init the map
  var maxX = points.Max(p => p.X);
  var maxY = points.Max(p => p.Y);
  var map = new List<char[]>();
  for (var y = 0; y <= maxY; y++)
  {
    var line = new List<char>();
    for (var x = 0; x <= maxX; x++)
    {
      line.Add('.');
    }
    map.Add(line.ToArray());
  }

  // Add the rock points
  foreach (var point in points)
  {
    map[point.Y][point.X] = '#';
  }

  // Compute the viewbox
  var vbMinY = points.Select(p => p.Y).Min();
  var vbMaxY = points.Select(p => p.Y).Max();
  var vbMinX = points.Select(p => p.X).Min();
  var vbMaxX = points.Select(p => p.X).Max();

  return (map.ToArray(), new Point() { X = vbMinX, Y = vbMinY }, new Point() { X = vbMaxX, Y = vbMaxY });
}


// Get the points from the input
static IEnumerable<List<Point>> GetPoints(string input)
{
  foreach (var line in File.ReadAllLines(input))
  {
    var parts = line.Split("->");
    var points = new List<Point>();
    foreach (var part in parts)
    {
      var coordinates = part.Trim().Split(",").Select(int.Parse);
      points.Add(new Point() { X = coordinates.First(), Y = coordinates.Last() });
    }
    yield return points;
  }
}

// Get all points between start and end
static IEnumerable<Point> GetLine(Point start, Point end)
{
  for (var y = Math.Min(start.Y, end.Y); y <= Math.Max(start.Y, end.Y); y++)
  {
    for (var x = Math.Min(start.X, end.X); x <= Math.Max(start.X, end.X); x++)
    {
      yield return new Point() { X = x, Y = y };
    }
  }
}

// Convenience struct for parsing the input
struct Point
{
  public int X { get; set; }
  public int Y { get; set; }
}
