using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

namespace day13;

public class Equation
{
    public Equation((long, long) a, (long,
        long) b, (long, long) prize)
    {
        A = a;
        B = b;
        Prize = prize;
    }

    public (long, long) A { get; }
    public (long, long) B { get; }
    public (long, long) Prize { get; }

    // Resolve the equation and if possible, return (A, B).
    public (long, long)? Resolve()
    {
        // Resolve linear equations
        var coefs = Matrix<double>.Build.DenseOfArray(new double[,]
        {
            { A.Item1, B.Item1 },
            { A.Item2, B.Item2 }
        });
        var vectors = Vector<double>.Build.Dense(new double[] { Prize.Item1, Prize.Item2 });
        var solution = coefs.Solve(vectors);
        var (a, b) = ((long)Math.Round(solution[0]), (long)Math.Round(solution[1]));

        // Verify solution
        if (a * A.Item1 + b * B.Item1 == Prize.Item1 && a * A.Item2 + b * B.Item2 == Prize.Item2)
            return (a, b);
        return null;
    }

    // Parse the equations
    public static IEnumerable<Equation> Parse(string input, bool part2 = false)
    {
        (long, long)? a = null;
        (long, long)? b = null;

        var buttonARegex = new Regex(@"Button A\: X\+([0-9]+)\, Y\+([0-9]+)");
        var buttonBRegex = new Regex(@"Button B\: X\+([0-9]+)\, Y\+([0-9]+)");
        var prizeRegex = new Regex(@"Prize\: X\=([0-9]+)\, Y\=([0-9]+)");

        foreach (var line in File.ReadAllLines(input))
        {
            if (string.IsNullOrEmpty(line))
                continue;

            var match = buttonARegex.Match(line);
            if (match.Success)
            {
                a = (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
                continue;
            }

            match = buttonBRegex.Match(line);
            if (match.Success)
            {
                b = (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
                continue;
            }

            match = prizeRegex.Match(line);
            if (match.Success)
            {
                (long, long)? prize = (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
                if (part2)
                    prize = (prize.Value.Item1 + 10000000000000, prize.Value.Item2 + 10000000000000);
                yield return new Equation(a.Value, b.Value, prize.Value);
                a = null;
                b = null;
            }
        }
    }

    // Return the minimum number of tokens to win all pricess that can be won
    public static long GetMinimumTokens(IEnumerable<Equation> equations)
    {
        return equations.Select(e => e.Resolve()).Where(solution => solution.HasValue)
            .Select(solution => solution!.Value.Item1 * 3 + solution.Value.Item2 * 1).Sum();
        ;
    }
}