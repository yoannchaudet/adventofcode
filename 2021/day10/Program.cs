var inputPath = "./inputs/input.txt";
var syntaxScores = new Dictionary<char, int>() {
  { ')', 3 },
  { ']', 57 },
  { '}', 1197 },
  { '>', 25137 },
};
var autocompleteScores = new Dictionary<char, int>() {
  { ')', 1 },
  { ']', 2 },
  { '}', 3 },
  { '>', 4 },
};
var totalSyntaxErrorScore = File.ReadAllLines(inputPath)
  .Select(line => GetFirstIllegalCharacter(line))
  .Where(c => c.HasValue)
  .Select(c => syntaxScores[c.Value])
  .Sum();
Console.WriteLine("Total syntax error score (part 1): {0}", totalSyntaxErrorScore);

var allAutocompleteScores = File.ReadAllLines(inputPath)
  .Select(line => (line, GetFirstIllegalCharacter(line)))
  .Where(c => !c.Item2.HasValue)
  .Select(line => GetAutocompleteScore(line.Item1, autocompleteScores))
  .ToList();
allAutocompleteScores.Sort();
Console.WriteLine("Middle auto complete score (part 2): {0}", allAutocompleteScores[allAutocompleteScores.Count / 2]);

static char? GetFirstIllegalCharacter(string line)
{
  var stack = new Stack<char>();
  foreach (var c in line)
  {
    switch (c)
    {
      case '(':
        stack.Push(')');
        break;
      case '[':
        stack.Push(']');
        break;
      case '{':
        stack.Push('}');
        break;
      case '<':
        stack.Push('>');
        break;

      case ')':
      case ']':
      case '}':
      case '>':
        if (stack.Count == 0)
        {
          // Incomplete line
          return null;
        }

        if (stack.Pop() != c)
        {
          // Return the first illegal character (corrupted)
          return c;
        }
        break;
    }
  }

  // Valid input
  return null;
}

static double GetAutocompleteScore(string line, IDictionary<char, int> autocompleteScores)
{
  var stack = new Stack<char>();
  foreach (var c in line)
  {
    switch (c)
    {
      case '(':
        stack.Push(')');
        break;
      case '[':
        stack.Push(']');
        break;
      case '{':
        stack.Push('}');
        break;
      case '<':
        stack.Push('>');
        break;

      case ')':
      case ']':
      case '}':
      case '>':
        stack.Pop();
        break;
    }
  }

  double score = 0;
  while (stack.Count > 0)
  {
    score *= 5d;
    score += autocompleteScores[stack.Pop()];
  }
  return score;
}