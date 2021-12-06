
var inputPath = "./inputs/input.txt";
var afterDays = 80;
var timers = File.ReadAllLines(inputPath).Where(line => line.Length > 0).First().Split(',').Select(int.Parse).ToList();
var population = timers.Select(timer => GetLanternFishes(timer, afterDays)).Sum();
Console.WriteLine("Lantern fishes (at day {0}) = {1}", afterDays, population);

// Get the number of lantern fishes after a number of days (for a given initial timer).
static int GetLanternFishes(int timer, int afterDays)
{
  // Self
  var population = 1;

  while (afterDays > 1)
  {
    timer--;
    afterDays--;

    if (timer == 0)
    {
      // New fish + reset timer
      population += GetLanternFishes(8 + 1, afterDays);
      timer = 6 + 1;
    }
  }

  return population;
}
