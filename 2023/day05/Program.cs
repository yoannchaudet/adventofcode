const string input = "inputs/input.txt";
var (seeds, maps) = ParseInput(input);

// Part 1
var part1 = seeds.Min(seed => GetLocation(seed, maps));
Console.WriteLine($"Part 1 = {part1}");

// Part 2

// Extract seed ranges
var seedRanges = new List<Range>();
for (var i = 0; i < seeds.Count; i += 2)
    seedRanges.Add(new Range(seeds[i], seeds[i] + seeds[i + 1] - 1));
Console.WriteLine("Seed ranges:");
Console.WriteLine(string.Join(Environment.NewLine, seedRanges));

// Start from an arbitrary large number... (this is cheating and just iterate over locations until we find a matching seed)
for (var i = 1_000_000L; i < long.MaxValue; i += 100)
{
    var possibleLocations = new List<long>(capacity: 100);
    Parallel.For(i, i + 100, location =>
    {
        var seed = GetSeed(location, maps);
        if (seedRanges.Any(r => r.Contains(seed)))
        {
            possibleLocations.Add(location);
        }
    });
    if (possibleLocations.Count > 0)
    {
        var part2 = possibleLocations.Min();
        Console.WriteLine("Part 2 = " + part2);
        return;
    }
}

long GetLocation(long seed, Dictionary<string, Map> maps)
{
    var source = "seed";
    var currentValue = seed;
    while (maps.ContainsKey(source) && source != "location")
    {
        var matchingRanges = maps[source].Ranges.FirstOrDefault(ranges => ranges.SourceRange.Contains(currentValue));
        if (matchingRanges != null) currentValue = matchingRanges.ConvertSourceToDestination(currentValue);
        source = maps[source].Destination;
    }

    return currentValue;
}

long GetSeed(long location, Dictionary<string, Map> maps)
{
    var destination = "location";
    var currentValue = location;
    do
    {
        var map = maps.Values.FirstOrDefault(m => m.Destination == destination);
        var matchinRanges = map.Ranges.FirstOrDefault(ranges => ranges.DestinationRange.Contains(currentValue));
        if (matchinRanges != null)
        {
            currentValue = matchinRanges.ConvertDestinationToSource(currentValue);
        }

        destination = map.Source;
    } while (destination != "seed");

    return currentValue;
}

(List<long>, Dictionary<string, Map>) ParseInput(string inputFile)
{
    var seeds = new List<long>();
    var maps = new Dictionary<string, Map>();
    Map? currentMap = null;

    foreach (var line in File.ReadAllLines(inputFile))
        // Extract seeds
        if (line.StartsWith("seeds:"))
        {
            seeds = line.Substring("seeds: ".Length).Split(" ").Select(s => s.Trim()).Select(long.Parse).ToList();
        }

        // Start extracting map
        else if (line.Contains("map:"))
        {
            var name = line.Split(" ").First().Split("-to-");
            currentMap = new Map(name.First(), name.Last(), new List<Ranges>());
        }

        // Last line or empty line (lazy here, let's tweak the input)
        else if (string.IsNullOrWhiteSpace(line))
        {
            if (currentMap != null)
            {
                maps.Add(currentMap.Source, currentMap);
                currentMap = null;
            }
        }

        // Map line
        else
        {
            if (currentMap != null)
            {
                var range = line.Split(" ").Select(s => s.Trim()).Select(long.Parse).ToList();
                currentMap.Ranges.Add(new Ranges(range[0], range[1], range[2]));
            }
        }

    return (seeds, maps);
}

internal class Map
{
    public Map(string source, string destination, List<Ranges> ranges)
    {
        Source = source;
        Destination = destination;
        Ranges = ranges;
    }

    public string Source { get; init; }
    public string Destination { get; init; }
    public List<Ranges> Ranges { get; init; }
}

internal class Ranges
{
    public Ranges(long destinationRangeStart, long sourceRangeStart, long rangeLength)
    {
        SourceRange = new Range(sourceRangeStart, sourceRangeStart + rangeLength - 1);
        DestinationRange = new Range(destinationRangeStart, destinationRangeStart + rangeLength - 1);
    }

    public Ranges(Range source, Range destination)
    {
        SourceRange = source;
        DestinationRange = destination;
    }

    public Range? SourceRange { get; init; }

    public Range DestinationRange { get; init; }

    public long ConvertSourceToDestination(long source)
    {
        if (SourceRange != null && SourceRange.Contains(source))
        {
            var delta = source - SourceRange.Start;
            return DestinationRange.Start + delta;
        }

        return source;
    }

    public long ConvertDestinationToSource(long destination)
    {
        if (SourceRange != null)
        {
            var delta = destination - DestinationRange.Start;
            return SourceRange.Start + delta;
        }

        return destination;
    }
}

internal class Range
{
    public Range(long start, long end)
    {
        Start = start;
        End = end;
    }

    public long Start { get; init; }
    public long End { get; init; }

    public bool Contains(long value)
    {
        return value >= Start && value <= End;
    }

    public Range? GetOverlap(Range range)
    {
        if (range.End < Start || range.Start > End) return null;
        return new Range(Math.Min(Start, range.Start), Math.Max(End, range.End));
    }

    public Range? GetIntersection(Range range)
    {
        if (range.End < Start || range.Start > End) return null;
        return new Range(Math.Max(Start, range.Start), Math.Min(End, range.End));
    }

    public override string ToString()
    {
        return $"[{Start}; {End}] (size {End - Start})";
    }
}