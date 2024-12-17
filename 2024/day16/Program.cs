using day16;

var input = "i_puzzle.txt";
var maze = new Maze(input);
var part1 = maze.ShortestPath();
Console.WriteLine($"Part 1  = {part1}");