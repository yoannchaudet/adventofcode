using day09;

var input = "i_puzzle.txt";
var diskMap = File.ReadLines(input).FirstOrDefault("");

// Part 1
{
    var bo = new BlockOrganizer(diskMap);
    bo.CompactPart1();
    var part1 = bo.ComputeChecksum();
    Console.WriteLine($"Part 1 = {part1}");
}

// Part 2
{
    var bo = new BlockOrganizer(diskMap);
    bo.CompactPart2();
    var part2 = bo.ComputeChecksum();
    Console.WriteLine($"Part 2 = {part2}");
}