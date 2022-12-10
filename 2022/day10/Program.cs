var input = "input.txt";

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
  foreach (var instruction in ParseInstructions(input))
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

// Part 2
{
  // register
  var x = 1;
  // current crt row
  var crtRow = -1;
  // crt
  var crt = new char[][] {
    "........................................".ToArray(),
    "........................................".ToArray(),
    "........................................".ToArray(),
    "........................................".ToArray(),
    "........................................".ToArray(),
    "........................................".ToArray(),
  };
  // clock
  var clock = new Clock(tick =>
  {
    var localTick = (tick - 1) % 40;
    var localX = x % 41;

    // Bump crt row counter
    if ((tick % 41) - 1 == 0)
    {
      crtRow++;
    }

    // Lit or not?
    if (Math.Abs(localTick - localX) <= 1)
    {
      crt[crtRow][localTick] = '#';
    }
  });
  foreach (var instruction in ParseInstructions(input))
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
  Console.WriteLine("Part 2:");
  foreach (var row in crt)
  {
    Console.WriteLine(new string(row));
  }
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