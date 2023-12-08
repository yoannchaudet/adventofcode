// Parse input
const string input = "inputs/input.txt";
var hands = ParseInput().ToList();

// Part 1
var sortedHands = hands.OrderBy(h => h.Item1, new HandComparer("AKQJT98765432")).ToList();
Console.WriteLine("Sorted cards:");
Console.WriteLine(string.Join(Environment.NewLine, sortedHands.Select(h => h.Item1).ToList()));
var part1 = sortedHands.Select((hand, index) => hand.Item2 * (index + 1)).Sum();
Console.WriteLine($"Part 1 = {part1}");
Console.WriteLine();

// Part 2
var handsWithJoker = hands.Select(h => (new Hand(h.Item1.Value, true), h.Item2)).ToList();
var sortedHandsWithJoker = handsWithJoker.OrderBy(h => h.Item1, new HandComparer("AKQT98765432J")).ToList();
Console.WriteLine("Sorted cards:");
Console.WriteLine(string.Join(Environment.NewLine, sortedHandsWithJoker.Select(h => h.Item1).ToList()));
var part2 = sortedHandsWithJoker.Select((hand, index) => hand.Item2 * (index + 1)).Sum();
Console.WriteLine($"Part 2 = {part2}");

IEnumerable<(Hand, int)> ParseInput()
{
    return File.ReadAllLines(input).Select(line =>
    {
        var parts = line.Split(" ");
        return (new Hand(parts[0].Trim()), int.Parse(parts[1].Trim()));
    });
}

public enum HandType
{
    FiveOfAKind = 0,
    FourOfAKind,
    FullHouse,
    ThreeOfAKind,
    TwoPairs,
    OnePair,
    HighCard
}

public class HandComparer : IComparer<Hand>
{
    public HandComparer(string handsOrder)
    {
        HandsOrder = handsOrder;
    }

    private string HandsOrder { get; }

    public int Compare(Hand? card1, Hand? card2)
    {
        if (card1 == null || card2 == null) throw new Exception("Invalid null comparison");

        var c1 = card1.GetHandType();
        var c2 = card2.GetHandType();

        // Compare the cards
        if (c1 == c2)
            for (var i = 0; i < 5; i++)
            {
                var compare = Compare(card1.Value[i], card2.Value[i]);
                if (compare != 0 || i == 4)
                    return compare;
            }

        return c1 < c2 ? 1 : -1;
    }

    private int Compare(char card1, char card2)
    {
        var c1 = HandsOrder.IndexOf(card1);
        var c2 = HandsOrder.IndexOf(card2);
        if (c1 == c2)
            return 0;
        return c1 < c2 ? 1 : -1;
    }
}

public class Hand
{
    public Hand(string value, bool withJoker = false)
    {
        Value = value;
        WithJoker = withJoker;
    }

    public string Value { get; init; }
    private bool WithJoker { get; }
    private HandType? MemoHandType { get; set; }

    public HandType GetHandType()
    {
        if (!MemoHandType.HasValue)
        {
            if (WithJoker && Value.Contains("J"))
                MemoHandType = GetHandTypeWithJoker(Value);
            else
                MemoHandType = GetHandType(Value);
        }

        return MemoHandType.Value;
    }

    private HandType GetHandTypeWithJoker(string value)
    {
        var cards = value.GroupBy(c => c).Where(c => c.Key != 'J').Select(c => (c.Key.ToString(), c.Count())).ToList();
        var topCard = cards.Count == 0 ? "J" : cards.MaxBy(c => c.Item2).Item1;

        return GetHandType(value.Replace("J", topCard));
    }

    private HandType GetHandType(string value)
    {
        var cards = value.GroupBy(c => c).Select(c => (c.Key.ToString(), c.Count())).ToList();
        switch (cards.Count)
        {
            case 1:
                return HandType.FiveOfAKind;
            case 2:
                if (cards.Count(c => c.Item2 == 3) == 1)
                    return HandType.FullHouse;
                return HandType.FourOfAKind;
            case 3:
                if (cards.Count(c => c.Item2 == 2) == 2)
                    return HandType.TwoPairs;
                return HandType.ThreeOfAKind;
            case 4:
                return HandType.OnePair;
            default:
                return HandType.HighCard;
        }
    }

    public override string ToString()
    {
        return $"{Value} ({GetHandType().ToString()})";
    }
}