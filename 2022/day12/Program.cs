var input = "input.txt";
var map = GetMap(input);
var target = GetPoints(map).First(x => map[x.Y][x.X] == 'E');

// Part 1
{
  var source = GetPoints(map).First(x => map[x.Y][x.X] == 'S');
  var (dist, prev) = Djkstra( new Point[] {source}, target, map);
  var shortestPathLength = GetShortestPathLength(source, target, dist, prev);
  Console.WriteLine("Part 1: {0}", shortestPathLength);
}

// Part 2
{
  var sources = GetPoints(map).Where(x => map[x.Y][x.X] == 'a' || map[x.Y][x.X] == 'S');
  var (dist, prev) = Djkstra( sources.ToArray(), target, map);
  var min = sources.Select(source => GetShortestPathLength(source, target, dist, prev)).Min();
  Console.WriteLine("Part 2: {0}", min);
}

static int GetShortestPathLength(Point source, Point target, Dictionary<Point, int> dist, Dictionary<Point, Point?> prev)
{
  var s = new List<Point>();
  var u = target;
  if (prev[u] != null || u == source)
  {
    while (u != null)
    {
      s.Add(u);
      u = prev[u];
    }
  }
  return s.Count() - 1;
}

static (Dictionary<Point, int>, Dictionary<Point, Point?>) Djkstra(Point[] sources, Point target, char[][] map)
{
  // Init Dijkstra
  var dist = new Dictionary<Point, int>();
  var prev = new Dictionary<Point, Point?>();
  var q = new List<Point>();
  foreach (var vertex in GetPoints(map))
  {
    dist[vertex] = int.MaxValue;
    prev[vertex] = null;
    q.Add(vertex);
  }
  foreach (var source in sources)
    dist[source] = 0;

  // Run Dijkstra
  while (q.Count > 0)
  {
    var u = q.OrderBy(x => dist[x]).First();
    if (u == target)
    {
      break;
    }
    q.Remove(u);

    foreach (var v in GetAccessibleNeighbors(u, map).Where(v => q.Contains(v)))
    {
      var alt = dist[u] + 1;
      if (alt < dist[v])
      {
        dist[v] = alt;
        prev[v] = u;
      }
    }
  }

  return (dist, prev);
}

// Get acceptable neighbors
static IEnumerable<Point> GetAccessibleNeighbors(Point point, char[][] map)
{
  // Get elevation of current point
  var elevation = GetElevation(point, map);

  // Process accessible neighbors
  foreach (var neighbor in GetNeighbors(point, map))
  {
    var neighborElevation = GetElevation(neighbor, map);
    if ((neighborElevation - elevation) <= 1)
      yield return neighbor;
  }
}

static char GetElevation(Point point, char[][] map)
{
  var elevation = map[point.Y][point.X];
  if (elevation == 'S')
    elevation = 'a';
  else if (elevation == 'E')
    elevation = 'z';
  return elevation;
}

// Return the neighbors of a given point (up, down, left, right)
static IEnumerable<Point> GetNeighbors(Point point, char[][] map)
{
  // Up
  if (point.Y > 0)
    yield return new Point { X = point.X, Y = point.Y - 1 };

  // Down
  if (point.Y < map.Length - 1)
    yield return new Point { X = point.X, Y = point.Y + 1 };

  // Left
  if (point.X > 0)
    yield return new Point { X = point.X - 1, Y = point.Y };

  // Right
  if (point.X < map[point.Y].Length - 1)
    yield return new Point { X = point.X + 1, Y = point.Y };
}

static IEnumerable<Point> GetPoints(char[][] map)
{
  for (int y = 0; y < map.Length; y++)
  {
    for (int x = 0; x < map[y].Length; x++)
    {
      yield return new Point { X = x, Y = y };
    }
  }
}

static char[][] GetMap(string input)
{
  return File.ReadAllLines(input).Select(x => x.ToCharArray()).ToArray();
}

class Point
{
  public int X { get; set; }
  public int Y { get; set; }

  public override bool Equals(object obj)
  {
    var other = obj as Point;
    if (other == null)
      return false;
    return X == other.X && Y == other.Y;
  }

  public override int GetHashCode()
  {
    return X.GetHashCode() ^ Y.GetHashCode();
  }
}