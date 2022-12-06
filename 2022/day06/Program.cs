// Part 1
var buffer = File.ReadAllText("input.txt");
{
  var firstPacketMarker = GetFirstMarkerPosition(buffer, 4);
  Console.WriteLine("Part 1: {0}", firstPacketMarker);
}

// Part 2
{
  var firstMessageMarker = GetFirstMarkerPosition(buffer, 14);
  Console.WriteLine("Part 1: {0}", firstMessageMarker);
}

// Return the position of the first marker
static int GetFirstMarkerPosition(string buffer, int markerLength = 4)
{
  for (int i = Math.Min(markerLength, buffer.Length); i < buffer.Length; i++)
  {
    if (buffer[(i - markerLength)..i].Distinct().Count() == markerLength)
    {
      return i;
    }
  }
  return -1;
}