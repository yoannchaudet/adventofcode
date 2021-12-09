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

var simpleDigits = displays.Select(x => x.GetSimpleDigitsCount()).Sum();
Console.WriteLine("Simple digits: {0}", simpleDigits);

class Display
{
  public static IDictionary<int, int[]> segmentsCount = new Dictionary<int, int[]>() {
    { 2, new int[] { 1 } },
    { 3, new int[] { 7 } },
    { 4, new int[] { 4 } },
    { 5, new int[] { 2, 5, 3 } },
    { 6, new int[] { 0, 6, 9 } },
    { 7, new int[] { 8 } },
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
}