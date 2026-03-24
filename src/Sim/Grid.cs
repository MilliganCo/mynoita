using System.Drawing;

namespace Sim;

public class Grid
{
    public readonly int width;
    public readonly int height;
    private readonly Cell[,] cells;
    private readonly Random rng;

    public Grid(int width, int height, int seed = 0)
    {
        this.width = width;
        this.height = height;
        this.cells = new Cell[height, width];
        this.rng = new Random(seed);

        // Инициализация всех клеток как Empty
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = new Cell();
            }
        }
    }

    public void Step(float dt)
    {
        // Используем Dictionary для избежания конфликтов при обновлении
        var updates = new Dictionary<(int x, int y), Cell>();
        var processed = new HashSet<(int x, int y)>();

        // Проход снизу вверх для правильной обработки падающих материалов
        // Оптимизация: пропускаем Empty и Stone (статические материалы)
        for (int y = height - 2; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (processed.Contains((x, y))) continue;

                var cell = cells[y, x];
                
                // Пропускаем статические материалы для оптимизации
                if (cell.material == Material.Empty || cell.material == Material.Stone)
                    continue;

                switch (cell.material)
                {
                    case Material.Sand:
                        UpdateSand(x, y, updates, processed);
                        break;
                    case Material.Water:
                        UpdateWater(x, y, updates, processed);
                        break;
                    case Material.Oil:
                        UpdateOil(x, y, updates, processed);
                        break;
                    case Material.Lava:
                        UpdateLava(x, y, updates, processed);
                        break;
                }
            }
        }

        // Обработка Fire и Smoke во всех направлениях
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (processed.Contains((x, y))) continue;

                var cell = cells[y, x];
                
                // Пропускаем статические материалы
                if (cell.material == Material.Empty || cell.material == Material.Stone)
                    continue;

                switch (cell.material)
                {
                    case Material.Fire:
                        UpdateFire(x, y, updates, processed);
                        break;
                    case Material.Smoke:
                        UpdateSmoke(x, y, updates, processed);
                        break;
                    case Material.Steam:
                        UpdateSteam(x, y, updates, processed);
                        break;
                    case Material.Ice:
                        UpdateIce(x, y, updates, processed);
                        break;
                    case Material.Wood:
                        UpdateWood(x, y, updates, processed);
                        break;
                }
            }
        }

        // Применяем все обновления
        foreach (var kvp in updates)
        {
            var (x, y) = kvp.Key;
            cells[y, x] = kvp.Value;
        }
    }

    private void UpdateSand(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        // Проверяем клетку ниже
        if (y + 1 < height && cells[y + 1, x].material == Material.Empty)
        {
            // Перемещаем вниз
            updates[(x, y)] = new Cell();
            updates[(x, y + 1)] = new Cell(Material.Sand, 0, 0);
            processed.Add((x, y));
            processed.Add((x, y + 1));
            return;
        }

        // Если внизу занято, проверяем диагонали
        bool leftDown = y + 1 < height && x - 1 >= 0 && cells[y + 1, x - 1].material == Material.Empty;
        bool rightDown = y + 1 < height && x + 1 < width && cells[y + 1, x + 1].material == Material.Empty;

        if (leftDown && rightDown)
        {
            // Случайный выбор
            if (rng.Next(2) == 0)
            {
                updates[(x, y)] = new Cell();
                updates[(x - 1, y + 1)] = new Cell(Material.Sand, 0, 0);
                processed.Add((x, y));
                processed.Add((x - 1, y + 1));
            }
            else
            {
                updates[(x, y)] = new Cell();
                updates[(x + 1, y + 1)] = new Cell(Material.Sand, 0, 0);
                processed.Add((x, y));
                processed.Add((x + 1, y + 1));
            }
        }
        else if (leftDown)
        {
            updates[(x, y)] = new Cell();
            updates[(x - 1, y + 1)] = new Cell(Material.Sand, 0, 0);
            processed.Add((x, y));
            processed.Add((x - 1, y + 1));
        }
        else if (rightDown)
        {
            updates[(x, y)] = new Cell();
            updates[(x + 1, y + 1)] = new Cell(Material.Sand, 0, 0);
            processed.Add((x, y));
            processed.Add((x + 1, y + 1));
        }
    }

    private void UpdateWater(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        // Проверяем клетку ниже
        if (y + 1 < height && cells[y + 1, x].material == Material.Empty)
        {
            updates[(x, y)] = new Cell();
            updates[(x, y + 1)] = new Cell(Material.Water, 0, 0);
            processed.Add((x, y));
            processed.Add((x, y + 1));
            return;
        }

        // Если внизу занято, проверяем стороны
        bool left = x - 1 >= 0 && cells[y, x - 1].material == Material.Empty;
        bool right = x + 1 < width && cells[y, x + 1].material == Material.Empty;

        if (left && right)
        {
            // Случайный выбор
            if (rng.Next(2) == 0)
            {
                updates[(x, y)] = new Cell();
                updates[(x - 1, y)] = new Cell(Material.Water, 0, 0);
                processed.Add((x, y));
                processed.Add((x - 1, y));
            }
            else
            {
                updates[(x, y)] = new Cell();
                updates[(x + 1, y)] = new Cell(Material.Water, 0, 0);
                processed.Add((x, y));
                processed.Add((x + 1, y));
            }
        }
        else if (left)
        {
            updates[(x, y)] = new Cell();
            updates[(x - 1, y)] = new Cell(Material.Water, 0, 0);
            processed.Add((x, y));
            processed.Add((x - 1, y));
        }
        else if (right)
        {
            updates[(x, y)] = new Cell();
            updates[(x + 1, y)] = new Cell(Material.Water, 0, 0);
            processed.Add((x, y));
            processed.Add((x + 1, y));
        }
    }

    private void UpdateFire(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        if (processed.Contains((x, y))) return;

        var cell = cells[y, x];
        var newCell = new Cell(Material.Fire, cell.temperature, cell.age + 1);

        // Проверяем соседние клетки на Water для испарения
        if (y + 1 < height && cells[y + 1, x].material == Material.Water)
        {
            updates[(x, y + 1)] = new Cell(Material.Smoke, 0, 0);
        }
        if (y - 1 >= 0 && cells[y - 1, x].material == Material.Water)
        {
            updates[(x, y - 1)] = new Cell(Material.Smoke, 0, 0);
        }
        if (x + 1 < width && cells[y, x + 1].material == Material.Water)
        {
            updates[(x + 1, y)] = new Cell(Material.Smoke, 0, 0);
        }
        if (x - 1 >= 0 && cells[y, x - 1].material == Material.Water)
        {
            updates[(x - 1, y)] = new Cell(Material.Smoke, 0, 0);
        }

        // Если возраст >= 60, превращаем в Smoke
        if (newCell.age >= 60)
        {
            updates[(x, y)] = new Cell(Material.Smoke, 0, 0);
        }
        else
        {
            updates[(x, y)] = newCell;
        }

        processed.Add((x, y));
    }

    private void UpdateSmoke(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        if (processed.Contains((x, y))) return;

        var cell = cells[y, x];
        var newCell = new Cell(Material.Smoke, cell.temperature, cell.age + 1);

        // Если возраст >= 120, исчезает
        if (newCell.age >= 120)
        {
            updates[(x, y)] = new Cell();
            processed.Add((x, y));
            return;
        }

        // Поднимается вверх
        if (y - 1 >= 0 && cells[y - 1, x].material == Material.Empty)
        {
            updates[(x, y)] = new Cell();
            updates[(x, y - 1)] = newCell;
            processed.Add((x, y));
            processed.Add((x, y - 1));
        }
        else
        {
            updates[(x, y)] = newCell;
            processed.Add((x, y));
        }
    }

    public void RenderTo(Color[] buffer)
    {
        // Оптимизированная версия с прямым маппингом цветов
        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var material = cells[y, x].material;
                
                // Прямой маппинг без switch для скорости
                if (material == Material.Sand)
                    buffer[index] = Color.FromArgb(255, 255, 200, 0);
                else if (material == Material.Water)
                    buffer[index] = Color.FromArgb(255, 0, 100, 255);
                else if (material == Material.Stone)
                    buffer[index] = Color.FromArgb(255, 128, 128, 128);
                else if (material == Material.Fire)
                    buffer[index] = Color.FromArgb(255, 255, 50, 0);
                else if (material == Material.Smoke)
                    buffer[index] = Color.FromArgb(255, 200, 200, 200);
                else if (material == Material.Oil)
                    buffer[index] = Color.FromArgb(255, 50, 50, 50);
                else if (material == Material.Lava)
                    buffer[index] = Color.FromArgb(255, 255, 100, 0);
                else if (material == Material.Steam)
                    buffer[index] = Color.FromArgb(255, 240, 240, 255);
                else if (material == Material.Ice)
                    buffer[index] = Color.FromArgb(255, 200, 240, 255);
                else if (material == Material.Wood)
                    buffer[index] = Color.FromArgb(255, 139, 90, 43);
                else
                    buffer[index] = Color.Black;
                
                index++;
            }
        }
    }


    private void UpdateOil(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        // Oil течёт как вода, но медленнее
        if (processed.Contains((x, y))) return;

        // Проверяем клетку ниже
        if (y + 1 < height && cells[y + 1, x].material == Material.Empty)
        {
            updates[(x, y)] = new Cell();
            updates[(x, y + 1)] = new Cell(Material.Oil, 0, 0);
            processed.Add((x, y));
            processed.Add((x, y + 1));
            return;
        }

        // Если внизу занято, проверяем стороны (реже чем вода)
        if (rng.Next(3) == 0) // Медленнее чем вода
        {
            bool left = x - 1 >= 0 && cells[y, x - 1].material == Material.Empty;
            bool right = x + 1 < width && cells[y, x + 1].material == Material.Empty;

            if (left && right)
            {
                if (rng.Next(2) == 0)
                {
                    updates[(x, y)] = new Cell();
                    updates[(x - 1, y)] = new Cell(Material.Oil, 0, 0);
                    processed.Add((x, y));
                    processed.Add((x - 1, y));
                }
                else
                {
                    updates[(x, y)] = new Cell();
                    updates[(x + 1, y)] = new Cell(Material.Oil, 0, 0);
                    processed.Add((x, y));
                    processed.Add((x + 1, y));
                }
            }
            else if (left)
            {
                updates[(x, y)] = new Cell();
                updates[(x - 1, y)] = new Cell(Material.Oil, 0, 0);
                processed.Add((x, y));
                processed.Add((x - 1, y));
            }
            else if (right)
            {
                updates[(x, y)] = new Cell();
                updates[(x + 1, y)] = new Cell(Material.Oil, 0, 0);
                processed.Add((x, y));
                processed.Add((x + 1, y));
            }
        }
    }

    private void UpdateLava(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        // Lava падает медленнее песка, поджигает материалы
        if (processed.Contains((x, y))) return;

        // Поджигаем соседние горючие материалы
        if (y + 1 < height && (cells[y + 1, x].material == Material.Wood || cells[y + 1, x].material == Material.Oil))
        {
            updates[(x, y + 1)] = new Cell(Material.Fire, 0, 0);
        }
        if (y - 1 >= 0 && (cells[y - 1, x].material == Material.Wood || cells[y - 1, x].material == Material.Oil))
        {
            updates[(x, y - 1)] = new Cell(Material.Fire, 0, 0);
        }
        if (x + 1 < width && (cells[y, x + 1].material == Material.Wood || cells[y, x + 1].material == Material.Oil))
        {
            updates[(x + 1, y)] = new Cell(Material.Fire, 0, 0);
        }
        if (x - 1 >= 0 && (cells[y, x - 1].material == Material.Wood || cells[y, x - 1].material == Material.Oil))
        {
            updates[(x - 1, y)] = new Cell(Material.Fire, 0, 0);
        }

        // Тает лёд
        if (y + 1 < height && cells[y + 1, x].material == Material.Ice)
        {
            updates[(x, y + 1)] = new Cell(Material.Water, 0, 0);
        }
        if (y - 1 >= 0 && cells[y - 1, x].material == Material.Ice)
        {
            updates[(x, y - 1)] = new Cell(Material.Water, 0, 0);
        }
        if (x + 1 < width && cells[y, x + 1].material == Material.Ice)
        {
            updates[(x + 1, y)] = new Cell(Material.Water, 0, 0);
        }
        if (x - 1 >= 0 && cells[y, x - 1].material == Material.Ice)
        {
            updates[(x - 1, y)] = new Cell(Material.Water, 0, 0);
        }

        // Падает вниз медленнее песка
        if (rng.Next(2) == 0) // Медленнее песка
        {
            if (y + 1 < height && cells[y + 1, x].material == Material.Empty)
            {
                updates[(x, y)] = new Cell();
                updates[(x, y + 1)] = new Cell(Material.Lava, 0, 0);
                processed.Add((x, y));
                processed.Add((x, y + 1));
                return;
            }

            // Проверяем диагонали
            bool leftDown = y + 1 < height && x - 1 >= 0 && cells[y + 1, x - 1].material == Material.Empty;
            bool rightDown = y + 1 < height && x + 1 < width && cells[y + 1, x + 1].material == Material.Empty;

            if (leftDown && rightDown)
            {
                if (rng.Next(2) == 0)
                {
                    updates[(x, y)] = new Cell();
                    updates[(x - 1, y + 1)] = new Cell(Material.Lava, 0, 0);
                    processed.Add((x, y));
                    processed.Add((x - 1, y + 1));
                }
                else
                {
                    updates[(x, y)] = new Cell();
                    updates[(x + 1, y + 1)] = new Cell(Material.Lava, 0, 0);
                    processed.Add((x, y));
                    processed.Add((x + 1, y + 1));
                }
            }
            else if (leftDown)
            {
                updates[(x, y)] = new Cell();
                updates[(x - 1, y + 1)] = new Cell(Material.Lava, 0, 0);
                processed.Add((x, y));
                processed.Add((x - 1, y + 1));
            }
            else if (rightDown)
            {
                updates[(x, y)] = new Cell();
                updates[(x + 1, y + 1)] = new Cell(Material.Lava, 0, 0);
                processed.Add((x, y));
                processed.Add((x + 1, y + 1));
            }
        }
    }

    private void UpdateSteam(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        if (processed.Contains((x, y))) return;

        var cell = cells[y, x];
        var newCell = new Cell(Material.Steam, cell.temperature, cell.age + 1);

        // Если возраст >= 90, конденсируется в воду
        if (newCell.age >= 90)
        {
            updates[(x, y)] = new Cell(Material.Water, 0, 0);
            processed.Add((x, y));
            return;
        }

        // Поднимается вверх быстрее дыма
        if (y - 1 >= 0 && cells[y - 1, x].material == Material.Empty)
        {
            updates[(x, y)] = new Cell();
            updates[(x, y - 1)] = newCell;
            processed.Add((x, y));
            processed.Add((x, y - 1));
        }
        else
        {
            updates[(x, y)] = newCell;
            processed.Add((x, y));
        }
    }

    private void UpdateIce(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        if (processed.Contains((x, y))) return;

        // Тает от огня или лавы
        bool shouldMelt = false;
        if (y + 1 < height && (cells[y + 1, x].material == Material.Fire || cells[y + 1, x].material == Material.Lava))
            shouldMelt = true;
        if (y - 1 >= 0 && (cells[y - 1, x].material == Material.Fire || cells[y - 1, x].material == Material.Lava))
            shouldMelt = true;
        if (x + 1 < width && (cells[y, x + 1].material == Material.Fire || cells[y, x + 1].material == Material.Lava))
            shouldMelt = true;
        if (x - 1 >= 0 && (cells[y, x - 1].material == Material.Fire || cells[y, x - 1].material == Material.Lava))
            shouldMelt = true;

        if (shouldMelt)
        {
            updates[(x, y)] = new Cell(Material.Water, 0, 0);
            processed.Add((x, y));
        }
    }

    private void UpdateWood(int x, int y, Dictionary<(int x, int y), Cell> updates, HashSet<(int x, int y)> processed)
    {
        if (processed.Contains((x, y))) return;

        // Горит от огня или лавы
        bool shouldBurn = false;
        if (y + 1 < height && (cells[y + 1, x].material == Material.Fire || cells[y + 1, x].material == Material.Lava))
            shouldBurn = true;
        if (y - 1 >= 0 && (cells[y - 1, x].material == Material.Fire || cells[y - 1, x].material == Material.Lava))
            shouldBurn = true;
        if (x + 1 < width && (cells[y, x + 1].material == Material.Fire || cells[y, x + 1].material == Material.Lava))
            shouldBurn = true;
        if (x - 1 >= 0 && (cells[y, x - 1].material == Material.Fire || cells[y, x - 1].material == Material.Lava))
            shouldBurn = true;

        if (shouldBurn)
        {
            updates[(x, y)] = new Cell(Material.Fire, 0, 0);
            processed.Add((x, y));
        }
    }

    public void PlaceMaterial(int x, int y, Material material)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            cells[y, x] = new Cell(material, 0, 0);
        }
    }

    public Material GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return cells[y, x].material;
        }
        return Material.Stone; // Границы считаются твёрдыми
    }
}
