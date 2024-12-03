var input = "i_puzzle.txt";
var l1 = new List<int>();
var l2 = new List<int>();
foreach (var line in File.ReadLines(input))
{ 
    var ids = line.Split("  ");
    l1.Add(int.Parse(ids[0]));
    l2.Add(int.Parse(ids[1]));
};
l1.Sort();
l2.Sort();

// Part 1
var part1 = 0;
for (var i = 0; i < l1.Count; i++)
{
    part1 += Math.Abs(l2[i] - l1[i]);
}
Console.WriteLine($"Part 1 = {part1}");

// Part 2
var part2 = 0;
for (var i = 0; i < l1.Count; i++)
{
    part2 += l1[i] * l2.Count(n => n == l1[i]);
}
Console.WriteLine($"Part 2 = {part2}");

