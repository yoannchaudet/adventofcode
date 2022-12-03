var rucksacks = GetRucksacks("input.txt");
var part1 = rucksacks.Select(r => r.GetMisplacedItems().Select(item => GetPriority(item)).Sum()).Sum();
Console.WriteLine("Part 1: {0}", part1);

static List<Rucksack> GetRucksacks(string input)
{
  return File.ReadAllLines(input).Select(line => new Rucksack(line)).ToList();
}

static int GetPriority(char c)
{
  if (c >= 'a' && c <= 'z') return c - 'a' + 1;
  if (c >= 'A' && c <= 'Z') return c - 'A' + 27;
  return 0;
}

class Rucksack
{
  Dictionary<char, int> Compartment1 { get; }
  Dictionary<char, int> Compartment2 { get; }

  public List<char> GetMisplacedItems()
  {
    var first = Compartment1.Keys.Where(key => Compartment2.ContainsKey(key)).ToList();
    var second = Compartment2.Keys.Where(key => Compartment1.ContainsKey(key)).ToList();
    return first.Union(second).Distinct().ToList();
  }

  public Rucksack(string input)
  {
    // Init
    Compartment1 = new Dictionary<char, int>();
    Compartment2 = new Dictionary<char, int>();

    // Validation
    var halfWay = input.Length / 2;
    if (input.Length % 2 == 1)
    {
      throw new Exception("Oups, even rucksack");
    }

    // Init compartments
    for (var i = 0; i < input.Length; i++)
    {
      var compartment = i + 1 <= halfWay ? Compartment1 : Compartment2;
      if (compartment.ContainsKey(input[i]))
      {
        compartment[input[i]]++;
      }
      else
      {
        compartment.Add(input[i], 1);
      }
    }
  }
}
