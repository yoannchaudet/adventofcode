var inputPath = "./inputs/input.txt";
var game = new Game(inputPath, new DeterministicDie());
game.Play();

var part1 = game.PlayerScores[game.GetLoser()] * game.Die.Rolls;
Console.WriteLine("Part 1 = {0}", part1);

class Game
{
  public int[] PlayerPositions { get; private set; }
  public int[] PlayerScores { get; private set; }
  public Die Die { get; private set; }

  public Game(string inputPath, Die die)
  {
    // Init players
    PlayerPositions = File.ReadAllLines(inputPath).Select(line => line.Split(':')[1]).Select(int.Parse).Select(x => x - 1).ToArray();

    // Init scores + rest
    PlayerScores = new int[PlayerPositions.Length];
    this.Die = die;
  }

  public void Play()
  {
    // Index of the player that needs to play
    var player = 0;
    do
    {
      // Make the rolls
      var roll = new int[3].Select(i => Die.Roll()).Sum();

      // Increment position
      PlayerPositions[player] = (PlayerPositions[player] + roll) % 10;

      // Increment score
      PlayerScores[player] += (PlayerPositions[player] + 1);

      // Prepare turn for next player
      player = (player + 1) % PlayerPositions.Length;
    } while (GetWinner() == -1);
  }

  // Return the winner or -1 if no winner yet
  private int GetWinner()
  {
    for (var i = 0; i < PlayerScores.Length; i++)
    {
      if (PlayerScores[i] >= 1000)
      {
        return i;
      }
    }
    return -1;
  }

  public int GetLoser()
  {
    // Assuming one player only
    var winner = GetWinner();
    if (winner != -1)
    {
      return winner == 0 ? 1 : 0;
    }
    return -1;
  }
}

abstract class Die
{
  public int Rolls { get; private set; }

  protected void IncrementRolls()
  {
    Rolls++;
  }

  public abstract int Roll();
}

class DeterministicDie : Die
{
  private int _value = 0;

  public override int Roll()
  {
    IncrementRolls();
    _value++;
    if (_value > 100)
    {
      _value = 1;
    }
    return _value;
  }
}