var directions = new Dictionary<string, int[]>() {
  // command, [x offset, y offset]
  { "forward", new int[]{ 1, 0 }},
  { "down", new int[]{ 0, 1 }},
  { "up", new int[]{ 0, -1 }},
};
// Parse a command.
(string, int) ParseCommand(string command)
{
  var parts = command.Split(' ');
  if (!directions.ContainsKey(parts[0]))
  {
    throw new Exception("Invalid command: " + command);
  }
  return (parts[0], int.Parse(parts[1]));
}

// Parse a list of commands.
List<(string, int)> ParseCommands(string inputPath)
{
  return File.ReadAllLines(inputPath)
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(ParseCommand)
    .ToList();
}

// Parse the commands and start computing the x and y coordinates.
var commands = ParseCommands("./inputs/input.txt");
int xPart1 = 0, yPart1 = 0;
int xPart2 = 0, yPart2 = 0, aimPart2 = 0;
commands.ForEach(command =>
{
  // Compute part 1 x and y coordinates
  xPart1 += directions[command.Item1][0] * command.Item2;
  yPart1 += directions[command.Item1][1] * command.Item2;

  // Compute part 2 x, y and aim
  switch (command.Item1)
  {
    case "forward":
      xPart2 += command.Item2;
      yPart2 += aimPart2 * command.Item2;
      break;
    case "up":
      aimPart2 -= command.Item2;
      break;
    case "down":
      aimPart2 += command.Item2;
      break;
  }
});
var result1 = xPart1 * yPart1;
var result2 = xPart2 * yPart2;
Console.WriteLine("Result (part1) = {0} (x = {1}, y = {2})", result1, xPart1, yPart1);
Console.WriteLine("Result (par 2) = {0} (x = {1}, y = {2})", result2, xPart2, yPart2);
