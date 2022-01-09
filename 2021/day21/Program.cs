var inputPath = "./inputs/input.txt";
var game = new Game(inputPath, new DeterministicDie(), 1000);

var winner = game.Play();
var part1 = game.PlayerScores[winner == 0 ? 1 : 0] * game.Die.Rolls;
Console.WriteLine("Part 1 = {0}", part1);

class Game
{
  public int[] PlayerPositions { get; private set; }
  public int[] PlayerScores { get; private set; }
  public Die Die { get; private set; }

  public int MaxScore { get; private set; }

  public Game(string inputPath, Die die, int maxScore)
  {
    // Init players
    PlayerPositions = File.ReadAllLines(inputPath).Select(line => line.Split(':')[1]).Select(int.Parse).Select(x => x - 1).ToArray();

    // Init scores + rest
    PlayerScores = new int[PlayerPositions.Length];
    this.Die = die;
    this.MaxScore = maxScore;
  }

  // Play a game and return the winner
  public int Play()
  {
    // Index of the player that needs to play
    var player = 0;
    do
    {
      // Make the rolls
      var roll = new int[3].Select(i => Die.Roll()[0]).Sum();

      // Increment position
      PlayerPositions[player] = (PlayerPositions[player] + roll) % 10;

      // Increment score
      PlayerScores[player] += (PlayerPositions[player] + 1);

      // Prepare turn for next player
      player = (player + 1) % PlayerPositions.Length;
    } while (PlayerScores[0] < MaxScore && PlayerScores[1] < MaxScore);

    // Return winner
    return PlayerScores[0] >= MaxScore ? 0 : 1;
  }
}

abstract class Die
{
  public int Rolls { get; private set; }

  protected void IncrementRolls()
  {
    Rolls++;
  }

  public abstract int[] Roll();
}

class DeterministicDie : Die
{
  private int _value = 0;

  public override int[] Roll()
  {
    IncrementRolls();
    _value++;
    if (_value > 100)
    {
      _value = 1;
    }
    return new int[] { _value };
  }
}

class DiracDice : Die
{
  public override int[] Roll()
  {
    return new int[] { 1, 2, 3 };
  }
}