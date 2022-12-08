var trees = GetTrees("input.txt");

// Part 1
{
  var visibleTrees = 0;
  visibleTrees += trees.Length * 2; // add left and right trees
  visibleTrees += (trees[0].Length - 2) * 2; // add top and bottom trees
  // Count visible trees on the inside
  for (var y = 1; y < trees.Length - 1; y++)
  {
    for (var x = 1; x < trees[0].Length - 1; x++)
    {
      var visible = false;
      visible |= IsVisible(trees, y, x, 0, y - 1, x, x); // top
      visible |= IsVisible(trees, y, x, y + 1, trees.Length - 1, x, x); // bottom
      visible |= IsVisible(trees, y, x, y, y, 0, x - 1); // left
      visible |= IsVisible(trees, y, x, y, y, x + 1, trees[0].Length - 1); // right
      if (visible)
        visibleTrees++;
    }
  }
  Console.WriteLine("Part 1: {0}", visibleTrees);
}

// Part 2
{

}

// Check if a tree is visible for a given direction
static bool IsVisible(int[][] trees, int y, int x, int dy, int dyy, int dx, int dxx)
{
  var visible = true;
  for (var yy = dy; yy <= dyy; yy++)
    for (var xx = dx; xx <= dxx; xx++)
      visible &= trees[y][x] > trees[yy][xx];
  return visible;
}

// Parse the input
static int[][] GetTrees(string input)
{
  return File.ReadAllLines(input).Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
}