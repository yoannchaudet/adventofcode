using System.Text.RegularExpressions;

var input = "input.txt";
var rowY = 2000000;

// Part 1
{
  var sensorBeacons = ParseInput(input);
  var map = new Map(sensorBeacons);

  // Find the sensors to consider
  var candidates = sensorBeacons.Where(sb => Math.Abs(sb.Sensor.Y - rowY) <= sb.Distance).ToList();

  // Identify the "no beacons" areas
  int noBeacons = 0;
  foreach (var candidate in candidates) {
    for (var x = candidate.Sensor.X - candidate.Distance; x <= candidate.Sensor.X + candidate.Distance; x++) {
      if (map.Get(rowY, x) != Map.AIR) {
        continue;
      }
      if (Point.GetDistance(x, rowY, candidate.Sensor.X, candidate.Sensor.Y) <= candidate.Distance) {
        map.Set(rowY, x, Map.NOTHING);
        noBeacons++;
      }
    }
  }
  Console.WriteLine("Part 1: {0}", noBeacons);
}

// Get the sensor/beacon location from the input
static IEnumerable<SensorBeacon> ParseInput(string input)
{
  var regex = new Regex(@"^Sensor at x=(?<sensorX>[0-9\-]+), y=(?<sensorY>[0-9\-]+)\: closest beacon is at x=(?<beaconX>[0-9\-]+), y=(?<beaconY>[0-9\-]+)$");
  foreach (var line in File.ReadAllLines(input))
  {
    var match = regex.Match(line);
    if (match.Success)
    {
      yield return new SensorBeacon(
        new Point(int.Parse(match.Groups["sensorX"].Value), int.Parse(match.Groups["sensorY"].Value)),
        new Point(int.Parse(match.Groups["beaconX"].Value), int.Parse(match.Groups["beaconY"].Value))
      );
    }
    else
    {
      throw new Exception("Invalid input: " + line);
    }
  }
}

class SensorBeacon
{
  public Point Sensor { get; private set; }
  public Point Beacon { get; private set; }
  public int Distance { get; private set; }

  public SensorBeacon(Point sensor, Point beacon)
  {
    Sensor = sensor;
    Beacon = beacon;
    Distance = Point.GetDistance(sensor, beacon);
  }

  public override String ToString()
  {
    return string.Format("Sensor: {0}, Beacon: {1}, Distance: {2}", Sensor, Beacon, Distance);
  }
}

class Point
{
  public int X { get; private set; }
  public int Y { get; private set; }

  public Point(int x, int y)
  {
    X = x;
    Y = y;
  }

  public static int GetDistance(Point a, Point b)
  {
    return GetDistance(a.X, a.Y, b.X, b.Y);
  }

  public static int GetDistance(int aX, int aY, int bX, int bY)
  {
    return Math.Abs(aX - bX) + Math.Abs(aY - bY);
  }

  public override string ToString()
  {
    return string.Format("({0}, {1})", X, Y);
  }
}

class Map
{
  // Consts
  public const char AIR = '.';
  public const char SENSOR = 'S';
  public const char BEACON = 'B';
  public const char NOTHING = '#';

  private Dictionary<int, Dictionary<int, char>> _map;
  public int MinX { get; private set; }
  public int MinY { get; private set; }
  public int MaxX { get; private set; }
  public int MaxY { get; private set; }

  // Init a map with a list of walls
  public Map(IEnumerable<SensorBeacon> sensorBeacons)
  {
    MinX = Int32.MaxValue;
    MinY = Int32.MaxValue;
    _map = new Dictionary<int, Dictionary<int, char>>();
    foreach (var sensorBeacon in sensorBeacons)
    {
      Set(sensorBeacon.Sensor.Y, sensorBeacon.Sensor.X, SENSOR);
      Set(sensorBeacon.Beacon.Y, sensorBeacon.Beacon.X, BEACON);
    }
  }

  // Return the char at the given position
  public char Get(int y, int x)
  {
    if (_map.ContainsKey(y) && _map[y].ContainsKey(x))
      return _map[y][x];
    else
      return AIR;
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
    MinY = Math.Min(MinY, y);
    MaxY = Math.Max(MaxY, y);
  }

  // Print the map
  public void Print(int margin = 2)
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