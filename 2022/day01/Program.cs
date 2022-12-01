var input = "input.txt";
var caloriesPerElf = GetCaloriesPerElf(input);
var top1TotalCalories = caloriesPerElf.Max();
Console.WriteLine("Top 1 total calories = {0}", top1TotalCalories);
var top3Calories = caloriesPerElf.OrderByDescending(x => x).Take(3).Sum();
Console.WriteLine("Top 3 total calories = {0}", top3Calories);

static List<int> GetCaloriesPerElf(string input)
{
  var caloriesPerElf = new List<int>();
  int currentCalories = 0;
  foreach (var line in File.ReadAllLines(input))
  {
    if (String.IsNullOrEmpty(line))
    {
      caloriesPerElf.Add(currentCalories);
      currentCalories = 0;
    }
    else
    {
      currentCalories += int.Parse(line);
    }
  }
  // last line
  caloriesPerElf.Add(currentCalories);
  return caloriesPerElf;
}
