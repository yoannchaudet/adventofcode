using System.Diagnostics;
using day11;

var input = "i_puzzle.txt";
var sa = new StoneAnalyzer(input);
var part1 = sa.GetStonesAfterBlinks(26);

// Part 1
Console.WriteLine($"Part 1 = {part1}");

// Part 2
Console.WriteLine("Starting part 2...");
var sw = new Stopwatch();
sw.Start();
var part2 = sa.GetStonesAfterBlinks(75);
Console.WriteLine($"Part 2 = {part2}");
sw.Stop();
Console.WriteLine("Elapsed = {sw.Elapsed}");