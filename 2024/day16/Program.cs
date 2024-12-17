using day16;

var input = "i_puzzle.txt";
var maze = new Maze(input);
var (part1, part2) = maze.GetBestPath();
Console.WriteLine($"Part 1  = {part1}");
Console.WriteLine($"Part 2  = {part2}");