using Priority_Queue;

var inputPath = "./inputs/input.txt";
var map = GetMap(inputPath);
var shortestRisk = GetShortestRisk(map, map[0][0], map[map.Length - 1][map[map.Length - 1].Length - 1]);
Console.WriteLine("Shortest risk (part 1): {0}", shortestRisk);

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

static Point[][] GetMap(string inputPath)
{
  List<Point[]> map = new List<Point[]>();

  // Init the map
  int y = 0;
  foreach (var line in File.ReadAllLines(inputPath))
  {
    var cols = new Point[line.Length];
    for (var x = 0; x < line.Length; x++)
    {
      cols[x] = new Point(x, y, int.Parse(line[x].ToString()));
    }
    map.Add(cols);
    y++;
  }

  // Init neighbors
  for (y = 0; y < map.Count; y++)
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
      if (y < map.Count - 1)
      {
        point.Neighbours.Add(map[y + 1][x]);
      }
    }
  }

  return map.ToArray();
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