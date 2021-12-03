// Get the commands from the file (as string)
static (int, List<string>) GetCommands(string inputPath) {
  var lines = File.ReadAllLines(inputPath).Where(line => !string.IsNullOrWhiteSpace(line));
  return (lines.First().Length, lines.ToList());
}

// Get the commands and compute the most common bits
var commands = GetCommands("./inputs/input.txt");
int[] zeroBits = new int[commands.Item1];
foreach (var command in commands.Item2) {
  for (int i = 0; i < zeroBits.Length; i++) {
    if (command[i] == '0') {
      zeroBits[i]++;
    }
  }
}

int gammaRateBase10 = 0;
int epsilonRateBase10 = 0;
for (int i = 0; i < zeroBits.Length; i++) {
   // Most common bit for position i is 1
   if (zeroBits[i] < commands.Item2.Count / 2) {
    gammaRateBase10 += (int)Math.Pow(2, zeroBits.Length - i - 1);
   } else {
    epsilonRateBase10 += (int)Math.Pow(2, zeroBits.Length - i - 1);
   }
}
Console.WriteLine("Gamma rate = {0}", gammaRateBase10);
Console.WriteLine("Epsilon rate = {0}", epsilonRateBase10);
Console.WriteLine("Result = {0}", gammaRateBase10 * epsilonRateBase10);