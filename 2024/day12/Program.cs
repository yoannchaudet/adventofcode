using day12;

var input = "i_puzzle.txt";

var farm = new Farm(input);
var regions = farm.GetRegions();

var part1 = regions.Select(r => r.Perimeter * r.Area).Sum();
Console.WriteLine($"Part 1 = {part1}");