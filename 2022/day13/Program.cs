using System.Text.Json.Nodes;

var input = "input.txt";

// Part 1
{
  var pairs = ParseInput(input).ToList();
  var indices = new List<int>();
  for (var i = 0; i < pairs.Count; i++)
  {
    var pair = pairs[i];
    if (CompareOrder(pair.Item1, pair.Item2) == -1)
    {
      indices.Add(i + 1);
    }
  }
  Console.WriteLine("Part 1: {0}", indices.Sum());
}

// Part 2
{

}

static int CompareOrder(JsonNode left, JsonNode right)
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
      lastOrder = CompareOrder(leftArray[i], rightArray[i]);

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
    return CompareOrder(ToArray(left), ToArray(right));
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

static IEnumerable<Tuple<JsonArray, JsonArray>> ParseInput(string input)
{
  var lines = File.ReadAllLines(input);
  for (var i = 0; i < lines.Length; i++)
  {
    yield return Tuple.Create(ParsePacket(lines[i]), ParsePacket(lines[++i]));
    i++;
  }
}

// Deserialize a single packet
static JsonArray ParsePacket(string line)
{
  var node = JsonNode.Parse(line);
  if (node is JsonArray array)
  {
    return array;
  }
  throw new Exception("Invalid packet: " + line);
}

class Packet
{
  public List<Packet> Values { get; private set; }

  public int? Value { get; private set; }

  public Packet(int value)
  {
    Value = value;
    Values = null;
  }

  public Packet(Packet parent, IEnumerable<Packet> values)
  {
    Value = null;
    Values = values.ToList();
  }
}
