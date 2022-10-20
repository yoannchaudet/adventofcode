var inputPath = "./inputs/sample.txt";
var game = new Game(inputPath, new DeterministicDie(), 1000);

var winner = game.Play();
var part1 = game.PlayerScores[winner == 0 ? 1 : 0] * game.Die.Rolls;
Console.WriteLine("Part 1 = {0}", part1);

var sums = new List<int>();
foreach (var roll1 in new[] { 1, 2, 3 })
{
  foreach (var roll2 in new[] { 1, 2, 3 })
  {
    foreach (var roll3 in new[] { 1, 2, 3 })
    {
      sums.Add(roll1 + roll2 + roll3);
    }
  }
}
var rolls = sums.GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() }).Select(s => (s.Key, s.Count)).ToArray();

var positions = File.ReadAllLines(inputPath).Select(line => line.Split(':')[1]).Select(int.Parse).Select(x => x - 1).ToArray();
var wins = (0L, 0L);
DiracGame((0, 0), (positions[0], positions[1]), (1, 1), ref wins, ref rolls);
Console.WriteLine("Part 2 = {0}", wins.Item1 > wins.Item2 ? wins.Item1 : wins.Item2);

// Play the Dirac game, return the number of wins for each player
static void DiracGame((int, int) scores, (int, int) positions, (long, long) universes, ref (long, long) wins, ref (int, int)[] rolls)
{
  // Winner
  if (scores.Item1 >= 21)
  {
    wins.Item1 += universes.Item1;
    return;
  }
  if (scores.Item2 >= 21)
  {
    wins.Item2 += universes.Item2;
    return;
  }

  // Play the game
  foreach (var rollPlayer1 in rolls)
  {
    foreach (var rollPlayer2 in rolls)
    {
      var nextPosition = (
        (positions.Item1 + rollPlayer1.Item1) % 10,
        (positions.Item2 + rollPlayer2.Item1) % 10);
      var nextScores = (
        scores.Item1 + nextPosition.Item1 + 1,
        scores.Item2 + nextPosition.Item2 + 1);
      var nextUniverses = (
        universes.Item1 * rollPlayer1.Item2,
        universes.Item2 * rollPlayer2.Item2);
      DiracGame(nextScores, nextPosition, nextUniverses, ref wins, ref rolls);
    }
  }
}

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