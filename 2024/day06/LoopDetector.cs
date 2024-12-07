namespace day06;

// Be dump, hardcode a limit 🙈
public class LoopDetector()
{
    private int _turns = 0;

    // Tell if a new turn is a loop
    public bool IsLoop((int, int) newTurn)
    {
        _turns++;
        if (_turns > 10000)
        {
            return true;
        }
        return false;
    }
}