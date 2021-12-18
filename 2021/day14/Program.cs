using System.Text;

var inputPath = "./inputs/input.txt";
var (template, pairs) = ReadInput(inputPath);
for (var step = 1; step <= 10; step++)
{
  template = PerformStep(template, pairs);
}

// Get most and least common elements
var elements = template.GroupBy(c => c).OrderByDescending(g => g.Count());
var mostCommon = elements.First();
var leastCommon = elements.Last();
var part1 = mostCommon.Count() - leastCommon.Count();
Console.WriteLine("Part 1: {0}", part1);

static string PerformStep(string chain, IDictionary<string, string> pairs)
{
  var newChain = new StringBuilder();
  for (int i = 0; i < chain.Length; i++)
  {
    if (i < chain.Length - 1)
    {
      var pair = chain.Substring(i, 2);
      if (pairs.ContainsKey(pair))
      {
        newChain.Append(chain[i]);
        newChain.Append(pairs[pair]);
        continue;
      }
    }
    newChain.Append(chain[i]);
  }
  return newChain.ToString();
}

static (string, IDictionary<string, string>) ReadInput(string inputPath)
{
  var template = "";
  var pairs = new Dictionary<string, string>();
  foreach (var line in File.ReadAllLines(inputPath))
  {
    if (template == "")
    {
      template = line;
    }
    else if (line != "")
    {
      var parts = line.Split("->");
      pairs.Add(parts[0].Trim(), parts[1].Trim());
    }
  }
  return (template, pairs);
}