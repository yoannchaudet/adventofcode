const string input = "inputs/input.txt";

// Part 1
var races = ParseInput(input).ToList();
var totalWins = new List<long>();
foreach (var race in races)
    totalWins.Add(race.GetTotalWins());
var part1 = totalWins.Aggregate((a, b) => a * b);
Console.WriteLine($"Part 1 = {part1}");

// Part 2
races = ParseInput(input, true).ToList();
Console.WriteLine($"Part 2 = {races.First().GetTotalWins()}");

IEnumerable<Race> ParseInput(string input, bool clearKerning=false)
{
    var lines = File.ReadLines(input).ToList();
    if(clearKerning)
    {
        lines = lines.Select(l => l.Replace(" ", "")).ToList();
        var time = long.Parse(lines[0].Substring("Time:".Length));
        var record = long.Parse(lines[1].Substring("Distance:".Length));
        yield return new Race(time, record);
    }

    else
    {
        var times = lines[0].Substring("Time:".Length).Split(" ").Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(long.Parse).ToList();
        var records = lines[1].Substring("Distance:".Length).Split(" ").Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(long.Parse).ToList();
        for (var i = 0; i < times.Count(); i++)
        {
            yield return new Race(times[i], records[i]);
        }
    }
}   

class Race
{
    public long TimeInMs { get; set; }
    public long RecordDistanceInMm { get; set; }

    public Race(long timeInMs, long recordDistanceInMm)
    {
        TimeInMs = timeInMs;
        RecordDistanceInMm = recordDistanceInMm;
    }
    
    public long GetDistance(long pushTimeInMs)
    {
        if (pushTimeInMs >= TimeInMs)
        {
            return 0;
        }
        return (TimeInMs - pushTimeInMs) * pushTimeInMs;
    }

    public long GetTotalWins()
    {
        long wins = 0;
        for (int pushTime = 1; pushTime < TimeInMs; pushTime++)
        {
            if (GetDistance(pushTime) > RecordDistanceInMm)
            {
                wins += 1;
            }
        }
        return wins;
    }
}