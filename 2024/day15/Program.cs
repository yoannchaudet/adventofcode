using day15;

var input = "i_puzzle.txt";
var (sokoban, directions) = Sokoban.ParseInput(input);

sokoban.PrintMap();
foreach (var direction in directions)
    // Console.WriteLine($"Moving in direction {direction}");
    sokoban.Move(direction);
// sokoban.PrintMap();
Console.WriteLine($"Part 1 = {sokoban.GetSumGPS()}");