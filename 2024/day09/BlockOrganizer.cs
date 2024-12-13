namespace day09;

public class BlockOrganizer
{
    public BlockOrganizer(string diskMap)
    {
        // Init blocks
        Blocks = new List<long>();
        var nextBlocksFree = false;
        var blockId = 0;
        foreach (var b in diskMap)
        {
            var blockCount = long.Parse(b.ToString());
            for (var j = 0; j < blockCount; j++) Blocks.Add(nextBlocksFree ? -1 : blockId);

            if (!nextBlocksFree) blockId++;
            nextBlocksFree = !nextBlocksFree;
        }
    }

    private List<long> Blocks { get; }

    public void CompactPart1()
    {
        for (var currentBlockId = Blocks.Count - 1; currentBlockId >= 0; currentBlockId--)
        {
            var block = Blocks[currentBlockId];

            // Skip free blocks
            if (block == -1) continue;

            // Find the first free block
            var firstFreeBlockId = Blocks.IndexOf(-1);

            // No more free blocks / stop
            if (firstFreeBlockId == -1) break;
            // Reached the end
            if (firstFreeBlockId >= currentBlockId) break;

            // Move
            Blocks[firstFreeBlockId] = block;
            Blocks[currentBlockId] = -1;
        }
    }

    public void CompactPart2()
    {
        for (var currentBlockId = Blocks.Count - 1; currentBlockId >= 0; currentBlockId--)
        {
            var block = Blocks[currentBlockId];
            var blockLength = RepeatingBlocksBackward(currentBlockId);

            // Skip free blocks
            if (block == -1) continue;

            // Find the first free block
            var firstFreeBlockId = GetFirstFreeBlockId(blockLength);

            // No more free blocks / stop
            if (!(firstFreeBlockId == -1 || firstFreeBlockId >= currentBlockId))
                // Move
                for (var i = 0; i < blockLength; i++)
                {
                    Blocks[firstFreeBlockId + i] = block;
                    Blocks[currentBlockId - i] = -1;
                }

            currentBlockId -= blockLength - 1;
        }
    }

    public long ComputeChecksum()
    {
        return Blocks.Select((block, fileId) => block == -1 ? 0 : fileId * block).Sum();
    }

    private int GetFirstFreeBlockId(int minimumLength)
    {
        for (var i = 0; i < Blocks.Count; i++)
            if (Blocks[i] == -1)
            {
                var blockLength = RepeatingBlocksForward(i);
                if (blockLength >= minimumLength)
                    return i;
                i += blockLength - 1;
            }

        return -1;
    }

    // Return the number of repeating blocks starting at given index and going forward
    private int RepeatingBlocksForward(int blockIndex)
    {
        if (blockIndex == -1) return 0;
        var block = Blocks[blockIndex];
        var repeatingBlocks = 1;
        for (var i = blockIndex + 1; i < Blocks.Count; i++)
        {
            if (Blocks[i] != block) break;
            repeatingBlocks++;
        }

        return repeatingBlocks;
    }

    // Return the number of repeating blocks starting at given index and going forward
    private int RepeatingBlocksBackward(int blockIndex)
    {
        if (blockIndex == -1) return 0;
        var block = Blocks[blockIndex];
        var repeatingBlocks = 1;
        for (var i = blockIndex - 1; i >= 0; i--)
        {
            if (Blocks[i] != block) break;
            repeatingBlocks++;
        }

        return repeatingBlocks;
    }
}