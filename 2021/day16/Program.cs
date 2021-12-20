using System.Text;

// I don't like binary, let's use strings :troll:

var inputPath = "./inputs/input.txt";

var input = ReadInput(inputPath);
var packet = new PacketParser(input).Parse();
Console.WriteLine("Sum of versions: {0}", SumVersions(packet));

static int SumVersions(Packet packet) {
  var sum = 0;
  sum += packet.PacketVersion;
  if (packet is OperatorPacket op) {
    foreach (var operand in op.Operands) {
      sum += SumVersions(operand);
    }
  }
  return sum;
}

// Return the input as a long string of 0 and 1.
static string ReadInput(string inputPath)
{
  var bytes = File.ReadAllText(inputPath).Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'));
  var sb = new StringBuilder();
  foreach (var b in bytes)
  {
    sb.Append(b);
  }
  return sb.ToString();
}

class PacketParser
{
  // Input being parsed
  private string input;
  // Current position in the input
  private int position = 0;

  public PacketParser(string input)
  {
    this.input = input;
  }

  private int ReadNumber(int length)
  {
    var number = Convert.ToInt32(input.Substring(position, length), 2);
    position += length;
    return number;
  }

  private long ReadLiteral()
  {
    var literal = new StringBuilder();
    var lastGroup = false;
    while (true)
    {
      lastGroup = input[position] == '0';
      literal.Append(input.Substring(position + 1, 4));
      position += 5;
      if (lastGroup)
      {
        break;
      }
    }
    return Convert.ToInt64(literal.ToString(), 2);
  }

  public Packet Parse()
  {
    // Header
    var packetVersion = ReadNumber(3);
    var typeId = ReadNumber(3);

    // Literal
    if (typeId == 4)
    {
      var literal = ReadLiteral();
      return new LiteralPacket(packetVersion, typeId, literal);
    }

    // Operator
    else
    {
      var packet = new OperatorPacket(packetVersion, typeId);

      // Read n packets
      if (ReadNumber(1) == 1)
      {
        var subpacketsCount = ReadNumber(11);
        for (var i = 1; i <= subpacketsCount; i++)
        {
          packet.Operands.Add(Parse());
        }
      }

      // Read n bytes worth of packets
      else
      {
        var subpacketsLength = ReadNumber(15);
        var initialPosition = position;
        while (position < initialPosition + subpacketsLength)
        {
          packet.Operands.Add(Parse());
        }
      }

      return packet;
    }

    throw new Exception("Unknown packet type");
  }
}

class Packet
{
  public int PacketVersion { get; set; }
  public int TypeId { get; set; }

  public Packet(int packetVersion, int typeId)
  {
    PacketVersion = packetVersion;
    TypeId = typeId;
  }
}

class LiteralPacket : Packet
{
  public long Literal { get; set; }

  public LiteralPacket(int packetVersion, int typeId, long literal) : base(packetVersion, typeId)
  {
    Literal = literal;
  }
}

class OperatorPacket : Packet
{
  public List<Packet> Operands { get; private set; }

  public OperatorPacket(int packetVersion, int typeId) : base(packetVersion, typeId)
  {
    Operands = new List<Packet>();
  }
}