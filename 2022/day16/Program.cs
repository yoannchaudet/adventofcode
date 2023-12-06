using System.Collections;
using System.Text.RegularExpressions;

var valves = GetValves("inputs/sample.txt");
var steps = new Steps();
steps.Move("DD");
steps.OpenValve("DD", valves["DD"].FlowRate);
steps.Move("CC");
steps.Move("BB");
steps.OpenValve("BB", valves["BB"].FlowRate);
steps.Move("AA");
steps.Move("II");
steps.Move("JJ");
steps.OpenValve("JJ", valves["JJ"].FlowRate);
steps.Move("II");
steps.Move("AA");
steps.Move("DD");
steps.Move("EE");
steps.Move("FF");
steps.Move("GG");
steps.Move("HH");
steps.OpenValve("HH", valves["HH"].FlowRate);
steps.Move("GG");
steps.Move("FF");
steps.Move("EE");
steps.OpenValve("EE", valves["EE"].FlowRate);
steps.Move("DD");
steps.Move("CC");
steps.OpenValve("CC", valves["CC"].FlowRate);
Console.WriteLine(steps);

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
    var valve = valves.TryGetValue(label, out var valve1) ? valve1 : new Valve(label);
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

class Steps
{
  public List<string> Path { get; init; }
  private int MaxSteps { get; init; }
  public int FlowRate { get; private set; }

  public Steps(int maxSteps = 30)
  {
    Path = new List<string>();
    MaxSteps = 30;
    FlowRate = 0;
  }
  
  public void Move(string valve)
  {
    if (!IsComplete())
      Path.Add($"Move to {valve}");
  }

  public void OpenValve(string valve, int flowRate)
  {
    if (!IsComplete())
    {
      Path.Add($"Open valve {valve}");
      var remainingSteps = MaxSteps - Path.Count;
      if (remainingSteps > 0)
        FlowRate += remainingSteps * flowRate;
    }
  }
  
  public bool IsComplete()
  {
    return Path.Count() >= MaxSteps;
  }
  
  public override string ToString()
  {
    return string.Join(System.Environment.NewLine, Path) + System.Environment.NewLine + $"Total flow rate = {FlowRate}";
  }
} 