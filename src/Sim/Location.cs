namespace Sim;

public static class Location
{
    public static void GenerateBasicLevel(Grid grid)
    {
        int width = grid.width;
        int height = grid.height;

        // Создаём пол из камня внизу
        int floorHeight = height - 10;
        for (int x = 0; x < width; x++)
        {
            for (int y = floorHeight; y < height; y++)
            {
                grid.PlaceMaterial(x, y, Material.Stone);
            }
        }

        // Добавляем несколько платформ
        // Платформа слева
        for (int x = 20; x < 60; x++)
        {
            grid.PlaceMaterial(x, height - 40, Material.Stone);
        }

        // Платформа справа
        for (int x = width - 60; x < width - 20; x++)
        {
            grid.PlaceMaterial(x, height - 60, Material.Stone);
        }

        // Платформа в центре
        for (int x = width / 2 - 30; x < width / 2 + 30; x++)
        {
            grid.PlaceMaterial(x, height - 80, Material.Stone);
        }
    }

    public static void GenerateEmptyArena(Grid grid)
    {
        int width = grid.width;
        int height = grid.height;

        // Только пол внизу
        int floorHeight = height - 5;
        for (int x = 0; x < width; x++)
        {
            for (int y = floorHeight; y < height; y++)
            {
                grid.PlaceMaterial(x, y, Material.Stone);
            }
        }
    }

    public static void GenerateChunk(Grid grid, int chunkX, int chunkY)
    {
        int width = grid.width;
        int height = grid.height;
        Random rng = new Random(chunkX * 1000 + chunkY);

        // Определяем биом на основе координат чанка
        int biomeSeed = (chunkX + chunkY) % 3;

        switch (biomeSeed)
        {
            case 0: // Равнина с платформами
                GeneratePlains(grid, rng);
                break;
            case 1: // Пещеры
                GenerateCaves(grid, rng);
                break;
            case 2: // Платформенный уровень
                GeneratePlatforms(grid, rng);
                break;
        }
    }

    private static void GeneratePlains(Grid grid, Random rng)
    {
        int width = grid.width;
        int height = grid.height;

        // Пол внизу
        int floorHeight = height - 10;
        for (int x = 0; x < width; x++)
        {
            for (int y = floorHeight; y < height; y++)
            {
                grid.PlaceMaterial(x, y, Material.Stone);
            }
        }

        // Случайные платформы
        for (int i = 0; i < 3; i++)
        {
            int platformX = rng.Next(20, width - 20);
            int platformY = rng.Next(30, floorHeight - 20);
            int platformWidth = rng.Next(30, 60);
            for (int x = platformX; x < platformX + platformWidth && x < width; x++)
            {
                grid.PlaceMaterial(x, platformY, Material.Stone);
            }
        }
    }

    private static void GenerateCaves(Grid grid, Random rng)
    {
        int width = grid.width;
        int height = grid.height;

        // Пол внизу
        int floorHeight = height - 5;
        for (int x = 0; x < width; x++)
        {
            for (int y = floorHeight; y < height; y++)
            {
                grid.PlaceMaterial(x, y, Material.Stone);
            }
        }

        // Стены по бокам
        for (int y = 0; y < height; y++)
        {
            grid.PlaceMaterial(0, y, Material.Stone);
            grid.PlaceMaterial(width - 1, y, Material.Stone);
        }

        // Пещеры - удаляем случайные блоки
        for (int x = 10; x < width - 10; x++)
        {
            for (int y = 20; y < floorHeight - 10; y++)
            {
                if (rng.Next(100) < 30) // 30% шанс пустоты
                {
                    grid.PlaceMaterial(x, y, Material.Empty);
                }
            }
        }
    }

    private static void GeneratePlatforms(Grid grid, Random rng)
    {
        int width = grid.width;
        int height = grid.height;

        // Пол внизу
        int floorHeight = height - 5;
        for (int x = 0; x < width; x++)
        {
            for (int y = floorHeight; y < height; y++)
            {
                grid.PlaceMaterial(x, y, Material.Stone);
            }
        }

        // Множество платформ на разных уровнях
        for (int level = 0; level < 4; level++)
        {
            int platformY = height - 20 - level * 30;
            if (platformY < 20) continue;

            int numPlatforms = rng.Next(2, 5);
            for (int i = 0; i < numPlatforms; i++)
            {
                int platformX = rng.Next(10, width - 50);
                int platformWidth = rng.Next(20, 40);
                for (int x = platformX; x < platformX + platformWidth && x < width; x++)
                {
                    grid.PlaceMaterial(x, platformY, Material.Stone);
                }
            }
        }
    }
}
