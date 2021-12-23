var inputPath = "./inputs/input.txt";
Part1(inputPath);
Part2(inputPath);

static void Part1(string inputPath)
{
  SnailNumber sum = null;
  File.ReadAllLines(inputPath).ToList().ForEach(line =>
  {
    var number = new SnailNumberParser(line).Parse();
    if (sum == null)
    {
      sum = number;
    }
    else
    {
      sum += number;
    }
  });
  Console.WriteLine("Part 1");
  Console.WriteLine("Sum: {0}", sum.ToString());
  Console.WriteLine("Magnitude: {0}", sum.GetMagnitude());
}

static void Part2(string inputPath)
{
  var numbers = File.ReadAllLines(inputPath);
  var maxMagnitude = int.MinValue;
  foreach (var a in numbers)
  {
    foreach (var b in numbers)
    {
      var pa = new SnailNumberParser(a).Parse();
      var pb = new SnailNumberParser(b).Parse();
      var magnitude = (pa + pb).GetMagnitude();
      if (magnitude > maxMagnitude)
      {
        maxMagnitude = magnitude;
      }
    }
  }
  Console.WriteLine("Part 2");
  Console.WriteLine("Max magnitude: {0}", maxMagnitude);
}

class SnailNumberParser
{
  private string Number;
  private int Position;
  private IList<SnailNumber> Numbers;

  public SnailNumberParser(string number)
  {
    this.Number = number;
    this.Position = 0;
    this.Numbers = new List<SnailNumber>();
  }

  private int ReadLiteral()
  {
    string literal = "";
    while (Position < Number.Length && char.IsDigit(Number[Position]))
    {
      literal += Number[Position];
      Position++;
    }
    return int.Parse(literal);
  }

  public SnailNumber Parse()
  {
    var number = Parse(null);

    // Init most left/right
    for (var i = 0; i < Numbers.Count; i++)
    {
      if (i > 0)
        Numbers[i].MostLeftNumber = Numbers[i - 1];
      if (i < Numbers.Count - 1)
        Numbers[i].MostRightNumber = Numbers[i + 1];
    }

    return number;
  }

  private SnailNumber Parse(SnailNumber parent)
  {
    SnailNumber currentNumber = null;
    while (Position < Number.Length)
    {
      if (Number[Position] == '[')
      {
        Position++;
        currentNumber = new SnailNumber(parent);
        currentNumber.Left = Parse(currentNumber);
      }
      else if (char.IsDigit(Number[Position]))
      {
        var n = new SnailNumber(parent, ReadLiteral());
        Numbers.Add(n);
        return n;
      }
      else if (Number[Position] == ',')
      {
        if (currentNumber == null)
        {
          throw new Exception("Unexpected comma");
        }
        Position++;
        currentNumber.Right = Parse(currentNumber);
      }
      else if (Number[Position] == ']')
      {
        if (currentNumber == null)
        {
          throw new Exception("Unexpected ]");
        }
        Position++;
        return currentNumber;
      }
      else
      {
        throw new Exception("Unexpected character");
      }
    }

    throw new Exception("Unexpected end of number");
  }
}

class SnailNumber
{
  public SnailNumber? Parent { get; private set; }
  public int? Number { get; set; }
  public SnailNumber? Left { get; set; }
  public SnailNumber? Right { get; set; }

  public SnailNumber? MostLeftNumber { get; set; }
  public SnailNumber? MostRightNumber { get; set; }

  public SnailNumber(SnailNumber parent)
  {
    Parent = parent;
  }

  public SnailNumber(SnailNumber parent, int number)
  {
    Parent = parent;
    Number = number;
  }

  public SnailNumber(SnailNumber parent, SnailNumber left, SnailNumber right)
  {
    Parent = parent;
    Left = left;
    Right = right;
  }

  // Addition operation
  public static SnailNumber operator +(SnailNumber a, SnailNumber b)
  {
    // NOTE: this is a quick and dirty solution. It cannot adds up the same instance twice and
    // both Explode and Split don't maintain the most left/right numbers.
    // Extra parsing is done at each mutation to reset the whole thing.

    var addition = new SnailNumber(null, a, b);
    addition = new SnailNumberParser(addition.ToString()).Parse();

    // Reduction logic
    while (true)
    {
      // If a pair is nested inside four pairs, explode it
      var pairAt4 = addition.GetPairAtDepth(4);
      if (pairAt4 != null)
      {
        pairAt4.Explode();
        addition = new SnailNumberParser(addition.ToString()).Parse();
        continue;
      }

      // If a number is 10 or greater, split it
      var number10OrGreater = addition.GetNumbers().Where(n => n.Number >= 10).FirstOrDefault((SnailNumber)null);
      if (number10OrGreater != null)
      {
        number10OrGreater.Split();
        addition = new SnailNumberParser(addition.ToString()).Parse();
        continue;
      }

      // Done
      break;
    }

    return addition;
  }

  // Return the number's string representation (again)
  public string ToString()
  {
    if (Number.HasValue)
    {
      return Number.Value.ToString();
    }
    else
    {
      return "[" + Left.ToString() + "," + Right.ToString() + "]";
    }
  }

  // Return a pair at a given depth if any.
  public SnailNumber GetPairAtDepth(int depth)
  {
    // Ignore numbers
    if (Number.HasValue)
    {
      return null;
    }

    // Final depth for a pair
    if (depth == 0)
    {
      return this;
    }

    // Recursion for a pair
    else
    {
      // Descent left first
      var left = Left.GetPairAtDepth(depth - 1);
      if (left != null)
      {
        return left;
      }

      // Then right
      var right = Right.GetPairAtDepth(depth - 1);
      if (right != null)
      {
        return right;
      }
    }

    // Fallback
    return null;
  }

  public List<SnailNumber> GetNumbers()
  {
    var numbers = new List<SnailNumber>();
    if (Number.HasValue)
    {
      numbers.Add(this);
    }
    if (Left != null)
    {
      numbers.AddRange(Left.GetNumbers());
    }
    if (Right != null)
    {
      numbers.AddRange(Right.GetNumbers());
    }
    return numbers;
  }

  // Expode a number.
  public void Explode()
  {
    if (Number.HasValue)
    {
      throw new Exception("Cannot explode a number");
    }

    // Do the additions
    if (Left.MostLeftNumber != null)
    {
      Left.MostLeftNumber.Number += Left.Number;
    }
    if (Right.MostRightNumber != null)
    {
      Right.MostRightNumber.Number += Right.Number;
    }

    // Remove the pair
    Left = null;
    Right = null;
    Number = 0;
  }

  // Split a pair.
  public void Split()
  {
    if (!Number.HasValue)
    {
      throw new Exception("Cannot split a pair");
    }

    var left = (int)Math.Floor(Number.Value / 2d);
    var right = Number.Value - left;

    Number = null;
    Left = new SnailNumber(this, left);
    Right = new SnailNumber(this, right);
  }

  // Compute the magnitude of the snail number.
  public int GetMagnitude()
  {
    if (Number.HasValue)
    {
      return Number.Value;
    }
    return 3 * Left.GetMagnitude() + 2 * Right.GetMagnitude();
  }
}
