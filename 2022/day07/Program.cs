using System.Text.RegularExpressions;

var root = ParseRoot("input.txt");

// Part 1
{
  var largeDirectories = root.GetDirectories().Where(f => f.ComputeSize() <= 100000).ToList();
  var sum = largeDirectories.Sum(f => f.ComputeSize());
  Console.WriteLine("Part 1: {0}", sum);
}

// Parse the input and return the root of the file system
static Node ParseRoot(string input)
{
  // Create a root
  var root = new Node();

  // Regex
  var cdPattern = new Regex(@"^\$ cd (.+)$");
  var filePattern = new Regex(@"^(\d+) (.+)$");
  var folderPattern = new Regex(@"^dir (.+)$");

  // Pointer to current folder
  Node currentFolder = null;

  // All lines in the file
  var lines = File.ReadAllLines(input);

  // Iterate over the input
  for (var i = 0; i < lines.Length; i++)
  {
    var line = lines[i];

    // $ cd
    var cdMatch = cdPattern.Match(line);
    if (cdMatch.Success)
    {
      var folder = cdMatch.Groups[1].Value;
      // Select root
      if (folder == "/")
      {
        currentFolder = root;
      }

      // Select parent
      else if (folder == "..")
      {
        ensureNode(currentFolder);
        currentFolder = currentFolder.Parent;
      }

      // Select child
      else
      {
        ensureNode(currentFolder);
        currentFolder = currentFolder.Files.Single(f => f.Name == folder && f.IsDirectory);
        ensureNode(currentFolder);
      }
    }

    // $ ls
    else if (line == "$ ls")
    {
      // Validate context
      ensureNode(currentFolder);

      // Parse ouput of ls command
      while (i < lines.Length - 1 && !lines[i + 1].StartsWith("$"))
      {
        i++;

        // File
        var match = filePattern.Match(lines[i]);
        if (match.Success)
        {
          currentFolder.Files.Add(new Node(match.Groups[2].Value, int.Parse(match.Groups[1].Value), currentFolder));
          continue;
        }

        match = folderPattern.Match(lines[i]);
        if (!match.Success)
          throw new Exception("Cannot parse line " + lines[i]);
        currentFolder.Files.Add(new Node(match.Groups[1].Value, currentFolder));
      }
    }
  }

  // Return the root node
  return root;
}

static void ensureNode(Node? node)
{
  if (node == null)
    throw new Exception("Invalid command in the context of no folder");
}

class Node
{
  // Size of the node itself (if a file)
  private int size;

  // Cached computed size
  private int computedSize = -1;

  // Return the node name
  public string Name { get; private set; }

  // Is the node a directory?
  public bool IsDirectory { get; private set; }

  // Children of the node (if a directory)
  public List<Node> Files { get; private set; }

  // Parent of the node (null for the root node)
  public Node Parent { get; private set; }

  // Create a new file
  public Node(string name, int size, Node parent)
  {
    this.Parent = parent;
    this.size = size;
    this.Name = name;
    this.IsDirectory = false;
    this.Files = new List<Node>();
  }

  // Create a new folder
  public Node(string name = "/", Node parent = null)
  {
    this.Parent = parent;
    this.size = 0;
    this.Name = name;
    this.IsDirectory = true;
    this.Files = new List<Node>();
  }

  // Return the size of the file + the sum of it's children's sizes
  public int ComputeSize()
  {
    if (computedSize == -1)
    {
      computedSize = size + Files.Sum(f => f.ComputeSize());
    }
    return computedSize;
  }

  // Recursively return the list of directories
  public List<Node> GetDirectories()
  {
    var localDirectories = Files.Where(f => f.IsDirectory).ToList();
    var childDirectories = localDirectories.SelectMany(f => f.GetDirectories()).ToList();
    return localDirectories.Concat(childDirectories).ToList();
  }
}