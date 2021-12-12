var filePath = "./inputs/input.txt";
var cave = ParseCave(filePath);

var distinctPaths = new List<string>();
WalkCaves(cave, "", ref distinctPaths);
Console.WriteLine("Distinct paths (part 1): {0}", distinctPaths.Count);

var distinctPaths2 = new List<string>();
WalkCaves2(cave, "", ref distinctPaths2);
Console.WriteLine("Distinct paths (part 2): {0}", distinctPaths2.Count);

static void WalkCaves(Cave cave, string path, ref List<string> distinctPaths)
{
  foreach (var neighbor in cave.Neighbors)
  {
    if (neighbor.Label == "start")
    {
      continue;
    }
    else if (neighbor.Label == "end")
    {
      if (!distinctPaths.Contains(path))
      {
        distinctPaths.Add(path);
      }
      continue;
    }
    else if (neighbor.IsSmallCave && path.Contains(neighbor.Label))
    {
      // Small caves can only be visited once
      continue;
    }
    else
    {
      WalkCaves(neighbor, path + "," + neighbor.Label, ref distinctPaths);
    }
  }
}

static void WalkCaves2(Cave cave, string path, ref List<string> distinctPaths)
{
  foreach (var neighbor in cave.Neighbors)
  {
    if (neighbor.Label == "start")
    {
      continue;
    }
    else if (neighbor.Label == "end")
    {
      if (!distinctPaths.Contains(path))
      {
        distinctPaths.Add(path);
      }
      continue;
    }
    else
    {
      if (neighbor.IsSmallCave && path.Contains(neighbor.Label))
      {
        // This is really not optimized...
        var visitedSmallCaveTwice = path.Split(',').ToList().GroupBy(x => x).Where(x => x.Key.ToLower() == x.Key && x.Count() > 1).ToList().Count > 0;
        if (!visitedSmallCaveTwice)
        {
          visitedSmallCaveTwice = true;
        }
        else
        {
          continue;
        }
      }

      WalkCaves2(neighbor, path + "," + neighbor.Label, ref distinctPaths);
    }
  }
}

// Parse the input and return the start cave.
static Cave ParseCave(string filePath)
{
  var caves = new Dictionary<string, Cave>();
  foreach (var line in File.ReadAllLines(filePath))
  {
    var parts = line.Split('-');
    var from = parts[0];
    var to = parts[1];

    if (!caves.ContainsKey(from))
    {
      caves[from] = new Cave(from);
    }
    if (!caves.ContainsKey(to))
    {
      caves[to] = new Cave(to);
    }
    caves[from].Neighbors.Add(caves[to]);
    caves[to].Neighbors.Add(caves[from]);
  }
  return caves["start"];
}

struct Cave
{
  public string Label { get; }

  public List<Cave> Neighbors { get; }

  public bool IsSmallCave { get; }

  public Cave(string label)
  {
    Label = label;
    Neighbors = new List<Cave>();
    IsSmallCave = label.ToLower() == label;
  }
}