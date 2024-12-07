using day07;

var input = "i_puzzle.txt";
var equations = File.ReadLines(input).Select(line => new Equation(line)).ToList();

var part1 = equations.Where(e => e.Resolves()).Select(e => e.Total).Sum();
Console.WriteLine($"Part1 = {part1}");

var part2 = equations.Where(e => e.Resolves(true)).Select(e => e.Total).Sum();
Console.WriteLine($"Part2 = {part2}");