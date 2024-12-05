using day05;

var input = "i_puzzle.txt";
var (orders, updates) = ReadInput(input);

// Part 1
var part1 = updates.Where(IsOrdered).Select(GetMiddle).Sum();
Console.WriteLine($"Part 1 = {part1}");

// Part 2
var pageComparer = new PageComparer(orders);
var unorderedUpdates = updates.Where(u => !IsOrdered(u)).ToList();
foreach (var u in unorderedUpdates) u.Sort(pageComparer);
var part2 = unorderedUpdates.Select(GetMiddle).Sum();
Console.WriteLine($"Part 2 = {part2}");

bool IsOrdered(List<int> u)
{
    for (var i = 1; i <= u.Count - 1; i++)
    {
        var a = u[i - 1];
        var b = u[i];
        if (!orders.Contains((a, b))) return false;
    }

    return true;
}

static (List<(int, int)>, List<List<int>>) ReadInput(string input)
{
    var lines = File.ReadAllLines(input);
    var orders = lines.Where(l => "|".Contains(l)).Select(l =>
    {
        var tuple = l.Split("|");
        return (int.Parse(tuple[0]), int.Parse(tuple[1]));
    }).ToList();
    var updates = lines.Where(l => ",".Contains(l)).Select(l => l.Split(",").Select(int.Parse).ToList()).ToList();
    return (orders, updates);
}

static int GetMiddle(List<int> updates)
{
    return updates[updates.Count / 2];
}