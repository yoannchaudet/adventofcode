var trees = GetTrees("input.txt");

// Part 1
{
  var visibleTrees = 0;
  visibleTrees += trees.Length * 2; // add left and right trees
  visibleTrees += (trees[0].Length - 2) * 2; // add top and bottom trees
  // count visible trees on the inside
  for (var y = 1; y < trees.Length - 1; y++)
  {
    for (var x = 1; x < trees[0].Length - 1; x++)
    {
      var visible = false;
      visible |= IsVisible(trees, trees[y][x], GetTopRange(trees, Tuple.Create(y, x))); // top
      visible |= IsVisible(trees, trees[y][x], GetBottomRange(trees, Tuple.Create(y, x))); // bottom
      visible |= IsVisible(trees, trees[y][x], GetLeftRange(trees, Tuple.Create(y, x))); // left
      visible |= IsVisible(trees, trees[y][x], GetRightRange(trees, Tuple.Create(y, x))); ; // right
      if (visible)
        visibleTrees++;
    }
  }
  Console.WriteLine("Part 1: {0}", visibleTrees);
}

// Part 2
{
  var maxScenicScore = 0;
  for (var y = 1; y < trees.Length; y++)
  {
    for (var x = 1; x < trees[0].Length; x++)
    {
      var scenicScore = 1;
      scenicScore *= GetVisibleTrees(trees, trees[y][x], GetTopRange(trees, Tuple.Create(y, x))); // top
      scenicScore *= GetVisibleTrees(trees, trees[y][x], GetBottomRange(trees, Tuple.Create(y, x))); // bottom
      scenicScore *= GetVisibleTrees(trees, trees[y][x], GetLeftRange(trees, Tuple.Create(y, x))); // left
      scenicScore *= GetVisibleTrees(trees, trees[y][x], GetRightRange(trees, Tuple.Create(y, x))); // right
      if (scenicScore > maxScenicScore)
        maxScenicScore = scenicScore;
    }
  }
  Console.WriteLine("Part 2: {0}", maxScenicScore);
}

// Check if a tree is visible for a given direction
static bool IsVisible(int[][] trees, int referenceTree, IEnumerable<Tuple<int, int>> range)
{
  var visible = true;
  foreach (var tree in range)
  {
    visible &= referenceTree > trees[tree.Item1][tree.Item2];
  }
  return visible;
}

// Count the number of visible trees in a given direction
static int GetVisibleTrees(int[][] trees, int referenceTree, IEnumerable<Tuple<int, int>> range)
{
  var visibleTrees = 0;
  foreach (var tree in range)
  {
    if (referenceTree > trees[tree.Item1][tree.Item2])
      visibleTrees++;
    else
      return visibleTrees + 1;
  }
  return visibleTrees;
}

// Parse the input
static int[][] GetTrees(string input)
{
  return File.ReadAllLines(input).Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
}

//
// Ranges boring stuff
//

static IEnumerable<Tuple<int, int>> GetTopRange(int[][] grid, Tuple<int, int> start)
{
  for (var y = start.Item1 - 1; y >= 0; y--)
  {
    yield return Tuple.Create(y, start.Item2);
  }
}

static IEnumerable<Tuple<int, int>> GetBottomRange(int[][] grid, Tuple<int, int> start)
{
  for (var y = start.Item1 + 1; y < grid.Length; y++)
  {
    yield return Tuple.Create(y, start.Item2);
  }
}


static IEnumerable<Tuple<int, int>> GetLeftRange(int[][] grid, Tuple<int, int> start)
{
  for (var x = start.Item2 - 1; x >= 0; x--)
  {
    yield return Tuple.Create(start.Item1, x);
  }
}

static IEnumerable<Tuple<int, int>> GetRightRange(int[][] grid, Tuple<int, int> start)
{
  for (var x = start.Item2 + 1; x < grid[0].Length; x++)
  {
    yield return Tuple.Create(start.Item1, x);
  }
}
