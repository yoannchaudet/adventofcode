class Day01 {

  public static void Main(string inputPath) {
    // Collect all depths
    var depths = File.ReadAllLines(inputPath)
      .Select(line => int.Parse(line))
      .ToArray();

    // Count how many time the depth increased
    var depthIncreaseCount = 0;
    for (int i = 1; i < depths.Length; i++) {
      if (depths[i - 1] < depths[i]) {
        depthIncreaseCount++;
      }
    }

    Console.WriteLine("Measurements are larger than the previous measurement = {0}", depthIncreaseCount);
  }

}