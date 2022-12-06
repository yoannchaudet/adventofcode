// Part 1
var firstMarker = GetFirstMarkerPosition(File.ReadAllText("input.txt"));
Console.WriteLine("Part 1: {0}", firstMarker);


// Return the position of the first marker
static int GetFirstMarkerPosition(string buffer)
{
  for (int i = Math.Min(4, buffer.Length); i < buffer.Length; i++)
  {
    if (buffer[(i - 4)..i].Distinct().Count() == 4) {
      return i;
    }
  }
  return -1;
}