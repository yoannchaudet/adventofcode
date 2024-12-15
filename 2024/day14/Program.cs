using day14;

// var planner = new Planner((11, 7), "i_sample.txt");
var planner = new Planner((101, 103), "i_puzzle.txt");

// Part 1
{
    for (var tick = 0; tick < 100; tick++)
        planner.Tick();
    Console.WriteLine($"Part 1 = {planner.GetSafetyFactor()}");
}

// Part 2
{
    Console.WriteLine("Starting to write forever...");
    var output = "/some/local/path.txt";
    var tick = 1;
    using (var writer = new StreamWriter(output))
    {
        while (true)
        {
            writer.WriteLine($"Seconds: {tick}");
            planner.Tick();

            writer.WriteLine(planner.PrintMap());

            if (tick % 100 == 0) writer.Flush();

            tick++;
        }
    }
}