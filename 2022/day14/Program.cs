var input = "input.txt";

// Collect all points in the completed paths
var paths = GetPoints(input).ToList();
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

// Part 1
{
  var map = new Map(points, false);
  int units = 0;
  map.Print();
  while (OneMoreUnitOfSand(map)) { units++; }
  map.Print();
  Console.WriteLine("Part 1: {0}", units);
}

// Part 2
{
  var map = new Map(points, true);
  int units = 0;
  map.Print();
  while (map.Get(0, 500) != 'o') { OneMoreUnitOfSand2(map); units++; }
  map.Print();
  Console.WriteLine("Part 2: {0}", units);
}

// Let one particle of sand fall from the top, return true if it settles
bool OneMoreUnitOfSand(Map map)
{
  // Start location
  var x = 500;
  var y = 0;

  while (true)
  {
    // Out of bound
    if (y > map.MaxY)
    {
      return false;
    }

    // Drop down
    if (map.Get(y + 1, x) == '.')
    {
      y++;
    }

    // Slide on sand/rock
    else if (map.Get(y + 1, x) == '#' || map.Get(y + 1, x) == 'o')
    {
      // Out of bound
      if (x < map.MinX) return false;
      if (x > map.MaxX - 1) return false;
      if (y > map.MaxY - 1) return false;

      // Slide left
      if (map.Get(y + 1, x - 1) == '.')
      {
        x--;
        y++;
      }

      // Move right
      else if (map.Get(y + 1, x + 1) == '.')
      {
        x++;
        y++;
      }

      // Stay here
      else
      {
        map.Set(y, x, 'o');
        return true;
      }
    }
  }
}

// Let one particle of sand fall from the top, return true if it settles
bool OneMoreUnitOfSand2(Map map)
{
  // Start location
  var x = 500;
  var y = 0;

  while (true)
  {
    // Drop down
    if (map.Get(y + 1, x) == '.')
    {
      y++;
    }

    // Slide on sand/rock
    else if (map.Get(y + 1, x) == '#' || map.Get(y + 1, x) == 'o')
    {
      // Slide left
      if (map.Get(y + 1, x - 1) == '.')
      {
        x--;
        y++;
      }

      // Move right
      else if (map.Get(y + 1, x + 1) == '.')
      {
        x++;
        y++;
      }

      // Stay here
      else
      {
        map.Set(y, x, 'o');
        return true;
      }
    }
  }
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

class Map
{
  private Dictionary<int, Dictionary<int, char>> _map;
  public int MinX { get; private set; }
  public int MinY { get; private set; }
  public int MaxX { get; private set; }
  public int MaxY { get; private set; }

  public bool InfiniteBottom { get; private set; }

  // Init a map with a list of walls
  public Map(List<Point> points, bool infiniteBottom)
  {
    MinX = Int32.MaxValue;
    MinY = Int32.MaxValue;

    _map = new Dictionary<int, Dictionary<int, char>>();
    foreach (var point in points)
    {
      Set(point.Y, point.X, '#');
    }
    InfiniteBottom = infiniteBottom;
  }

  // Return the char at the given position
  public char Get(int y, int x)
  {
    if (_map.ContainsKey(y) && _map[y].ContainsKey(x))
      return _map[y][x];
    else
    {
      // Add an infinite wall
      if (InfiniteBottom && y >= MaxY + 2)
        return '#';

      // Return some air
      return '.';
    }
  }

  // Set the chart at a given position
  public void Set(int y, int x, char c)
  {
    // Store new character
    if (!_map.ContainsKey(y))
      _map.Add(y, new Dictionary<int, char>());
    if (!_map[y].ContainsKey(x))
      _map[y].Add(x, c);
    else
      _map[y][x] = c;

    // Update dimensions
    MinX = Math.Min(MinX, x);
    MaxX = Math.Max(MaxX, x);
    if (!InfiniteBottom)
    {
      // Don't update the bottom if it's infinite
      MinY = Math.Min(MinY, y);
      MaxY = Math.Max(MaxY, y);
    }
  }

  // Print the map
  public void Print(int margin = 5)
  {
    for (var y = MinY - margin; y <= MaxY + margin; y++)
    {
      for (var x = MinX - margin; x <= MaxX + margin; x++)
      {
        Console.Write(Get(y, x));
      }
      Console.WriteLine();
    }
    Console.WriteLine();
  }
}