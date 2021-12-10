//   0:      1:      2:      3:      4:
//  aaaa    ....    aaaa    aaaa    ....
// b    c  .    c  .    c  .    c  b    c
// b    c  .    c  .    c  .    c  b    c
//  ....    ....    dddd    dddd    dddd
// e    f  .    f  e    .  .    f  .    f
// e    f  .    f  e    .  .    f  .    f
//  gggg    ....    gggg    gggg    ....

//   5:      6:      7:      8:      9:
//  aaaa    aaaa    aaaa    aaaa    aaaa
// b    .  b    .  .    c  b    c  b    c
// b    .  b    .  .    c  b    c  b    c
//  dddd    dddd    ....    dddd    dddd
// .    f  e    f  .    f  e    f  .    f
// .    f  e    f  .    f  e    f  .    f
//  gggg    gggg    ....    gggg    gggg

var inputPath = "./inputs/input.txt";
var displays = File.ReadAllLines(inputPath).Select(x => new Display(x)).ToList();

var simpleDigits = displays.Select(d => d.GetSimpleDigitsCount()).Sum();
Console.WriteLine("Simple digits: {0}", simpleDigits);

var sum = displays.Select(d => d.ResolveOutput()).Sum();
Console.WriteLine("Sum (part2): {0}", sum);

class Display
{
  // Number of segments -> possible digit
  public static IDictionary<int, int[]> segmentsCount = new Dictionary<int, int[]>() {
    { 2, new int[] { 1 } },
    { 3, new int[] { 7 } },
    { 4, new int[] { 4 } },
    { 5, new int[] { 2, 5, 3 } },
    { 6, new int[] { 0, 6, 9 } },
    { 7, new int[] { 8 } },
  };

  public static IDictionary<string, int> digitMap = new Dictionary<string, int>() {
    { "abcefg", 0 },
    { "cf", 1 },
    { "acdeg", 2 },
    { "acdfg", 3 },
    { "bcdf", 4 },
    { "abdfg", 5 },
    { "abdefg", 6 },
    { "acf", 7 },
    { "abcdefg", 8 },
    { "abcdfg", 9 },
  };

  public List<string> patterns { get; private set; }
  public List<string> output { get; private set; }

  public Display(string input)
  {
    var parts = input.Split('|').ToArray();
    patterns = parts[0].Trim().Split(' ').ToList();
    output = parts[1].Trim().Split(' ').ToList();
  }

  // Return the number of times simple digits (1, 4, 7 adn 8) appear in the output.
  public int GetSimpleDigitsCount()
  {
    return output.Where(x => segmentsCount[x.Length].Length == 1).Count();
  }

  public int ResolveOutput()
  {
    //
    // Deduction
    //

    // Build a mapping (segment letter -> possible segment letter for the current display)
    var mapping = new Dictionary<char, char>();

    // Deduct 1, 7, 4, 8
    var one = patterns.Where(x => x.Length == 2).First();
    var seven = patterns.Where(x => x.Length == 3).First();
    var four = patterns.Where(x => x.Length == 4).First();
    var eight = patterns.Where(x => x.Length == 7).First();
    // Deduct a
    mapping['a'] = seven.Where(x => !one.Contains(x)).First();
    // Deduct 6
    var six = patterns.Where(x => x.Length == 6).Where(x => !(x.Contains(one[0]) && x.Contains(one[1]))).First();
    // Deduct f
    mapping['f'] = one.Intersect(six).First();
    // Deduct c
    mapping['c'] = one.Where(x => x != mapping['f']).First();
    // Deduct 2, 5, 3, 0 and 9
    var two = patterns.Where(x => !x.Contains(mapping['f'])).First();
    var five = patterns.Where(x => x.Length == 5).Where(x => !x.Contains(mapping['c'])).First();
    var three = patterns.Where(x => x.Length == 5).Where(x => x != two && x != five).First();
    var nineOrZero = patterns.Where(x => x.Length == 6).Where(x => x != six);
    var zero = nineOrZero.Where(x => x.ToArray().Intersect(five.ToArray()).Count() == 4).First();
    var nine = nineOrZero.Where(x => x != zero).First();
    // Deduct d
    mapping['d'] = eight.Where(x => !zero.Contains(x)).First();
    // Deduct g
    mapping['g'] = three.Where(x => x != mapping['a'] && x != mapping['c'] && x != mapping['d'] && x != mapping['f']).First();
    // Deduct e
    mapping['e'] = zero.Where(x => !nine.Contains(x) && x != mapping['d']).First();
    // Deduct b
    mapping['b'] = "abcdefg".Where(x => !mapping.ContainsValue(x)).First();

    //
    // Assembly
    //

    var result = 0;
    for (var digit = 0; digit < 4; digit++)
    {
      var mappedOutput = output[digit].Select(x => mapping.Keys.Where(k => mapping[k] == x).First()).ToList();
      mappedOutput.Sort();
      var digitString = new String(mappedOutput.ToArray());
      result += (int)Math.Pow(10, 4 - digit - 1) * digitMap[digitString];
    }
    return result;
  }


}