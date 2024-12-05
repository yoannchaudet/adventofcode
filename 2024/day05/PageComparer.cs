namespace day05;

public class PageComparer(List<(int, int)> orders) : IComparer<int>
{
    public int Compare(int x, int y)
    {
        if (x == y) return 0;

        if (orders.Contains((x, y))) return -1;

        return 1;
    }
}