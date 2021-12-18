var inputPath = "./inputs/input.txt";
var (template, transitions) = ReadInput(inputPath);
var maxStep = 40;

// Solving the sample without optimization requires 3,298 GB of RAM just for the string
// or 1,099 GB of RAM if we breakdown the problem  into 2-char strings.
// The puzzle input would need 10TB (without optimization).
// var size = (double) 4;
// for (var step = 0 ; step <= 40; step++) {
//   Console.WriteLine("Step {0}: size = {1}", step, size);
//   size = size + size -1;
// }

// Count elements in the template
var elements = new Dictionary<string, long>();
foreach (var c in template)
{
  IncrementElement(c.ToString(), elements);
}

// Start the iteration
var cache = new Dictionary<(string, int), IDictionary<string, long>>();
for (var i = 0; i < template.Length - 1; i++)
{
  CountElements(template.Substring(i, 2), transitions, elements, maxStep, 1, cache);
}

var orderedElements = elements.OrderByDescending(x => x.Value).ToList();
var mostCommon = orderedElements.First();
var leastCommon = orderedElements.Last();
var result = mostCommon.Value - leastCommon.Value;
Console.WriteLine("Most/Least common difference at step {0}: {1}", maxStep, result);

static void IncrementElement(string element, IDictionary<string, long> elements, long value = 1)
{
  if (elements.ContainsKey(element))
  {
    elements[element] += value;
  }
  else
  {
    elements[element] = value;
  }
}

static void CombineElements(IDictionary<string, long> target, IDictionary<string, long> source)
{
  foreach (var element in source)
    IncrementElement(element.Key, target, source[element.Key]);
}

static void CountElements(string template, IDictionary<string, string> transitions, IDictionary<string, long> elements, int maxStep, int step, IDictionary<(string, int), IDictionary<string, long>> cache)
{
  // Console.WriteLine("step " + step + " - " + template);
  if (transitions.ContainsKey(template))
  {
    var newElement = transitions[template];
    IncrementElement(newElement, elements);

    if (step < maxStep)
    {
      if (cache.ContainsKey((template, step)))
      {
        CombineElements(elements, cache[(template, step)]);
        return;
      }

      var elements1 = new Dictionary<string, long>();
      var elements2 = new Dictionary<string, long>();

      CountElements(template.Substring(0, 1) + newElement, transitions, elements1, maxStep, step + 1, cache);
      CountElements(newElement + template.Substring(1, 1), transitions, elements2, maxStep, step + 1, cache);

      var cachedElements = new Dictionary<string, long>();
      CombineElements(cachedElements, elements1);
      CombineElements(cachedElements, elements2);
      cache.Add((template, step), cachedElements);
      CombineElements(elements, cachedElements);
    }
  }
}

static (string, IDictionary<string, string>) ReadInput(string inputPath)
{
  var template = "";
  var transitions = new Dictionary<string, string>();
  foreach (var line in File.ReadAllLines(inputPath))
  {
    if (template == "")
    {
      template = line;
    }
    else if (line != "")
    {
      var parts = line.Split("->");
      transitions.Add(parts[0].Trim(), parts[1].Trim());
    }
  }
  return (template, transitions);
}