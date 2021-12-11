var inputPath = "./inputs/input.txt";
var heightMap = GetHeightMap(inputPath);
var riskSum = GetLowPoints(heightMap).Select(x => x + 1).Sum();
Console.WriteLine("Risk sum (part1): {0}", riskSum);

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

static int[] GetAdjacentPoints(int[][] map, int x, int y)
{
  var adjacentPoints = new List<int>();

  // Left
  if (x > 0)
  {
    adjacentPoints.Add(map[y][x - 1]);
  }

  // Right
  if (x < map[y].Length - 1)
  {
    adjacentPoints.Add(map[y][x + 1]);
  }

  // Top
  if (y > 0)
  {
    adjacentPoints.Add(map[y - 1][x]);
  }

  // Bottom
  if (y < map.Length - 1)
  {
    adjacentPoints.Add(map[y + 1][x]);
  }

  return adjacentPoints.ToArray();
}

static int[] GetLowPoints(int[][] map)
{
  var lowPoints = new List<int>();
  for (var y = 0; y < map.Length; y++)
  {
    for (var x = 0; x < map[y].Length; x++)
    {
      if (GetAdjacentPoints(map, x, y).All(a => map[y][x] < a))
      {
        lowPoints.Add(map[y][x]);
      }
    }
  }
  return lowPoints.ToArray();
}
