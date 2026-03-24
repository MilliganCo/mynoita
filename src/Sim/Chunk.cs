namespace Sim;

public class Chunk
{
    public readonly int chunkX;
    public readonly int chunkY;
    public readonly Grid grid;
    public bool isGenerated;
    public bool isDirty; // Флаг изменения для оптимизации рендеринга

    public Chunk(int chunkX, int chunkY)
    {
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.grid = new Grid(320, 180, seed: chunkX * 1000 + chunkY);
        this.isGenerated = false;
        this.isDirty = true; // Новый чанк нужно отрендерить
    }

    public void Generate()
    {
        if (isGenerated) return;
        Location.GenerateChunk(grid, chunkX, chunkY);
        isGenerated = true;
        isDirty = true;
    }

    public void MarkDirty()
    {
        isDirty = true;
    }
}
