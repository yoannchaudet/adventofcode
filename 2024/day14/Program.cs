using day14;

// var planner = new Planner((11, 7), "i_sample.txt");
var planner = new Planner((101, 103), "i_puzzle.txt");

// Part 1
planner.PrintMap();
for (var tick = 0; tick < 100; tick++)
    planner.Tick();
planner.PrintMap();
Console.WriteLine($"Part 1 = {planner.GetSafetyFactor()}");