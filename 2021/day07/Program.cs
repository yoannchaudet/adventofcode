
var inputPath = "./inputs/input.txt";
var positions = File.ReadAllLines(inputPath).First().Split(',').Select(int.Parse).ToArray();
var min = positions.Min();
var max = positions.Max();
var avg = (int)positions.Average();

// Part 1
// Start at the average and find local minimum in both directions (go through half the dataset).

var hlTop = new List<int>();
for (var horizontalLocation = avg; horizontalLocation <= (max / 2); horizontalLocation++)
{
  hlTop.Add(GetCost(positions, horizontalLocation));
}

var hlBottom = new List<int>();
for (var horizontalLocation = avg; horizontalLocation >= (min / 2); horizontalLocation--)
{
  hlBottom.Add(GetCost(positions, horizontalLocation));
}

var minimumFuel = Math.Min(hlTop.Min(), hlBottom.Min());
Console.WriteLine("Minimum fuel (part 1): {0}", minimumFuel);

hlTop = new List<int>();
for (var horizontalLocation = avg; horizontalLocation <= (max / 2); horizontalLocation++)
{
  hlTop.Add(GetCost2(positions, horizontalLocation));
}

hlBottom = new List<int>();
for (var horizontalLocation = avg; horizontalLocation >= (min / 2); horizontalLocation--)
{
  hlBottom.Add(GetCost2(positions, horizontalLocation));
}

minimumFuel = Math.Min(hlTop.Min(), hlBottom.Min());
Console.WriteLine("Minimum fuel (part 2): {0}", minimumFuel);

// Simple cost function (part 1)
static int GetCost(int[] positions, int horizontalLocation)
{
  return positions.Select(x => Math.Abs(x - horizontalLocation)).Sum();
}

// Complex cost function (part 2)
static int GetCost2(int[] positions, int horizontalLocation)
{
  return positions.Select(x => GetDistanceCost(Math.Abs(x - horizontalLocation))).Sum();
}

// Cost for a given traveled distance (part 2)
static int GetDistanceCost(int distance) {
  var cost = 0;
  for (int i = 1; i <= distance; i++) {
    cost += i;
  }
  return cost;
}