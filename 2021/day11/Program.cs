var inputPath = "./inputs/input.txt";
var grid = GetGrid(inputPath);

var flashes = 0;
for (var step = 1; step <= 100; step++)
{
  flashes += ProcessOneStep(ref grid);
}
Console.WriteLine("Flashes after 100 steps: {0}", flashes);

// Perform one step and return the number of octopuses that flashed.
static int ProcessOneStep(ref int[][] grid)
{
  // Increment all energy levels
  for (int y = 0; y < grid.Length; y++)
  {
    for (int x = 0; x < grid[y].Length; x++)
    {
      grid[y][x]++;
    }
  }

  // Flash all octopuses that needs it
  int flashes = 0;
  while (true)
  {
    var octopusesToFlash = GetOctopusToFlash(ref grid);
    foreach (var octopus in octopusesToFlash)
    {
      flashes++;
      FlashOctopus(octopus.Item2, octopus.Item1, ref grid);
    }
    if (octopusesToFlash.Count == 0)
    {
      break;
    }
  }

  // Reset to 0, flashed octopuses
  for (int y = 0; y < grid.Length; y++)
  {
    for (int x = 0; x < grid[y].Length; x++)
    {
      if (grid[y][x] == -1)
      {
        grid[y][x] = 0;
      }
    }
  }

  return flashes;
}

// Make an octopus flash
static void FlashOctopus(int x, int y, ref int[][] grid)
{
  // Mark the octopus as flashed
  grid[y][x] = -1;

  // Increment adjacent octopuses
  foreach (var adjacentLocation in GetAdjacentOctopus(x, y))
  {
    if (grid[adjacentLocation.Item1][adjacentLocation.Item2] > 0)
    {
      grid[adjacentLocation.Item1][adjacentLocation.Item2]++;
    }
  }
}

// Return the location of the adjacent octopuses (including diagonals) to a given position.
static List<(int, int)> GetAdjacentOctopus(int x, int y)
{
  // 000
  // 0.0
  // 000

  var adj = new List<(int, int)>();

  // Left, Right, Up, Down
  if (x > 0)
    adj.Add((y, x - 1));
  if (x < 9)
    adj.Add((y, x + 1));
  if (y > 0)
    adj.Add((y - 1, x));
  if (y < 9)
    adj.Add((y + 1, x));

  // Diagonals
  if (x > 0 && y > 0)
    adj.Add((y - 1, x - 1));
  if (x < 9 && y > 0)
    adj.Add((y - 1, x + 1));
  if (x > 0 && y < 9)
    adj.Add((y + 1, x - 1));
  if (x < 9 && y < 9)
    adj.Add((y + 1, x + 1));

  // Return the list
  return adj;
}

// Return the list of octopus locations that need to flash.
static List<(int, int)> GetOctopusToFlash(ref int[][] grid)
{
  var toFlash = new List<(int, int)>();
  for (int y = 0; y < grid.Length; y++)
  {
    for (int x = 0; x < grid[y].Length; x++)
    {
      if (grid[y][x] > 9)
      {
        toFlash.Add((y, x));
      }
    }
  }
  return toFlash;
}

// Parse the grid from an input file.
static int[][] GetGrid(string inputPath)
{
  var lines = File.ReadAllLines(inputPath);
  var grid = new int[lines.Length][];
  for (int i = 0; i < lines.Length; i++)
  {
    grid[i] = lines[i].Select(c => int.Parse(c.ToString())).ToArray();
  }
  return grid;
}