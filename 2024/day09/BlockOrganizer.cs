namespace day09;

public class BlockOrganizer
{
    public List<long> Blocks { get; init; }
    
    public BlockOrganizer(string diskMap)
    {
        // Init blocks
        Blocks = new List<long>();
        bool nextBlocksFree = false;
        int blockId = 0;
        foreach (var b in diskMap)
        {
            long blockCount = long.Parse(b.ToString());
            for (var j = 0; j < blockCount; j++)
            {
                Blocks.Add(nextBlocksFree ? -1 : blockId);
            }
            
            if (!nextBlocksFree)
                blockId++;
            nextBlocksFree = !nextBlocksFree;
        }
    }

    public void Compact()
    {
        for (var currentBlockId = Blocks.Count - 1; currentBlockId >= 0; currentBlockId--)
        {
            var b = Blocks[currentBlockId];
            
            // Skip free blocks
            if (b == -1)
                continue;

            var firstFreeBlockId = Blocks.IndexOf(-1);
            // No more free blocks / stop
            if (firstFreeBlockId == -1)
                break;
            // Reached the end
            if (firstFreeBlockId >= currentBlockId)
                break;
            
            // Move
            Blocks[firstFreeBlockId] = b;
            Blocks[currentBlockId] = -1;
        }
    }

    public long ComputeChecksum()
    {
        return Blocks.Where(b => b != -1).Select((block, fileId) => fileId * block ).Sum();
    }
}