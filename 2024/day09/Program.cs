using day09;

var input = "i_puzzle.txt";
var diskMap = File.ReadLines(input).FirstOrDefault("");
var bo = new BlockOrganizer(diskMap);
bo.Compact();
var part1 = bo.ComputeChecksum();
Console.WriteLine($"Part 1 = {part1}");