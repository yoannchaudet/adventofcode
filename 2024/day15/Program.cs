using day15;

var input = "i_puzzle.txt";

// Part 1
{
    var (sokoban, directions) = Sokoban.ParseInput(input);
    sokoban.PrintMap();
    foreach (var direction in directions)
        sokoban.Move(direction);
    Console.WriteLine($"Part 1 = {sokoban.GetSumGps()}");
    Console.WriteLine();
}

// Part 2
{
    var (sokoban, directions) = Sokoban.ParseInput(input, true);
    sokoban.PrintMap();
    foreach (var direction in directions) sokoban.Move(direction);
    Console.WriteLine($"Part 2 = {sokoban.GetSumGps()}");
}