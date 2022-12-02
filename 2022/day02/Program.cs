var rounds = ParseInput("input.txt");
var totalScore = rounds.Select(round => GetScore(round)).Sum();
Console.WriteLine("Total score = {0}", totalScore);

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

// Get the score for a given round
static int GetScore(Tuple<string, string> round)
{
  // Self hand
  int score = 0;
  score += (int)GetSelfHand(round.Item2);

  // Outcome
  switch (GetOutcome(GetSelfHand(round.Item2), getOpponentHand(round.Item1)))
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