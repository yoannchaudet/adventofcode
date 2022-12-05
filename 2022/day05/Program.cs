using System.Text.RegularExpressions;

// Read the input
var input = "input.txt";
List<Stack<char>> stacks;
List<Instruction> instructions;

// Part 1
{
  ReadInput(input, out stacks, out instructions);

  // Process the instructions
  foreach (var instruction in instructions)
  {
    for (var i = 0; i < instruction.Count; i++)
    {
      var label = stacks[instruction.From - 1].Pop();
      stacks[instruction.To - 1].Push(label);
    }
  }

  // Get the labels at the top of each stack
  var labels = "";
  foreach (var stack in stacks)
  {
    labels += stack.Peek();
  }
  Console.WriteLine("Part 1: {0}", labels);
}

// Part 2
{
  ReadInput(input, out stacks, out instructions);

  // Process the instructions
  foreach (var instruction in instructions)
  {
    // Just keep a buffer to push the crate in the other direction
    var tempCrates = "";
    for (var i = 0; i < instruction.Count; i++)
    {
      tempCrates += stacks[instruction.From - 1].Pop();
    }
    foreach (var label in tempCrates.Reverse())
    {
      stacks[instruction.To - 1].Push(label);
    }
  }

  // Get the labels at the top of each stack
  var labels = "";
  foreach (var stack in stacks)
  {
    labels += stack.Peek();
  }
  Console.WriteLine("Part 2: {0}", labels);
}

// Parse the input
static void ReadInput(string input, out List<Stack<char>> stacks, out List<Instruction> instructions)
{
  // Initial state
  bool buildStacks = true;

  // Raw stacks data
  var rawStacks = new Dictionary<int, string>();

  // Init instructions
  instructions = new List<Instruction>();
  var instructionRegex = new Regex(@"^move ([0-9]+) from ([0-9]+) to ([0-9]+)$");

  // Go through each lines
  foreach (var line in File.ReadAllLines(input))
  {
    // Build stacks
    if (buildStacks)
    {
      for (var i = 0; i < line.Length; i++)
      {
        if (line[i] == '[')
        {
          var label = line[i + 1];
          var index = (int)((i + 1) / 4);
          if (rawStacks.ContainsKey(index))
          {
            rawStacks[index] += label;
          }
          else
          {
            rawStacks[index] = label.ToString();
          }
        }
      }
    }

    // Parse instructions
    else if (!String.IsNullOrEmpty(line))
    {
      var match = instructionRegex.Match(line);
      if (!match.Success)
      {
        throw new Exception("unexpected instruction: " + line);
      }
      else
      {
        instructions.Add(new Instruction()
        {
          Count = int.Parse(match.Groups[1].Value),
          From = int.Parse(match.Groups[2].Value),
          To = int.Parse(match.Groups[3].Value)
        });
      }
    }

    // Switch to instructions
    if (line.StartsWith(" 1"))
    {
      buildStacks = false;
      continue;
    }
  }

  // Build the stacks
  stacks = new List<Stack<char>>();
  for (var stackIndex = 0; stackIndex < rawStacks.Count; stackIndex++)
  {
    var stack = new Stack<char>();
    foreach (var label in rawStacks[stackIndex].Reverse())
    {
      stack.Push(label);
    }
    stacks.Add(stack);
  }
}

struct Instruction
{
  public int Count;
  public int From;
  public int To;
}