// Part 1
{
  var x = 1;
  var signalStrength = 0;
  var ticks = new List<int>() { 20, 60, 100, 140, 180, 220 };
  var clock = new Clock(tick =>
  {
    if (ticks.Contains(tick))
    {
      signalStrength += tick * x;
    }
  });
  foreach (var instruction in ParseInstructions("input.txt"))
  {
    if (instruction is AddX)
    {
      clock.Tick();
      clock.Tick();
      x += ((AddX)instruction).X;
    }
    else if (instruction is Noop)
    {
      clock.Tick();
    }
  }
  Console.WriteLine("Part 1: {0}", signalStrength);
}

// Parse instructions
static IEnumerable<Instruction> ParseInstructions(string input)
{
  foreach (var line in File.ReadAllLines(input))
  {
    if (line.StartsWith("addx"))
    {
      yield return new AddX(int.Parse(line.Substring("addx ".Length)));
    }
    else if (line == "noop")
    {
      yield return new Noop();
    }
    else
    {
      throw new Exception("Unknown instruction: " + line);
    }
  }
}

// Clock
class Clock
{
  public int Cycle { get; private set; }
  public Action<int> Ticker { get; private set; }

  public Clock(Action<int> ticker)
  {
    Cycle = 0;
    Ticker = ticker;
  }

  public void Tick()
  {
    Cycle++;
    Ticker.Invoke(Cycle);
  }
}

// Instructions
class Instruction
{
}

class Noop : Instruction
{
}

class AddX : Instruction
{
  public int X { get; private set; }

  public AddX(int x)
  {
    X = x;
  }
}