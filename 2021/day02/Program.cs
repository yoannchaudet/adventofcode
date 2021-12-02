var directions = new Dictionary<string, int[]>() {
  // command, [x offset, y offset]
  { "forward", new int[]{ 1, 0 }},
  { "down", new int[]{ 0, 1 }},
  { "up", new int[]{ 0, -1 }},
};

// Parse a command.
(string, int) ParseCommand(string command) {
  var parts = command.Split(' ');
  if (!directions.ContainsKey(parts[0])) {
    throw new Exception("Invalid command: " + command);
  }
  return (parts[0], int.Parse(parts[1]));
}

// Parse a list of commands.
List<(string, int)> ParseCommands(string inputPath) {
  return File.ReadAllLines(inputPath)
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(ParseCommand)
    .ToList();
}

// Parse the commands and start computing the x and y coordinates.
var commands = ParseCommands("./inputs/input.txt");
var x = 0;
var y = 0;
commands.ForEach(command => {
  x += directions[command.Item1][0] * command.Item2;
  y += directions[command.Item1][1] * command.Item2;
});
var result = x * y;
Console.WriteLine("Result = {0} (x = {1}, y = {2})", result, x, y);