
using System.Numerics;

var inputPath = "./inputs/input.txt";
var afterDays = 256;
var timers = File.ReadAllLines(inputPath).Where(line => line.Length > 0).First().Split(',').Select(int.Parse).ToList();
var groupedTimers = timers.GroupBy(x => x);
var cache = new Dictionary<(int, int), long>();
var population = groupedTimers.Select(timerGroup => timerGroup.Count() * GetLanternFishes(timerGroup.Key, afterDays, cache)).Sum();
Console.WriteLine("Lantern fishes (at day {0}) = {1}", afterDays, population);

// Get the number of lantern fishes after a number of days (for a given initial timer).
static long GetLanternFishes(int timer, int afterDays, IDictionary<(int, int), long> cache)
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
      // New fish
      var key = (8 + 1, afterDays);
      if (!cache.ContainsKey(key))
        cache.Add(key, GetLanternFishes(8 + 1, afterDays, cache));
      population += cache[key];

      // Reset timer (+ optimize delta)
      timer = 6 + 1;
      delta = timer;
    }
  }
  return population;
}
