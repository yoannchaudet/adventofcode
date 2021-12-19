using Priority_Queue;

var inputPath = "./inputs/input.txt";
var map = GetMap(inputPath);
var shortestRisk = GetShortestRisk(map, map[0][0], map[map.Length - 1][map[map.Length - 1].Length - 1]);
Console.WriteLine("Shortest risk (part 1): {0}", shortestRisk);

var map2 = GetMap(inputPath, true);
shortestRisk = GetShortestRisk(map2, map2[0][0], map2[map2.Length - 1][map2[map2.Length - 1].Length - 1]);
Console.WriteLine("Shortest risk (part 2): {0}", shortestRisk);

// Hello Dijkstra!
static int GetShortestRisk(Point[][] map, Point source, Point destination)
{
  // See https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

  // Init
  var q = new SimplePriorityQueue<Point, int>();
  var dist = new Dictionary<Point, int>();
  var prev = new Dictionary<Point, Point?>();
  dist[source] = 0;
  prev[source] = null;
  map.SelectMany(p => p).ToList().ForEach(vertex =>
  {
    if (!vertex.Equals(source))
    {
      dist.Add(vertex, int.MaxValue);
      prev.Add(vertex, null);
    }
    q.Enqueue(vertex, dist[vertex]);
  });

  // Search
  while (q.Count > 0)
  {
    var u = q.Dequeue();
    if (u.Equals(destination))
      break;

    foreach (var v in u.Neighbours)
    {
      var alt = dist[u] + v.Risk;
      if (alt < dist[v])
      {
        dist[v] = alt;
        prev[v] = u;
        q.UpdatePriority(v, alt);
      }
    }
  }

  // Reconstruct path
  {
    var u = (Point?)destination;
    var risk = u.Value.Risk;
    while (u != null)
    {
      u = prev[u.Value];
      if (u.HasValue) risk += u.Value.Risk;
    }
    return risk - source.Risk;
  }
}

static Point[][] GetMap(string inputPath, bool isPart2 = false)
{
  var lines = File.ReadAllLines(inputPath);
  var map = new Point[lines.Length][];

  // Init the map
  for (var y = 0; y < lines.Length; y++)
  {
    var line = lines[y];
    var cols = new Point[line.Length];
    for (var x = 0; x < line.Length; x++)
    {
      cols[x] = new Point(x, y, int.Parse(line[x].ToString()));
    }
    map[y] = cols;
  }

  if (isPart2)
  {
    // Grow the map horizontally
    for (var y = 0; y < map.Length; y++)
    {
      var cols = new Point[map[y].Length * 5];
      for (var x = 0; x < map[y].Length; x++)
      {
        cols[x] = map[y][x];
        cols[x + map[y].Length * 1] = new Point(x + map[y].Length * 1, y, GetIncrementalRisk(cols[x].Risk + 1));
        cols[x + map[y].Length * 2] = new Point(x + map[y].Length * 2, y, GetIncrementalRisk(cols[x].Risk + 2));
        cols[x + map[y].Length * 3] = new Point(x + map[y].Length * 3, y, GetIncrementalRisk(cols[x].Risk + 3));
        cols[x + map[y].Length * 4] = new Point(x + map[y].Length * 4, y, GetIncrementalRisk(cols[x].Risk + 4));
      }
      map[y] = cols;
    }

    // Grow the map vertically
    var newMap = new Point[map.Length * 5][];
    for (var y = 0; y < map.Length; y++ ) {
      newMap[y] = map[y];
      newMap[y + map.Length * 1] = new Point[map[y].Length];
      newMap[y + map.Length * 2] = new Point[map[y].Length];
      newMap[y + map.Length * 3] = new Point[map[y].Length];
      newMap[y + map.Length * 4] = new Point[map[y].Length];
      for (var x = 0; x < map[y].Length; x++) {
        newMap[y + map.Length * 1][x] = new Point(x, y + map.Length * 1, GetIncrementalRisk(map[y][x].Risk + 1));
        newMap[y + map.Length * 2][x] = new Point(x, y + map.Length * 2, GetIncrementalRisk(map[y][x].Risk + 2));
        newMap[y + map.Length * 3][x] = new Point(x, y + map.Length * 3, GetIncrementalRisk(map[y][x].Risk + 3));
        newMap[y + map.Length * 4][x] = new Point(x, y + map.Length * 4, GetIncrementalRisk(map[y][x].Risk + 4));
      }
    }
    map = newMap;
  }

  // Init neighbors
  for (var y = 0; y < map.Length; y++)
  {
    for (var x = 0; x < map[y].Length; x++)
    {
      var point = map[y][x];

      // Left
      if (x > 0)
      {
        point.Neighbours.Add(map[y][x - 1]);
      }

      // Right
      if (x < map[y].Length - 1)
      {
        point.Neighbours.Add(map[y][x + 1]);
      }

      // Top
      if (y > 0)
      {
        point.Neighbours.Add(map[y - 1][x]);
      }

      // Bottom
      if (y < map.Length - 1)
      {
        point.Neighbours.Add(map[y + 1][x]);
      }
    }
  }

  return map;
}

static int GetIncrementalRisk(int risk)
{
  if (risk > 9) risk = risk % 10 + 1;
  return risk;
}

struct Point
{
  public int X { get; private set; }
  public int Y { get; private set; }

  public int Risk { get; private set; }

  public List<Point> Neighbours { get; private set; }

  public Point(int x, int y, int risk)
  {
    X = x;
    Y = y;
    Risk = risk;
    Neighbours = new List<Point>();
  }

  public override bool Equals(object? obj)
  {
    if (obj is Point p)
    {
      return X == p.X && Y == p.Y;
    }
    return false;
  }
}