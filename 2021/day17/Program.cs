using System.Text.RegularExpressions;

// Just brute force part 1.
//var input = "target area: x=20..30, y=-10..-5";

// Part 2 (more educated brute force)
var input = "target area: x=235..259, y=-118..-62";
var target = ParseTarget(input);

var maxY = int.MinValue;
var hitCounter = 0;
for (var dx = 1; dx <= target.BottomRight.X; dx++)
{
  for (var dy = Math.Min(target.TopLeft.Y, target.BottomRight.Y); dy <= Math.Min(target.TopLeft.Y, target.BottomRight.Y) * -1; dy++)
  {
    var result = Shoot(target, dx, dy);
    if (result.Hit)
    {
      maxY = Math.Max(maxY, result.MaxY);
      hitCounter++;
      Console.WriteLine("{0},{1}", dx, dy);
    }
  }
}
Console.WriteLine("Max Y: {0}", maxY);
Console.WriteLine("Hit counter: {0}", hitCounter);

static Result Shoot(Target target, int dx, int dy)
{
  var result = new Result();
  result.MaxY = int.MinValue;

  var probe = new Point(0, 0);
  while (true)
  {
    probe.X += dx;
    probe.Y += dy;

    if (probe.Y > result.MaxY)
    {
      result.MaxY = probe.Y;
    }

    // Adjust dx
    if (dx < 0)
    {
      dx++;
    }
    else if (dx > 0)
    {
      dx--;
    }

    // Adjust dy
    dy--;

    // Exit conditions
    if (target.OnTarget(probe))
    {
      result.Hit = true;
      result.LastPoint = probe;
      return result;
    }
    else if (target.PastTarget(probe))
    {
      result.Hit = false;
      result.LastPoint = probe;
      return result;
    }
  }
}

static Target ParseTarget(string input)
{
  var targetExpression = new Regex(@"^target area: x=(\-?[0-9]+)\.\.(\-?[0-9]+), y=(\-?[0-9]+)\.\.(\-?[0-9]+)$");
  var match = targetExpression.Match(input);
  if (!match.Success)
  {
    throw new Exception("Invalid target pattern: " + input);
  }
  var topLeft = new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[3].Value));
  var bottomRight = new Point(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[4].Value));
  return new Target(topLeft, bottomRight);
}

struct Point
{
  public int X { get; set; }
  public int Y { get; set; }

  public Point(int x, int y)
  {
    X = x;
    Y = y;
  }
}

class Target
{
  public Point TopLeft { get; set; }
  public Point BottomRight { get; set; }

  public Target(Point topLeft, Point bottomRight)
  {
    TopLeft = topLeft;
    BottomRight = bottomRight;
  }

  // Return a flag indicating if a point is part the target i.e.:
  // - x is farther than the right side
  // - y is under the bottom side
  //
  // Assumption: we always shoot right to left.
  public bool PastTarget(Point point)
  {
    return point.X > BottomRight.X || point.Y < Math.Min(TopLeft.Y, BottomRight.Y);
  }

  // Return a flag indicating if a point is on the target.
  public bool OnTarget(Point point)
  {
    return point.X >= TopLeft.X && point.X <= BottomRight.X && point.Y >= TopLeft.Y && point.Y <= BottomRight.Y;
  }
}

struct Result
{
  public bool Hit { get; set; }
  public Point LastPoint { get; set; }
  public int MaxY { get; set; }
}