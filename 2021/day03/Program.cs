// Given a list of binary number of same length, return an array containing
// a 1 at a given position to indicate 0 was the most common bit.
static int[] GetMostCommonZeroBits(List<string> binaryNumbers) {
  int[] zeroBits = new int[binaryNumbers.First().Length];
  foreach (var binaryNumber in binaryNumbers) {
    for (int i = 0; i < zeroBits.Length; i++) {
      if (binaryNumber[i] == '0') {
        zeroBits[i]++;
      }
    }
  }
  return zeroBits;
}

// Return the most common bit (1 or 0) for a given zero-bit count.
static int GetMostCommonBit(int zeroBits, int commandsCount) {
  return zeroBits <= commandsCount / 2 ? 1 : 0;
}

// Return the least common bit (1 or 0) for a given zero-bit count.
static int GetLeastCommonBit(int zeroBits, int commandsCount) {
  return GetMostCommonBit(zeroBits, commandsCount) == 0 ? 1 : 0;
}

// Given a most common zero bit array, return the most common base10 number.
static int GetCommonBase10(int[] mostCommonZeroBits, int commandsCount, int commonBit) {
  int numberBase10 = 0;
  for (int i = 0; i < mostCommonZeroBits.Length; i++) {
   // Most common bit for position i is 1
   if (GetMostCommonBit(mostCommonZeroBits[i], commandsCount) == commonBit) {
     numberBase10 += (int)Math.Pow(2, mostCommonZeroBits.Length - i - 1);
   }
  }
  return numberBase10;
}

// Get the commands and compute the most common bits
var inputPath = "./inputs/input.txt";
var commands = File.ReadAllLines(inputPath).ToList();
int[] mostCommonZeroBits = GetMostCommonZeroBits(commands);
int gammaRateBase10 = GetCommonBase10(mostCommonZeroBits, commands.Count, 1);
int epsilonRateBase10 = GetCommonBase10(mostCommonZeroBits, commands.Count, 0);
Console.WriteLine("Gamma rate = {0}", gammaRateBase10);
Console.WriteLine("Epsilon rate = {0}", epsilonRateBase10);
Console.WriteLine("Result (part1) = {0}", gammaRateBase10 * epsilonRateBase10);

int index = 0;
var ogrPrefix = GetMostCommonBit(mostCommonZeroBits[index++], commands.Count).ToString();
var ogrCandidates = commands.Where(c => c.StartsWith(ogrPrefix)).ToList();
while (ogrCandidates.Count > 1) {
  int[] candidatesMostCommonZeroBits = GetMostCommonZeroBits(ogrCandidates);
  ogrPrefix += GetMostCommonBit(candidatesMostCommonZeroBits[index++], ogrCandidates.Count).ToString();
  ogrCandidates = ogrCandidates.Where(c => c.StartsWith(ogrPrefix)).ToList();
}
var ogr = Convert.ToInt32(ogrCandidates.First(), 2);
Console.WriteLine("OGR = {0}", ogr);

index = 0;
var csrPrefix = GetLeastCommonBit(mostCommonZeroBits[index++], commands.Count).ToString();
var csrCandidates = commands.Where(c => c.StartsWith(csrPrefix)).ToList();
while (csrCandidates.Count > 1) {
  int[] candidatesMostCommonZeroBits = GetMostCommonZeroBits(csrCandidates);
  csrPrefix += GetLeastCommonBit(candidatesMostCommonZeroBits[index++], csrCandidates.Count).ToString();
  csrCandidates = csrCandidates.Where(c => c.StartsWith(csrPrefix)).ToList();
}
var csr = Convert.ToInt32(csrCandidates.First(), 2);
Console.WriteLine("CSR = {0}", csr);

Console.WriteLine("Life support rating (part 2) = {0}", ogr * csr);
