// See https://aka.ms/new-console-template for more information

// TODO: get some nice scaffolding for all this
// TODO: use args?

// Day1
// Run("./inputs/sample.txt");
Run("./inputs/input.txt");

static void Run(string inputPath) {
  // Collect all depths
  var depths = File.ReadAllLines(inputPath)
    .Select(line => int.Parse(line))
    .ToArray();

  // Count how many times the depth increased
  var result = DepthIncreaseCount(depths);
  Console.WriteLine("Part 1 = {0}", result);

  // Count how many times the depth increased for the sums of the sliding windows
  var result2 = DepthIncreaseCount(SlidingWindowsSums(depths));
  Console.WriteLine("Part 2 = {0}", result2);
}

// Return the number of times the depth in a given int array has increased.
static int DepthIncreaseCount(int[] depths) {
  var depthIncreaseCount = 0;
  for (int i = 1; i < depths.Length; i++) {
    if (depths[i - 1] < depths[i]) {
      depthIncreaseCount++;
    }
  }
  return depthIncreaseCount;
}

// Return a new int array containing the sums of the sliding windows of size 3.
static int[] SlidingWindowsSums(int[] depths) {
  // Not enough measurements
  if (depths.Length < 3) {
    return new int[0];
  }

  // Return the sums of the sliding windows
  return Enumerable.Range(0, depths.Length - 2)
    .Select(i => depths[i] + depths[i + 1] + depths[i + 2])
    .ToArray();
}