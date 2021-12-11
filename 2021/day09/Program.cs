var inputPath = "./inputs/input.txt";
var heightMap = GetHeightMap(inputPath);
var lowPoints = GetLowPoints(heightMap);
var riskSum = lowPoints.Select(x => x.Height + 1).Sum();
Console.WriteLine("Risk sum (part1): {0}", riskSum);

var bassinsSize = new List<int>();
foreach (var lowPoint in lowPoints)
{
  var bassin = new List<Point>();
  GetBassin(heightMap, lowPoint.X, lowPoint.Y, ref bassin);
  bassinsSize.Add(bassin.Count);
}
bassinsSize.Sort();
var largestBassinSizeSum = bassinsSize.TakeLast(3).Aggregate(1, (x, y) => x * y);
Console.WriteLine("Largest 3 bassin size sum (part2): {0}", largestBassinSizeSum);

static int[][] GetHeightMap(string inputPath)
{
  var lines = File.ReadAllLines(inputPath);
  var heightMap = new int[lines.Length][];
  for (int i = 0; i < lines.Length; i++)
  {
    heightMap[i] = lines[i].Select(c => int.Parse(c.ToString())).ToArray();
  }
  return heightMap;
}

// Get adjacent points to a given location
static Point[] GetAdjacentPoints(int[][] map, int x, int y)
{
  var adjacentPoints = new List<Point>();

  // Left
  if (x > 0)
  {
    adjacentPoints.Add(new Point(x - 1, y, map[y][x - 1]));
  }

  // Right
  if (x < map[y].Length - 1)
  {
    adjacentPoints.Add(new Point(x + 1, y, map[y][x + 1]));
  }

  // Top
  if (y > 0)
  {
    adjacentPoints.Add(new Point(x, y - 1, map[y - 1][x]));
  }

  // Bottom
  if (y < map.Length - 1)
  {
    adjacentPoints.Add(new Point(x, y + 1, map[y + 1][x]));
  }

  return adjacentPoints.ToArray();
}

// Get low points
static Point[] GetLowPoints(int[][] map)
{
  var lowPoints = new List<Point>();
  for (var y = 0; y < map.Length; y++)
  {
    for (var x = 0; x < map[y].Length; x++)
    {
      if (GetAdjacentPoints(map, x, y).All(a => map[y][x] < a.Height))
      {
        lowPoints.Add(new Point(x, y, map[y][x]));
      }
    }
  }
  return lowPoints.ToArray();
}

// Return all points forming a bassing starting at a given low point.
static void GetBassin(int[][] map, int x, int y, ref List<Point> bassin)
{
  // Add the current point
  bassin.Add(new Point(x, y, map[y][x]));

  // Recursively visit adjacent points not visited yet (unless heigh = 9)
  foreach (var adjacentPoint in GetAdjacentPoints(map, x, y))
  {
    if (adjacentPoint.Height != 9 && bassin.Where(p => p.X == adjacentPoint.X && p.Y == adjacentPoint.Y).Count() == 0)
    {
      GetBassin(map, adjacentPoint.X, adjacentPoint.Y, ref bassin);
    }
  }
}

// Point representation
struct Point
{
  public int X { get; set; }
  public int Y { get; set; }
  public int Height { get; set; }

  public Point(int x, int y, int height)
  {
    X = x;
    Y = y;
    Height = height;
  }
}