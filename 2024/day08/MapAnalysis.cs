namespace day08;

public class MapAnalysis
{
    public MapAnalysis(string input)
    {
        Map = File.ReadLines(input).Select(line => line.ToCharArray()).ToArray();

        // Init the antennas
        Antennas = new Dictionary<char, List<(int, int)>>();
        for (var y = 0; y < Map.Length; y++)
        for (var x = 0; x < Map[y].Length; x++)
        {
            var antenna = Map[y][x];
            if (!char.IsLetterOrDigit(antenna)) continue;
            if (!Antennas.ContainsKey(antenna)) Antennas[antenna] = new List<(int, int)>();
            Antennas[antenna].Add((y, x));
        }

        // Init antenodes
        Antenodes = new Dictionary<(int, int), ISet<char>>();
    }

    private char[][] Map { get; }

    public Dictionary<char, List<(int, int)>> Antennas { get; }

    public Dictionary<(int, int), ISet<char>> Antenodes { get; }

    public void ComputeAntenodes(bool part2)
    {
        foreach (var frequency in Antennas.Keys)
        foreach (var pairs in GetCombinations(Antennas[frequency].ToArray()))
        foreach (var antenode in GetAntenodes(pairs.Item1, pairs.Item2, part2))
        {
            if (part2 && char.IsLetterOrDigit(Map[antenode.Item1][antenode.Item2])) continue;
            if (!Antenodes.ContainsKey(antenode)) Antenodes[antenode] = new HashSet<char>();
            Antenodes[antenode].Add(frequency);
        }
    }

    // Print the map analysis
    public void Print()
    {
        Console.WriteLine("Map:");
        foreach (var row in Map) Console.WriteLine(row);
        Console.WriteLine();

        Console.WriteLine("Antenodes:");
        for (var y = 0; y < Map.Length; y++)
        {
            for (var x = 0; x < Map[y].Length; x++)
                if (Antenodes.ContainsKey((y, x)))
                    Console.Write('?');
                else
                    Console.Write(Map[y][x]);
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    // Get pair combinations for a given list of stuff
    public static IEnumerable<(T, T)> GetCombinations<T>(T[] stuff)
    {
        for (var i = 0; i < stuff.Length; i++)
        for (var j = i + 1; j < stuff.Length; j++)
            yield return (stuff[i], stuff[j]);
    }

    // Return the two antenodes for a given pair of antennas
    public IEnumerable<(int, int)> GetAntenodes((int, int) antennaA, (int, int) antennaB, bool part2 = false)
    {
        //  Ap -- A -- B -- Bp

        var (yA, xA) = antennaA;
        var (yB, xB) = antennaB;

        var antenodeAp = (2 * yA - yB, 2 * xA - xB);
        var yyA = yA;
        var xxA = xA;
        while (WithinBounds(antenodeAp))
        {
            yield return antenodeAp;
            if (!part2)
                break;
            var last = antenodeAp;
            antenodeAp = (2 * antenodeAp.Item1 - yyA, 2 * antenodeAp.Item2 - xxA);
            yyA = last.Item1;
            xxA = last.Item2;
        }

        var antenodeBp = (2 * yB - yA, 2 * xB - xA);
        while (WithinBounds(antenodeBp))
        {
            yield return antenodeBp;
            if (!part2)
                break;
            var last = antenodeBp;
            antenodeBp = (2 * antenodeBp.Item1 - yB, 2 * antenodeBp.Item2 - xB);
            yB = last.Item1;
            xB = last.Item2;
        }
    }

    public bool WithinBounds((int, int) point)
    {
        var (y, x) = point;
        return y >= 0 && y < Map.Length && x >= 0 && x < Map[y].Length;
    }
}