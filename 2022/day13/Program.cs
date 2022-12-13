using System.Text.Json.Nodes;

var input = "input.txt";

// Part 1
{
  var pairs = ParseInput(input).ToList();
  var indices = new List<int>();
  for (var i = 0; i < pairs.Count; i++)
  {
    var pair = pairs[i];
    if (pair.Item1.CompareTo(pair.Item2) == -1)
    {
      indices.Add(i + 1);
    }
  }
  Console.WriteLine("Part 1: {0}", indices.Sum());
}

// Part 2
{
  var packets = new List<Packet>();
  var marker1 = new Packet("[[2]]");
  var marker2 = new Packet("[[6]]");
  packets.Add(marker1);
  packets.Add(marker2);
  foreach (var tuple in ParseInput(input).ToList())
  {
    packets.Add(tuple.Item1);
    packets.Add(tuple.Item2);
  }
  packets.Sort();
  var decoder = (packets.IndexOf(marker1) + 1) * (packets.IndexOf(marker2) + 1);
  Console.WriteLine("Part 2: {0}", decoder);
}

static IEnumerable<Tuple<Packet, Packet>> ParseInput(string input)
{
  var lines = File.ReadAllLines(input);
  for (var i = 0; i < lines.Length; i++)
  {
    yield return Tuple.Create(new Packet(lines[i]), new Packet(lines[++i]));
    i++;
  }
}


class Packet : IComparable<Packet>
{
  public JsonArray Value { get; private set; }

  public Packet(string input)
  {
    var value = JsonNode.Parse(input) as JsonArray;
    if (value == null)
    {
      throw new Exception("Invalid packet: " + input);
    }
    Value = value;
  }

  public int CompareTo(Packet? other)
  {
    return CompareTo(Value, other.Value);
  }

  static int CompareTo(JsonNode left, JsonNode right)
  {
    // Compare two ints
    if (left is JsonValue leftNumber && right is JsonValue rightNumber)
    {
      var leftValue = leftNumber.GetValue<int>();
      var rightValue = rightNumber.GetValue<int>();
      return leftValue.CompareTo(rightValue);
    }

    // 2 arrays
    else if (left is JsonArray leftArray && right is JsonArray rightArray)
    {
      int? lastOrder = null;
      for (var i = 0; i < Math.Min(leftArray.Count, rightArray.Count); i++)
      {
        lastOrder = CompareTo(leftArray[i], rightArray[i]);

        // Stop if a decision is made
        if (lastOrder != 0)
        {
          return lastOrder.Value;
        }
      }

      // No decision
      if (lastOrder == null || lastOrder == 0)
      {
        // Left run out
        if (rightArray.Count > leftArray.Count)
        {
          return -1;
        }

        // Right run out
        else if (rightArray.Count < leftArray.Count)
        {
          return 1;
        }

        // No decision
        else
        {
          return 0;
        }
      }

      // Return decision
      return lastOrder.Value;
    }

    // 1 array / 1 number
    else
    {
      return CompareTo(ToArray(left), ToArray(right));
    }
  }

  // Return a JsonNode as an array
  static JsonArray ToArray(JsonNode node)
  {
    if (node is JsonArray array)
    {
      return array;
    }

    return new JsonArray() { JsonNode.Parse(node.ToJsonString()) };
  }
}
