using System.Text;

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
        Antenodes = new Dictionary<(int, int), List<char>>();
        foreach (var frequency in Antennas.Keys)
        {
            foreach (var pairs in GetCombinations(Antennas[frequency].ToArray()))
            {
                foreach (var antenode in GetAntenodes(pairs.Item1, pairs.Item2))
                {
                    if (!Antenodes.ContainsKey(antenode)) Antenodes[antenode] = new List<char>();
                    Antenodes[antenode].Add(frequency);
                }
            }
        }
    }

    private char[][] Map { get; }

    private Dictionary<char, List<(int, int)>> Antennas { get; }

    public Dictionary<(int,int), List<char>> Antenodes { get; }
    
    // Print the map analysis
    public void Print()
    {
        Console.WriteLine("Map:");
        foreach (var row in Map)
        {
            Console.WriteLine(row);
        }
        Console.WriteLine();
        
        Console.WriteLine("Antenodes:");
        for (var y = 0; y < Map.Length; y++)
        {
            for (var x = 0; x < Map.Length; x++)
            {
                if (Antenodes.ContainsKey((y,x)))
                    Console.Write('?');
                else
                    Console.Write(Map[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    // Get pair combinations for a given list of stuff
    public static IEnumerable<(T, T)>  GetCombinations<T>(T[] stuff) 
    {
        for (var i = 0; i < stuff.Length; i++)
        for (var j = i + 1; j < stuff.Length; j++)
        {
            yield return (stuff[i], stuff[j]);
        }        
    }
    
    // Return the two antenodes for a given pair of antennas
    public IEnumerable<(int,int)> GetAntenodes((int,int) antennaA, (int,int) antennaB)
    {
        var (yA, xA) =  antennaA ;
        var (yB, xB) =  antennaB ;
        
        var antenode1 = (2 * yA-yB, 2 * xA-xB);
        var antenode2 = (2 * yB-yA, 2 * xB-xA);
        
        if (WithinBounds(antenode1) )
            yield return antenode1;
        if (WithinBounds(antenode2) )
            yield return antenode2;
    }

    public bool WithinBounds((int, int) point)
    {
        var (y, x) = point;
        return y >= 0 && y < Map.Length && x >= 0 && x < Map[y].Length;
    }
}