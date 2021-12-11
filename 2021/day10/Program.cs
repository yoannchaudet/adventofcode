var inputPath = "./inputs/input.txt";
var syntaxScores = new Dictionary<char, int>() {
  { ')', 3 },
  { ']', 57 },
  { '}', 1197 },
  { '>', 25137 },
};
var totalSyntaxErrorScore = File.ReadAllLines(inputPath)
  .Select(line => GetFirstIllegalCharacter(line))
  .Where(c => c.HasValue)
  .Select(c => syntaxScores[c.Value])
  .Sum();
Console.WriteLine("Total syntax error score: {0}", totalSyntaxErrorScore);

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