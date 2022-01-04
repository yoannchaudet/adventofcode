using System.Text;

var inputPath = "./inputs/input.txt";
var logSteps = true;
var (algorithm, map) = ParseInput(inputPath);

var infiniMap = new InfiniMap(map);
infiniMap.Default = '.';
if (logSteps)
{
  Console.WriteLine(infiniMap);
  Console.WriteLine("---");
}

for (var step = 1; step <= 2; step++)
{
  infiniMap = Iterate(infiniMap, algorithm, step);
  if (logSteps)
  {
    Console.WriteLine("Step {0}", step);
    Console.WriteLine(infiniMap);
    Console.WriteLine("---");
  }
}

Console.WriteLine("Lit pixels = {0}", infiniMap.LitPixels());

static InfiniMap Iterate(InfiniMap map, string algorithm, int step)
{
  var newMap = new InfiniMap(map);
  if (step % 2 == 1 && algorithm[0] == '#') {
    newMap.Default = '#';
  } else {
    newMap.Default = '.';
  }

  for (var y = map.MinY - 1; y <= map.MaxY + 1; y++)
  {
    for (var x = map.MinX - 1; x <= map.MaxX + 1; x++)
    {
      newMap.Set(x, y, algorithm[GetPixelIndex(map, x, y)]);
    }
  }
  return newMap;
}

static int GetPixelIndex(InfiniMap map, int x, int y)
{
  var pixelString = "";

  // First line
  pixelString += map.Get(x - 1, y - 1);
  pixelString += map.Get(x, y - 1);
  pixelString += map.Get(x + 1, y - 1);

  // Second line
  pixelString += map.Get(x - 1, y);
  pixelString += map.Get(x, y);
  pixelString += map.Get(x + 1, y);

  // Third line
  pixelString += map.Get(x - 1, y + 1);
  pixelString += map.Get(x, y + 1);
  pixelString += map.Get(x + 1, y + 1);

  pixelString = pixelString.Replace(".", "0");
  pixelString = pixelString.Replace("#", "1");
  return Convert.ToInt32(pixelString, 2);
}

static (string, char[][]) ParseInput(string inputPath)
{
  var lines = File.ReadAllLines(inputPath);
  var algorithm = lines.First();
  var map = lines.Skip(2).Select(line => line.ToArray()).ToArray();
  return (algorithm, map);
}

class InfiniMap
{
  public int MinY { get; set; }
  public int MaxY { get; private set; }

  public int MinX { get; set; }
  public int MaxX { get; private set; }

  private IDictionary<(int, int), char> _map;

  public char Default { get; set ; }

  public InfiniMap(InfiniMap otherMap)
  {
    _map = new Dictionary<(int, int), char>(otherMap._map);
    MinX = otherMap.MinX;
    MinY = otherMap.MinY;
    MaxX = otherMap.MaxX;
    MaxY = otherMap.MaxY;
  }

  public InfiniMap(char[][] map)
  {
    // Load the map
    _map = new Dictionary<(int, int), char>();
    for (var y = 0; y < map.Length; y++)
    {
      for (var x = 0; x < map[0].Length; x++)
      {
        _map.Add((x, y), map[y][x]);
      }
    }
    MinX = 0;
    MinY = 0;
    MaxY = map.Length - 1;
    MaxX = map[0].Length - 1; // Assume the map is regular
  }

  public char Get(int x, int y)
  {
    if (_map.ContainsKey((x, y)))
    {
      return _map[(x, y)];
    }
    return Default;
  }

  public void Set(int x, int y, char c)
  {
    if (_map.ContainsKey((x, y)))
    {
      _map[(x, y)] = c;
    }
    else
    {
      _map.Add((x, y), c);
    }

    // Update the min/max
    if (x < MinX)
    {
      MinX = x;
    }
    if (x > MaxX)
    {
      MaxX = x;
    }
    if (y < MinY)
    {
      MinY = y;
    }
    if (y > MaxY)
    {
      MaxY = y;
    }
  }

  public int LitPixels()
  {
    var sum = 0;
    for (var y = MinY; y <= MaxY; y++)
    {
      for (var x = MinX; x <= MaxX; x++)
      {
        if (Get(x, y) == '#')
        {
          sum++;
        }
      }
    }
    return sum;
  }

  public override string ToString()
  {
    var output = new StringBuilder();
    for (var y = MinY; y <= MaxY; y++)
    {
      for (var x = MinX; x <= MaxX; x++)
      {
        output.Append(Get(x, y));
      }
      output.AppendLine();
    }
    return output.ToString();
  }
}