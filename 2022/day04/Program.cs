var input = "input.txt";
var pairs = GetInput(input);

// Part 1
{
  var result = pairs.Where(pair =>
     Includes(pair.Item1, pair.Item2) || Includes(pair.Item2, pair.Item1)
  ).Count();
  Console.WriteLine("Part 1: {0}", result);
}

// Part 2
{
  var result = pairs.Where(pair =>
    Includes(pair.Item1, pair.Item2) || Includes(pair.Item2, pair.Item1)
    || Overlaps(pair.Item1, pair.Item2) || Overlaps(pair.Item2, pair.Item1)
  ).Count();
  Console.WriteLine("Part 2: {0}", result);
}

// Return a boolean indicating if a includes b
static bool Includes(Range a, Range b)
{
  return b.Start.Value >= a.Start.Value && b.End.Value <= a.End.Value;
}

// Return a boolean indicating if a and b overlaps
static bool Overlaps(Range a, Range b)
{
  return b.Start.Value <= a.End.Value && a.End.Value <= b.End.Value;
}

// Read the input
static List<Tuple<Range, Range>> GetInput(string input)
{
  return File.ReadAllLines(input).Select(line =>
  {
    var parts = line.Split(",");
    var range1 = parts[0].Split("-").Select(int.Parse).ToArray();
    var range2 = parts[1].Split("-").Select(int.Parse).ToArray();
    return new Tuple<Range, Range>(new Range(range1[0], range1[1]), new Range(range2[0], range2[1]));
  }).ToList();
}