const string input = "inputs/input.txt";
var (instructions, tree) = ParseInput();
bool part2 = true;
if (!part2)
{
    Console.WriteLine($"Part 1 = {GetPart1Steps()}");
}

// Part 2
var startLocations = tree.Keys.Where(k => k.EndsWith("A")).ToList();
var steps = new List<long>();
foreach (var location in startLocations)
{
    steps.Add(GetPart2Steps(location));
    Console.WriteLine($"{location} = {GetPart2Steps(location)}");
}
// Just an intuition really. The input is full of cycle. Since each step count we get is much greater than the number of instructions, we have been cycling a bit already.
// The steps are not prime numbers (or I would have multiplied them). Trying to get the least common multiple instead.
Console.WriteLine($"Part 2 = {ArrayLcm(steps.ToArray())}");

// Just walk the graph/tree until we find ZZZ
int GetPart1Steps()
{
    var step = 0;
    var location = "AAA";

    while (location != "ZZZ")
    {
        var instruction = instructions[step % instructions.Length];
        location = instruction == 'L' ? tree[location].Item1 : tree[location].Item2;
        step += 1;
    }

    return step;
}

// Steal some math:
// https://stackoverflow.com/a/29717490 

static long ArrayLcm(long[] numbers)
{
    return numbers.Aggregate(Lcm);
}
static long Lcm(long a, long b)
{
    return Math.Abs(a * b) / Gcd(a, b);
}
static long Gcd(long a, long b)
{
    return b == 0 ? a : Gcd(b, a % b);
}

// Get the first number of steps for a given location
long GetPart2Steps(string location)
{
    var step = 0;

    while (!location.EndsWith("Z"))
    {
        var instruction = instructions[step % instructions.Length];
        location = instruction == 'L' ? tree[location].Item1 : tree[location].Item2;
        step += 1;
    }

    return step;
}

// Parse the input
(string, Dictionary<string, (string,string)>) ParseInput()
{
    var lines = File.ReadAllLines(input).ToList();
    var localInstructions = lines[0];
    var localTree = new Dictionary<string, (string, string)>();
    for (var i = 2; i < lines.Count; i++)
    {
        var line = lines[i];
        localTree.Add(line.Substring(0, 3), (line.Substring(7, 3), line.Substring(12, 3)));
    }

    return (localInstructions, localTree);
}