using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Text;

var input = "input.txt";

// Part 1
{
  var monkeys = GetMonkeys(input).ToList();
  var inspections = new int[monkeys.Count];

  // Run 20 rounds
  for (var round = 1; round <= 20; round++)
  {
    for (var i = 0; i < monkeys.Count; i++)
    {
      var monkey = monkeys[i];

      // Go through the items the monkey has now
      foreach (var item in monkey.Items)
      {
        var worryLevel = monkey.Operation.Invoke(item);
        worryLevel = (int)Math.Floor(worryLevel / 3d);
        if (worryLevel % monkey.Diviser == 0)
        {
          monkeys[monkey.DivisibleMonkey].Items.Add(worryLevel);
        }
        else
        {
          monkeys[monkey.NonDivisibleMonkey].Items.Add(worryLevel);
        }

        // Increment inspection counter
        inspections[i]++;
      }

      // Clear current list
      monkey.Items.Clear();
    }
  }

  // Compute monkey business
  var top = inspections.OrderByDescending(m => m).Take(2);
  var monkeyBusiness = top.First() * top.Last();
  Console.WriteLine("Part 1: {0}", monkeyBusiness);
}

// Return the monkeys from the input
static IEnumerable<Monkey> GetMonkeys(string input)
{
  var lines = File.ReadAllLines(input);
  var operationRegex = new Regex(@"  Operation: new = (.+)&");
  for (var i = 0; i < lines.Length; i++)
  {
    var line = lines[i];
    if (line.StartsWith("Monkey"))
    {
      // ugly parsing
      var items = lines[++i].Substring("  Starting items:".Length).Split(",").Select(long.Parse).ToList();
      var rawOperation = lines[++i].Substring("  Operation: new =".Length);
      var operation = CSharpScript.EvaluateAsync<Func<long, long>>("old => " + rawOperation).GetAwaiter().GetResult();
      var diviser = int.Parse(lines[++i].Substring("  Test: divisible by ".Length));
      var divisibleMonkey = int.Parse(lines[++i].Substring("    If true: throw to monkey ".Length));
      var nonDivisibleMonkey = int.Parse(lines[++i].Substring("    If false: throw to monkey ".Length));

      // return the monkey
      yield return new Monkey()
      {
        Items = items,
        Operation = operation,
        Diviser = diviser,
        DivisibleMonkey = divisibleMonkey,
        NonDivisibleMonkey = nonDivisibleMonkey,
      };
    }
  }
}

class Monkey
{
  public List<long> Items { get; set; }
  public Func<long, long> Operation { get; set; }
  public long Diviser { get; set; }
  public int DivisibleMonkey { get; set; }
  public int NonDivisibleMonkey { get; set; }
}
