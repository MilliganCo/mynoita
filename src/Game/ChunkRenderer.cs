using Microsoft.Xna.Framework.Graphics;
using Sim;
using System.Collections.Generic;

namespace Game;

public class ChunkRenderer
{
    private readonly Dictionary<(int chunkX, int chunkY), Texture2D> chunkTextures;
    private readonly GraphicsDevice graphicsDevice;
    private readonly Microsoft.Xna.Framework.Color[] frameBuffer;

    public ChunkRenderer(GraphicsDevice graphicsDevice, int chunkWidth, int chunkHeight)
    {
        this.graphicsDevice = graphicsDevice;
        this.chunkTextures = new Dictionary<(int chunkX, int chunkY), Texture2D>();
        this.frameBuffer = new Microsoft.Xna.Framework.Color[chunkWidth * chunkHeight];
    }

    public Texture2D GetOrCreateTexture(Chunk chunk)
    {
        var key = (chunk.chunkX, chunk.chunkY);
        
        // Если текстура не существует или чанк изменился, обновляем
        if (!chunkTextures.ContainsKey(key) || chunk.isDirty)
        {
            // Оптимизированная конвертация: прямой рендеринг в MonoGame Color[]
            var grid = chunk.grid;
            int index = 0;
            for (int y = 0; y < 180; y++)
            {
                for (int x = 0; x < 320; x++)
                {
                    var material = grid.GetCell(x, y);
                    
                    // Прямой маппинг без промежуточных конвертаций
                    if (material == Material.Sand)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(255, 200, 0);
                    else if (material == Material.Water)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(0, 100, 255);
                    else if (material == Material.Stone)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(128, 128, 128);
                    else if (material == Material.Fire)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(255, 50, 0);
                    else if (material == Material.Smoke)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(200, 200, 200);
                    else if (material == Material.Oil)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(50, 50, 50);
                    else if (material == Material.Lava)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(255, 100, 0);
                    else if (material == Material.Steam)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(240, 240, 255);
                    else if (material == Material.Ice)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(200, 240, 255);
                    else if (material == Material.Wood)
                        frameBuffer[index] = new Microsoft.Xna.Framework.Color(139, 90, 43);
                    else
                        frameBuffer[index] = Microsoft.Xna.Framework.Color.Black;
                    
                    index++;
                }
            }

            // Создаём или обновляем текстуру
            if (!chunkTextures.ContainsKey(key))
            {
                chunkTextures[key] = new Texture2D(graphicsDevice, 320, 180, false, SurfaceFormat.Color);
            }

            chunkTextures[key].SetData(frameBuffer);
            chunk.isDirty = false;
        }

        return chunkTextures[key];
    }

    public void Dispose()
    {
        foreach (var texture in chunkTextures.Values)
        {
            texture.Dispose();
        }
        chunkTextures.Clear();
    }
}
