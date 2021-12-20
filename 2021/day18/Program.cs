var number = new SnailNumberParser("[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]").Parse();
Console.WriteLine(number.ToString());

class SnailNumberParser
{
  private string Number;
  private int Position;

  public SnailNumberParser(string number)
  {
    this.Number = number;
    this.Position = 0;
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
    SnailNumber currentNumber = null;
    while (Position < Number.Length)
    {
      if (Number[Position] == '[')
      {
        Position++;
        currentNumber = new SnailNumber();
        currentNumber.Left = Parse();
      }
      else if (char.IsDigit(Number[Position]))
      {
        return new SnailNumber(ReadLiteral());
      }
      else if (Number[Position] == ',')
      {
        if (currentNumber == null)
        {
          throw new Exception("Unexpected comma");
        }
        Position++;
        currentNumber.Right = Parse();
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
  public int? Number { get; set; }
  public SnailNumber? Left { get; set; }
  public SnailNumber? Right { get; set; }

  public SnailNumber()
  {

  }

  public SnailNumber(int number)
  {
    Number = number;
  }

  public SnailNumber(SnailNumber left, SnailNumber right)
  {
    Left = left;
    Right = right;
  }

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
}
