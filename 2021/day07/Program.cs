
var inputPath = "./inputs/input.txt";
var positions = File.ReadAllLines(inputPath).First().Split(',').Select(int.Parse).ToArray();
var min = positions.Min();
var max = positions.Max();
var avg = (int)positions.Average();

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
Console.WriteLine("Minimum fuel: {0}", minimumFuel);

static int GetCost(int[] positions, int horizontalLocation)
{
  return positions.Select(x => Math.Abs(x - horizontalLocation)).Sum();
}