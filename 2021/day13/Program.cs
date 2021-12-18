using System.Text.RegularExpressions;

var inputPath = "./inputs/input.txt";
var (transparentPaper, folds) = GetTransparentPaper(inputPath);

// Do the first fold and count the dots
var firstFold = Fold(transparentPaper, folds.First());
var dots = firstFold.Select(y => y.Count(x => x == true)).Sum();
Console.WriteLine("Dots after first fold (part 1): {0}", dots);

// Do all folds
var currentPaper = firstFold;
foreach (var fold in folds.Skip(1))
{
  currentPaper = Fold(currentPaper, fold);
}

// Print the result
Console.WriteLine("Part 2:");
for (var y = 0 ; y < currentPaper.Length ; y++)
{
  for (var x = 0 ; x < currentPaper[y].Length ; x++)
  {
    Console.Write(currentPaper[y][x] ? '#' : '.');
  }
  Console.WriteLine();
}

// Perform one fold and return the new transparent paper
static bool[][] Fold(bool[][] paper, Fold fold)
{
  var maxX = fold.X.HasValue ? fold.X.Value : paper[0].Length;
  var maxY = fold.Y.HasValue ? fold.Y.Value : paper.Length;
  var newPaper = new bool[maxY][];
  for (var y = 0; y < maxY; y++)
  {
    newPaper[y] = new bool[maxX];
  }

  for (var y = 0; y < maxY; y++)
  {
    for (var x = 0; x < maxX; x++)
    {
      // Copy part that does not move
      newPaper[y][x] = paper[y][x];

      // Copy fold
      if (fold.Y.HasValue)
      {
        var projection = fold.Y.Value + (fold.Y.Value - y);
        if (projection >= 0 && projection < paper.Length)
        {
          newPaper[y][x] |= paper[projection][x];
        }
      }
      else if (fold.X.HasValue)
      {
        var projection = fold.X.Value + (fold.X.Value - x);
        if (projection >= 0 && projection < paper[0].Length)
        {
          newPaper[y][x] |= paper[y][projection];
        }
      }
    }
  }

  // Return the folded transparent paper
  return newPaper;
}

// Parse the input
static (bool[][], List<Fold>) GetTransparentPaper(string fileInput)
{
  var points = new List<int[]>();
  var folds = new List<Fold>();

  var foldPattern = new Regex(@"^fold along (x|y)=(\d+)$");
  var parsePoints = true;
  foreach (var line in File.ReadAllLines(fileInput))
  {
    // Switch to parsing folds
    if (line.Trim() == "")
    {
      parsePoints = false;
      continue;
    }

    if (parsePoints)
    {
      points.Add(line.Split(',').Select(int.Parse).ToArray());
    }
    else
    {
      var match = foldPattern.Match(line);
      if (!match.Success)
      {
        Console.WriteLine("Invalid fold: {0}", line);
        continue;
      }
      switch (match.Groups[1].Value)
      {
        case "x":
          folds.Add(new Fold(int.Parse(match.Groups[2].Value), null));
          break;
        case "y":
          folds.Add(new Fold(null, int.Parse(match.Groups[2].Value)));
          break;
      }
    }
  }

  var maxX = points.Max(p => p[0]);
  var maxY = points.Max(p => p[1]);
  var pointsMatrix = new bool[maxY + 1][];
  for (var y = 0; y < maxY + 1; y++)
  {
    pointsMatrix[y] = new bool[maxX + 1];
  }
  foreach (var point in points)
  {
    pointsMatrix[point[1]][point[0]] = true;
  }
  return (pointsMatrix, folds);
}

struct Fold
{
  public int? X;
  public int? Y;

  public Fold(int? x, int? y)
  {
    X = x;
    Y = y;
  }
}