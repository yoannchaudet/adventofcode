using day08;

var input = "i_puzzle.txt";

// Part 1
{
    var map = new MapAnalysis(input);
    map.ComputeAntenodes(false);
    map.Print();
    var part1 = map.Antenodes.Values.Count;
    Console.WriteLine($"Part 1 = {part1}");
}

// Part 2
{
    var map = new MapAnalysis(input);
    map.ComputeAntenodes(true);
    map.Print();
    var part2 = map.Antenodes.Values.Count;
    foreach (var antenna in map.Antennas.Keys)
        if (map.Antennas[antenna].Count > 1)
            part2 += map.Antennas[antenna].Count;
    Console.WriteLine($"Part 2 = {part2}");
}