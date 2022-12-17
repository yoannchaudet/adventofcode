using System.Text.RegularExpressions;

var valves = GetValves("input.txt");

// Compute distance between all valves
var valveDistance = new Dictionary<string, Dictionary<string, int>>();
foreach (var valve in valves.Values)
{
  var (distance, _) = BellmanFord(valves, valve.Label);
  valveDistance.Add(valve.Label, distance);
}

var promisingOrders = GetPromisingValveOrders(valves, valveDistance, 30, new List<string>()).ToList();
var pressureRelease = promisingOrders.Select(p => (p, GetPressureRelease(valves, p, valveDistance))).MaxBy(p => p.Item2);
Console.WriteLine("Part 1: {0}", pressureRelease.Item2);

// Return a list of promising valve orders
static IEnumerable<List<string>> GetPromisingValveOrders(Dictionary<string, Valve> allValves, Dictionary<string, Dictionary<string, int>> allValvesDistances, int minutesLeft, List<string> selectedValves)
{
  // Get current valve (either starting point or last one in the chain)
  var currentValve = selectedValves.Count == 0 ? "AA" : selectedValves.Last();

  // Get valves to consider
  var valvesToConsider = allValves.Keys.Where(v => !selectedValves.Contains(v) && allValves[v].FlowRate > 0).ToList();
  if (valvesToConsider.Count > 0)
  {
    int top = selectedValves.Count < 5 ? 3 : 1;
    var topN = valvesToConsider.Select(v => (v, (minutesLeft - allValvesDistances[currentValve][v] - 1) * allValves[v].FlowRate)).OrderByDescending(v => v.Item2).Take(top).ToList();
    foreach (var valve in topN)
    {
      var newSelectedValves = new List<string>(selectedValves);
      newSelectedValves.Add(valve.Item1);
      foreach (var order in GetPromisingValveOrders(allValves, allValvesDistances, (minutesLeft - allValvesDistances[currentValve][valve.Item1] - 1), newSelectedValves))
      {
        yield return order;
      }
    }
  }

  yield return selectedValves;
}

// Return the list of permutations of a given list
static IEnumerable<List<string>> GetPermutations(List<string> list)
{
  if (list.Count == 1) return new List<List<string>> { list };
  var permutations = new List<List<string>>();
  for (int i = 0; i < list.Count; i++)
  {
    var smallerPermutations = GetPermutations(list.Where((val, index) => index != i).ToList());
    foreach (var smallerPermutation in smallerPermutations)
    {
      var permutation = new List<string> { list[i] };
      permutation.AddRange(smallerPermutation);
      permutations.Add(permutation);
    }
  }
  return permutations;
}

static int GetPressureRelease(Dictionary<string, Valve> valves, List<string> valvesToOpen, Dictionary<string, Dictionary<string, int>> valveDistance)
{
  var currentLocation = valves["AA"];
  var pressureRate = 0;
  var pressureRelease = 0;
  var minutes = 1;
  var nextValveIndex = 0;
  var stop = false;
  while (minutes <= 30)
  {
    // select valves that can be opened
    if (!stop && nextValveIndex <= valvesToOpen.Count() - 1)
    {
      var nextValve = valvesToOpen[nextValveIndex++];
      var cost = valveDistance[currentLocation.Label][nextValve];

      if (minutes + cost < 30)
      {
        // move
        minutes += cost;
        pressureRelease += pressureRate * cost;

        // open valve
        minutes++;
        pressureRelease += pressureRate;
        pressureRate += valves[nextValve].FlowRate;
        currentLocation = valves[nextValve];
      }
      else
      {
        stop = true;
      }
    }

    // let time tick
    else
    {
      minutes++;
      pressureRelease += pressureRate;
    }
  }
  return pressureRelease;
}

// get distance, predecessor maps between all valves and a given source
static (Dictionary<string, int>, Dictionary<string, string>) BellmanFord(Dictionary<string, Valve> valves, string source)
{
  var distance = new Dictionary<string, int>();
  var predecessor = new Dictionary<string, string>();
  foreach (var valve in valves.Values)
  {
    distance.Add(valve.Label, Int16.MaxValue);
    predecessor.Add(valve.Label, null);
  }
  distance[source] = 0;
  for (int i = 0; i < valves.Count - 1; i++)
  {
    foreach (var valve in valves.Values)
    {
      foreach (var adjacentValve in valve.AdjacentValves)
      {
        if (distance[valve.Label] + 1 < distance[adjacentValve.Label])
        {
          distance[adjacentValve.Label] = distance[valve.Label] + 1;
          predecessor[adjacentValve.Label] = valve.Label;
        }
      }
    }
  }
  return (distance, predecessor);
}

// Parse the input
static Dictionary<string, Valve> GetValves(string input)
{
  var valves = new Dictionary<string, Valve>();
  var regex = new Regex(@"^Valve ([A-Z]{2}) has flow rate\=([0-9]+)\; tunnels? leads? to valves? (.+)$");
  foreach (var line in File.ReadAllLines(input))
  {
    var match = regex.Match(line);
    if (!match.Success)
    {
      throw new Exception("Invalid line: " + line);
    }

    var label = match.Groups[1].Value.Trim();
    var valve = valves.ContainsKey(label) ? valves[label] : new Valve(label);
    valve.FlowRate = int.Parse(match.Groups[2].Value);
    if (!valves.ContainsKey(valve.Label)) valves.Add(label, valve);
    foreach (var adjacentValve in match.Groups[3].Value.Split(','))
    {
      var adjacentValveLabel = adjacentValve.Trim();
      if (!valves.ContainsKey(adjacentValveLabel))
      {
        valves.Add(adjacentValveLabel, new Valve(adjacentValveLabel));
      }
      valve.AdjacentValves.Add(valves[adjacentValveLabel]);
    }
  }
  return valves;
}

class Valve
{
  public String Label { get; private set; }

  public int FlowRate { get; set; }

  public List<Valve> AdjacentValves { get; set; }

  public Valve(String label)
  {
    Label = label;
    AdjacentValves = new List<Valve>();
  }
}