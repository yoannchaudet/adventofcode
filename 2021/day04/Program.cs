using System.Text.RegularExpressions;
using System.Text;

// Parse one row from the input
static int[] GetRowFromInput(string input)
{
  var row = new int[5];
  var rowExpression = new Regex(@"^(\s*[0-9]+\s*)(\s*[0-9]+\s*)(\s*[0-9]+\s*)(\s*[0-9]+\s*)(\s*[0-9]+\s*)$");
  var match = rowExpression.Match(input);
  if (!match.Success)
  {
    throw new System.Exception("Invalid input: " + input);
  }
  for (var i = 0; i < row.Length; i++)
  {
    row[i] = int.Parse(match.Groups[i + 1].Value);
  }
  return row;
}

// Parse the boards from the input
static List<Board> GetBoards(List<string> inputLines, List<int> drawNumbers)
{
  var boards = new List<Board>();
  int[][] board = null;
  int boardIndex = 0;
  for (int i = 1; i < inputLines.Count; i++)
  {
    if (inputLines[i].Trim() == "")
    {
      board = new int[5][];
      boardIndex = 0;
    }
    else
    {
      board[boardIndex++] = GetRowFromInput(inputLines[i]);
      if (boardIndex == 5)
      {
        boards.Add(new Board(board, drawNumbers));
      }
    }
  }
  return boards;
}


// Parse the input
var inputLines = File.ReadAllLines("./inputs/input.txt");
var drawnNumbers = inputLines.First().Split(',').Select(int.Parse).ToList();
var boards = GetBoards(inputLines.ToList(), drawnNumbers);

var winningBoard = boards.MinBy(board => board.VictoryTurn);
Console.WriteLine("Result = {0}", winningBoard.GetScore());

var lastWinningBoard = boards.MaxBy(board => board.VictoryTurn);
Console.WriteLine("Result = {0}", lastWinningBoard.GetScore());

class Board
{
  // The actual board.
  public int[][] Matrix { get; private set; }

  public List<int> DrawnNumbers { get; private set; }

  // Rows and columns.
  public List<List<int>> Lines { get; private set; }

  // Number of turn before victory (0 for no victory).
  public int VictoryTurn { get; private set; }

  public Board(int[][] matrix, List<int> drawnNumbers)
  {
    Matrix = matrix;
    DrawnNumbers = drawnNumbers;
    InitializeLines();
    InitializeVictoryTurn();
  }

  private void InitializeLines() {
    // Set lines.
    var lines = new List<List<int>>();
    for (int i = 0; i < Matrix.Length; i++)
    {
      lines.Add(Matrix[i].ToList<int>());
      var column = new List<int>();
      for (int j = 0; j < Matrix.Length; j++)
      {
        column.Add(Matrix[j][i]);
      }
      lines.Add(column);
    }
    Lines = lines;
  }

  private void InitializeVictoryTurn()
  {
    // Start at 5.
    for (int i = 5; i < DrawnNumbers.Count; i++)
    {
      var currentDrawn = DrawnNumbers.Take(i);
      foreach (var line in Lines)
      {
        if (line.All(x => currentDrawn.Contains(x)))
        {
          VictoryTurn = i;
          return;
        }
      }
    }
  }

  // Assuming this board is the winning one, get the score.
  public int GetScore() {
    var currentDrawn = DrawnNumbers.Take(VictoryTurn);
    var unmarkedNumbersSum = Matrix.SelectMany(i => i).Where(i => !currentDrawn.Contains(i)).Sum();
    return unmarkedNumbersSum * currentDrawn.Last();
  }
}