
using System.Numerics;

var inputPath = "./inputs/input.txt";
var afterDays = 256;
var timers = File.ReadAllLines(inputPath).Where(line => line.Length > 0).First().Split(',').Select(int.Parse).ToList();
var groupedTimers = timers.GroupBy(x => x);
var population = groupedTimers.Select(timerGroup => timerGroup.Count() * GetLanternFishes(timerGroup.Key, afterDays)).Sum();
Console.WriteLine("Lantern fishes (at day {0}) = {1}", afterDays, population);

// Get the number of lantern fishes after a number of days (for a given initial timer).
static long GetLanternFishes(int timer, int afterDays)
{
  // Self
  var population = (long) 1;

  int delta = 1;

  while (afterDays > 1)
  {
    timer -= delta;
    afterDays -= delta;

    if (timer == 0 && afterDays >= 1)
    {
      // New fish + reset timer
      population += GetLanternFishes(8 + 1, afterDays);
      timer = 6 + 1;
      delta = timer;
    }
  }

  return population;
}
