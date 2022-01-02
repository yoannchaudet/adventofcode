﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

var inputPath = "./inputs/sample.txt";
var map = ReadInput(inputPath);

// The set of beacons in absolute positions (with scanner 0 at the origin)
// Init it with the beacons of scanner 0
var beacons = new HashSet<Point>();
foreach (var point in map[0])
{
  beacons.Add(point);
}

// List of scanners that have been stitched together
var mappedScanners = new List<int>();
mappedScanners.Add(0);

// While there are still scanners to map
loop:
while (mappedScanners.Count < map.Length)
{
  // Go over already mapped scanners
  foreach (var mappedScanner in mappedScanners)
  {
    // Iterate through all points in the mapped scanner, use them as reference point
    foreach (var referencePoint in map[mappedScanner])
    {
      // Go over all scanners that have not been mapped yet
      var availableScanners = map.Select((x, i) => i).Where(x => !mappedScanners.Contains(x));
      foreach (var candidateScanner in availableScanners)
      {
        // Go over all points in the available scanner
        foreach (var candidateReferencePoint in map[candidateScanner])
        {
          // Transpose both scanners (current and candidate) to an origin (that is immune to rotation) and compare
          // common points
          var transposedScanner = GetTransposedPoints(map[mappedScanner], referencePoint);
          var transposedCandidateScanner = GetTransposedPoints(map[candidateScanner], candidateReferencePoint);
          var commonPoints = GetCommonPoints(transposedScanner, transposedCandidateScanner);

          // If more than 12 points match, we have found an overlapping cube
          if (commonPoints.Item1 >= 12)
          {
            // Add the candidate scanner to the list of mapped scanners
            Console.WriteLine("Stitching scanner {0} (with {1})", candidateScanner, mappedScanner);
            mappedScanners.Add(candidateScanner);

            // Get the candidate scanner in the correct orientation
            var orientedCandidateScanner = transposedCandidateScanner.Select(x => x.GetAllCoordinates()[commonPoints.Item2]).ToArray();

            // Transpose the oriented candidate scanner back to the scanner's reference point
            // TODO: we need to transpose back to scanner0's referential...
            var transposedOrientedCandidateScanner = GetTransposedPoints(orientedCandidateScanner, referencePoint);

            // Add the beacons to the global map
            foreach (var point in transposedOrientedCandidateScanner)
            {
              if (!beacons.Contains(point))
              {
                beacons.Add(point);
              }
              else
              {
                Console.WriteLine(point.ToString());
              }
            }

            goto loop;
          }
        }
      }
    }
  }
}

// foreach (var referencePoint in map[0])
// {
//   Console.WriteLine("ref point: " + referencePoint);
//   for (var scanner = 1; scanner < map.Length; scanner++)
//   {
//     Console.WriteLine("scanner " + scanner);
//     foreach (var localReferencePoint in map[scanner])
//     {
//       var transposedScannerZero = GetTransposedPoints(map[0], referencePoint);
//       var transposedScannerN = GetTransposedPoints(map[scanner], localReferencePoint);
//       var commonPoints = GetCommonPoints(transposedScannerZero, transposedScannerN);
//       Console.WriteLine("Common points with scanner {2}: {0} (orientation {1})", commonPoints.Item1, commonPoints.Item2, scanner);
//     }
//   }
// }

Console.WriteLine("Beacons: " + beacons.Count);

// Return the maximum number of common points and the orientation between two arrays of points
static (int, int) GetCommonPoints(Point[] a, Point[] b)
{
  var commonPoints = new int[24];
  foreach (var pointA in a)
  {
    foreach (var pointB in b)
    {
      var allPointB = pointB.GetAllCoordinates();
      for (var orientation = 0; orientation < 24; orientation++)
      {
        if (allPointB[orientation].Equals(pointA))
        {
          commonPoints[orientation]++;
        }
      }
    }
  }
  return commonPoints.Select((x, i) => (x, i)).OrderByDescending(x => x.x).First();
}


// Return a list of points transposed by a reference point
static Point[] GetTransposedPoints(Point[] points, Point reference)
{
  return points.Select(point => new Point(reference.X - point.X, reference.Y - point.Y, reference.Z - point.Z)).ToArray();
}

// Return a map of the beacons per scanner
static Point[][] ReadInput(string inputPath)
{
  var scannerExpression = new Regex("^--- scanner ([0-9]+) ---$");

  var map = new Dictionary<int, List<Point>>();
  int? currentScanner = null;

  foreach (var line in File.ReadLines(inputPath))
  {
    // Ignore blank lines
    if (line.Trim() == "")
    {
      continue;
    }

    // Scanner
    var match = scannerExpression.Match(line);
    if (match.Success)
    {
      currentScanner = int.Parse(match.Groups[1].Value);
      map.Add(currentScanner.Value, new List<Point>());
    }

    // Point
    else if (currentScanner.HasValue)
    {
      var parts = line.Split(',').Select(int.Parse).ToArray();
      map[currentScanner.Value].Add(new Point(parts[0], parts[1], parts[2]));
    }
  }

  var points = new Point[map.Count][];
  map.Select((kvp, i) => new { kvp, i }).ToList().ForEach(x => points[x.i] = x.kvp.Value.ToArray());
  return points;
}

struct Point : IEqualityComparer<Point>
{
  public int X { get; private set; }
  public int Y { get; private set; }
  public int Z { get; private set; }

  public Point(int x, int y, int z)
  {
    X = x;
    Y = y;
    Z = z;
  }

  public bool Equals(Point a, Point b)
  {
    return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
  }

  public int GetHashCode([DisallowNull] Point obj)
  {
    return HashCode.Combine(obj.X, obj.Y, obj.Z);
  }

  public string ToString()
  {
    return $"({X}, {Y}, {Z})";
  }

  // Return all 24 possible coordinates for that specific point
  public Point[] GetAllCoordinates()
  {
    // Using each face of a die and a 3-d coordinate system, rotate clockwise (4 times) on each face to collect all possible coordinates
    return new Point[] {
      // Face 1
      new Point(X, Y, Z),
      new Point(Y, -X, Z),
      new Point(-X, -Y, Z),
      new Point(-Y, X, Z),

      // Face 2
      new Point(-Z, Y, X),
      new Point(Y, Z, X),
      new Point(Z, -Y, X),
      new Point(-Y,- Z, X),

      // Face 3
      new Point(X, Z, -Y),
      new Point(Z, -X, -Y),
      new Point(-X, -Z, -Y),
      new Point(-Z, X, -Y),

      // Face 4
      new Point(X, -Z, Y),
      new Point(-Z, -X, Y),
      new Point(-X, Z, Y),
      new Point(Z, X, Y),

      // Face 5
      new Point(Z, Y, -X),
      new Point(Y, -Z, -X),
      new Point(-Z, -Y, -X),
      new Point(-Y, Z, -X),

      // Face 6
      new Point(-X, Y, -Z),
      new Point(Y, X, -Z),
      new Point(X, -Y, -Z),
      new Point(-Y, -X, -Z),
    };
  }
}
