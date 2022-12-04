var input = "input.txt";
var pairs = GetInput(input);

// Part 1
{
  var result = pairs.Where(pair =>
  {
    // item1 contains item2
    return (pair.Item2.Start.Value >= pair.Item1.Start.Value && pair.Item2.End.Value <= pair.Item1.End.Value)
    ||
    // item2 contains item1
    (pair.Item1.Start.Value >= pair.Item2.Start.Value && pair.Item1.End.Value <= pair.Item2.End.Value);
  }).Count();
  Console.WriteLine("Part 1: {0}", result);
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