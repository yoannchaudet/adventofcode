// Main

var rounds = ParseInput("input.txt");
var totalScore = rounds.Select(round => GetScore(GetPart1Round(round))).Sum();
Console.WriteLine("Total score (part 1) = {0}", totalScore);

var totalScorePart2 = rounds.Select(round => GetScore(GetPart2Round(round))).Sum();
Console.WriteLine("Total score (part 2) = {0}", totalScorePart2);

// Return selft hand for part 1
static Hand GetSelfHand(string value)
{
  switch (value)
  {
    case "X":
      return Hand.Rock;
    case "Y":
      return Hand.Paper;
    case "Z":
      return Hand.Scissors;
    default:
      throw new Exception("Unknown hand");
  }
}

// Return the opponent hand
static Hand getOpponentHand(string value)
{
  switch (value)
  {
    case "A":
      return Hand.Rock;
    case "B":
      return Hand.Paper;
    case "C":
      return Hand.Scissors;
    default:
      throw new Exception("Unknown hand");
  }
}

static Outcome GetOutcome(Hand self, Hand opponent)
{
  if (self == opponent)
    return Outcome.Draw;
  if (self == Hand.Rock && opponent == Hand.Scissors)
    return Outcome.Win;
  if (self == Hand.Paper && opponent == Hand.Rock)
    return Outcome.Win;
  if (self == Hand.Scissors && opponent == Hand.Paper)
    return Outcome.Win;
  return Outcome.Lose;
}

// Get the score for a given round (part 1)
static Tuple<Hand, Hand> GetPart1Round(Tuple<string, string> round)
{
  return new Tuple<Hand, Hand>(GetSelfHand(round.Item2), getOpponentHand(round.Item1));
}

// Get the hands for the round (part 2)
static Tuple<Hand, Hand> GetPart2Round(Tuple<string, string> round)
{
  var opponent = getOpponentHand(round.Item1);
  var outcome = round.Item2 == "X" ? Outcome.Lose : round.Item2 == "Y" ? Outcome.Draw : Outcome.Win;

  foreach (var hand in Enum.GetValues(typeof(Hand)).Cast<Hand>())
  {
    if (GetOutcome(hand, opponent) == outcome)
      return new Tuple<Hand, Hand>(hand, opponent);
  }

  throw new Exception("Unable to find a hand matching strategy");
}

// Compute the score
static int GetScore(Tuple<Hand, Hand> round)
{
  // Self hand
  int score = 0;
  score += (int)round.Item1;

  // Outcome
  switch (GetOutcome(round.Item1, round.Item2))
  {
    case Outcome.Win:
      score += 6;
      break;
    case Outcome.Draw:
      score += 3;
      break;
    case Outcome.Lose:
      score += 0;
      break;
  }
  return score;
}

// Return the list of rounds
static List<Tuple<string, string>> ParseInput(string input)
{
  return File.ReadAllLines(input)
    .Select(line => line.Split(' '))
    .Select(parts => Tuple.Create(parts[0], parts[1]))
    .ToList();
}

enum Hand
{
  Rock = 1,
  Paper = 2,
  Scissors = 3,
}

enum Outcome
{
  Win = 0,
  Lose,
  Draw,
}