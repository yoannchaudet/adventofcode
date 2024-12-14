using day13;

var input = "i_puzzle.txt";

// Part 1
var equations = Equation.Parse(input).ToList();
var part1 = Equation.GetMinimumTokens(equations);
Console.WriteLine($"Part 1: {part1}");

// Part 2
equations = Equation.Parse(input, true).ToList();
var part2 = Equation.GetMinimumTokens(equations);
Console.WriteLine($"Part 2: {part2}");