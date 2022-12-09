// Part 1
var moves = GetMoves("input.txt");
var head = new Point();
var tail = new Point();
var uniqueTailPositions = new List<Point>();
uniqueTailPositions.Add(tail);
foreach (var move in moves)
{
  Console.WriteLine(move.ToString());
  for (var i = 1; i <= move.Count; i++)
  {
    head.MoveHead(move.Direction);
    tail.MoveTail(head);
    if (!uniqueTailPositions.Contains(tail))
    {
      uniqueTailPositions.Add(tail);
    }
  }
}
Console.WriteLine("Part 1: {0}", uniqueTailPositions.Count);

static List<Move> GetMoves(string input)
{
  var moves = new List<Move>();
  foreach (var line in File.ReadAllLines(input))
  {
    var parts = line.Split(" ");
    moves.Add(new Move()
    {
      Direction = ParseDirection(parts[0]),
      Count = int.Parse(parts[1])
    });
  }
  return moves;
}

static Direction ParseDirection(string input)
{
  switch (input)
  {
    case "U":
      return Direction.Up;
    case "D":
      return Direction.Down;
    case "L":
      return Direction.Left;
    case "R":
      return Direction.Right;
    default:
      throw new ArgumentException("Invalid direction");
  }
}

enum Direction
{
  Up, Down,
  Left, Right,
}

struct Move
{
  public Direction Direction { get; set; }
  public int Count { get; set; }

  public string ToString()
  {
    return $"{Direction} {Count}";
  }
}

struct Point
{
  public Point(int x = 0, int y = 0)
  {
    X = x;
    Y = y;
  }

  public int X { get; private set; }
  public int Y { get; private set; }

  public void MoveHead(Direction direction)
  {
    switch (direction)
    {
      case Direction.Up:
        Y++;
        break;
      case Direction.Down:
        Y--;
        break;
      case Direction.Left:
        X--;
        break;
      case Direction.Right:
        X++;
        break;
    }
  }

  public Tuple<int, int> GetDistance(Point other)
  {
    return new Tuple<int, int>(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
  }


  public void MoveTail(Point head)
  {
    var distance = GetDistance(head);

    // Same row, not touching
    if (X == head.X && distance.Item2 > 1)
    {
      Y = Y > head.Y ? Y - 1 : Y + 1;
    }

    // Same column, not touching
    else if (Y == head.Y && distance.Item1 > 1)
    {
      X = X > head.X ? X - 1 : X + 1;
    }

    // Diagonal, not touching
    else if (X != head.X && Y != head.Y && (distance.Item1 > 1 || distance.Item2 > 1))
    {
      X = X > head.X ? X - 1 : X + 1;
      Y = Y > head.Y ? Y - 1 : Y + 1;
    }
  }
}