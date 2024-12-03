const string input = "inputs/sample.txt";
var map = ParseInput();
var (startY, startX) = GetStartingPosition(map);

void GetLoop(List<string> map, (int y, int x) start, List<(int,int)> loop)
{
    var neighbors = GetNeighbors(map, start);
}

// Return the starting position
(int, int) GetStartingPosition(List<string> map)
{
    for (var y = 0; y < map.Count; y++)
    {
        var x = map[y].IndexOf("S");
        if (x != -1)
            return (y, x);
    }
    throw new Exception("No starting position found");
}

// Get the connected neighbors points for a given point
List<(int, int)> GetNeighbors(List<string> map, (int y, int x) location)
{
    var neighbors = new List<(int, int)>();
    var y = location.y;
    var x = location.x;
    
    // north
    if (y >0 && map[y-1][x] == '|') 
        neighbors.Add((y-1, x));
        
    // south
    if (y < map.Count -1 && map[y+1][x] == '|')
        neighbors.Add((y+1, x));
    
    // west
    if (x > 0 && new char[]{'-', 'L', 'F'}.Contains(map[y][x-1] ))
        neighbors.Add((y, x-1));
    
    // east
    if (x < map[y].Length -1 && new char[]{'-','7', 'J'}.Contains(map[y][x+1]))
        neighbors.Add((y, x+1));
    
    return neighbors;
}
    
// Parse the input
List<string> ParseInput()
{
    return File.ReadAllLines(input).ToList();
}

// Get the point one step in a given direction (if any)
(int, int)? GetPoint(List<string> map, (int y, int x) reference, Direction direction)
{
    switch (direction)
    {
        case Direction.North:
            if (reference.y > 0)
                return (reference.y-1, reference.x);
            break;
        case Direction.South:
            if (reference.y < map.Count-1)
                return (reference.y+1, reference.x);
            break;
        case Direction.West:
            if (reference.x > 0)
                return (reference.y, reference.x-1);
            break;
        case Direction.East:
            if (reference.x < map[reference.y].Length-1)
                return (reference.y, reference.x+1);
            break;
    }
    return null;
}

enum Direction
{
    North,
    South,
    West,
    East
}
