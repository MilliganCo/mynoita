namespace Sim;

public class World
{
    private readonly Dictionary<(int chunkX, int chunkY), Chunk> chunks;
    public const int ChunkWidth = 320;
    public const int ChunkHeight = 180;

    public World()
    {
        chunks = new Dictionary<(int chunkX, int chunkY), Chunk>();
    }

    public Chunk GetChunk(int chunkX, int chunkY)
    {
        var key = (chunkX, chunkY);
        if (!chunks.ContainsKey(key))
        {
            var chunk = new Chunk(chunkX, chunkY);
            chunk.Generate();
            chunks[key] = chunk;
        }
        return chunks[key];
    }

    public Material GetCell(int worldX, int worldY)
    {
        int chunkX = worldX / ChunkWidth;
        int chunkY = worldY / ChunkHeight;
        int localX = worldX % ChunkWidth;
        int localY = worldY % ChunkHeight;

        // Обработка отрицательных координат
        if (worldX < 0)
        {
            chunkX = (worldX + 1) / ChunkWidth - 1;
            localX = ((worldX % ChunkWidth) + ChunkWidth) % ChunkWidth;
        }
        if (worldY < 0)
        {
            chunkY = (worldY + 1) / ChunkHeight - 1;
            localY = ((worldY % ChunkHeight) + ChunkHeight) % ChunkHeight;
        }

        var chunk = GetChunk(chunkX, chunkY);
        return chunk.grid.GetCell(localX, localY);
    }

    public void PlaceMaterial(int worldX, int worldY, Material material)
    {
        int chunkX = worldX / ChunkWidth;
        int chunkY = worldY / ChunkHeight;
        int localX = worldX % ChunkWidth;
        int localY = worldY % ChunkHeight;

        // Обработка отрицательных координат
        if (worldX < 0)
        {
            chunkX = (worldX + 1) / ChunkWidth - 1;
            localX = ((worldX % ChunkWidth) + ChunkWidth) % ChunkWidth;
        }
        if (worldY < 0)
        {
            chunkY = (worldY + 1) / ChunkHeight - 1;
            localY = ((worldY % ChunkHeight) + ChunkHeight) % ChunkHeight;
        }

        var chunk = GetChunk(chunkX, chunkY);
        chunk.grid.PlaceMaterial(localX, localY, material);
        chunk.MarkDirty(); // Помечаем чанк как изменённый
    }

    public void Step(float dt, float cameraX, float cameraY, int screenWidth, int screenHeight)
    {
        // Обновляем только видимые чанки для оптимизации (уменьшенная область)
        int startChunkX = ((int)cameraX - screenWidth / 2) / ChunkWidth - 1;
        int endChunkX = ((int)cameraX + screenWidth / 2) / ChunkWidth + 1;
        int startChunkY = ((int)cameraY - screenHeight / 2) / ChunkHeight - 1;
        int endChunkY = ((int)cameraY + screenHeight / 2) / ChunkHeight + 1;

        for (int cx = startChunkX; cx <= endChunkX; cx++)
        {
            for (int cy = startChunkY; cy <= endChunkY; cy++)
            {
                var key = (cx, cy);
                if (chunks.ContainsKey(key))
                {
                    chunks[key].grid.Step(dt);
                    chunks[key].MarkDirty();
                }
            }
        }
    }

    public List<(int chunkX, int chunkY)> GetVisibleChunks(float cameraX, float cameraY, int screenWidth, int screenHeight)
    {
        int startChunkX = ((int)cameraX - screenWidth / 2) / ChunkWidth - 1;
        int endChunkX = ((int)cameraX + screenWidth / 2) / ChunkWidth + 1;
        int startChunkY = ((int)cameraY - screenHeight / 2) / ChunkHeight - 1;
        int endChunkY = ((int)cameraY + screenHeight / 2) / ChunkHeight + 1;

        var visibleChunks = new List<(int chunkX, int chunkY)>();
        for (int cx = startChunkX; cx <= endChunkX; cx++)
        {
            for (int cy = startChunkY; cy <= endChunkY; cy++)
            {
                visibleChunks.Add((cx, cy));
                // Убеждаемся, что чанк загружен
                GetChunk(cx, cy);
            }
        }
        return visibleChunks;
    }
}
