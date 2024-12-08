using day08;

var input = "i_puzzle.txt";
var map = new MapAnalysis(input);
map.Print();

var part1 = map.Antenodes.Values.Count;
Console.WriteLine($"Part 1 = {part1}");