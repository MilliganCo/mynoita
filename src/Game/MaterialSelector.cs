using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sim;

namespace Game;

public class MaterialSelector
{
    private readonly Material[] materials;
    private int selectedIndex;
    private readonly int screenWidth;
    private readonly int screenHeight;
    private const int PanelHeight = 50;
    private const int ItemSize = 40;
    private const int ItemSpacing = 5;

    public MaterialSelector(int screenWidth, int screenHeight)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        
        // Исключаем Empty из списка материалов для выбора
        materials = new Material[]
        {
            Material.Sand,
            Material.Water,
            Material.Stone,
            Material.Fire,
            Material.Smoke,
            Material.Oil,
            Material.Lava,
            Material.Steam,
            Material.Ice,
            Material.Wood
        };
        selectedIndex = 0;
    }

    public Material GetSelectedMaterial()
    {
        return materials[selectedIndex];
    }

    public void HandleInput(KeyboardState currentKeyboard, KeyboardState previousKeyboard, MouseState currentMouse, MouseState previousMouse)
    {
        // Переключение клавишами 1-9, 0
        for (int i = 0; i < 10; i++)
        {
            Keys key = i == 9 ? Keys.D0 : (Keys)((int)Keys.D1 + i);
            if (currentKeyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key))
            {
                if (i < materials.Length)
                {
                    selectedIndex = i;
                }
            }
        }

        // Переключение колёсиком мыши
        int scrollDelta = currentMouse.ScrollWheelValue - previousMouse.ScrollWheelValue;
        if (scrollDelta > 0)
        {
            selectedIndex = (selectedIndex + 1) % materials.Length;
        }
        else if (scrollDelta < 0)
        {
            selectedIndex = (selectedIndex - 1 + materials.Length) % materials.Length;
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D whiteTexture)
    {
        // Рисуем панель внизу экрана
        int panelY = screenHeight - PanelHeight;
        var panelRect = new Rectangle(0, panelY, screenWidth, PanelHeight);
        spriteBatch.Draw(whiteTexture, panelRect, new Color(30, 30, 30, 200)); // Темно-серая панель

        // Рисуем материалы
        int startX = (screenWidth - (materials.Length * (ItemSize + ItemSpacing))) / 2;
        for (int i = 0; i < materials.Length; i++)
        {
            int itemX = startX + i * (ItemSize + ItemSpacing);
            int itemY = panelY + (PanelHeight - ItemSize) / 2;
            var itemRect = new Rectangle(itemX, itemY, ItemSize, ItemSize);

            // Цвет материала
            Color materialColor = GetMaterialColor(materials[i]);
            
            // Выделение выбранного материала
            if (i == selectedIndex)
            {
                // Рамка вокруг выбранного
                var borderRect = new Rectangle(itemX - 2, itemY - 2, ItemSize + 4, ItemSize + 4);
                spriteBatch.Draw(whiteTexture, borderRect, Color.White);
            }

            spriteBatch.Draw(whiteTexture, itemRect, materialColor);
        }
    }

    private Color GetMaterialColor(Material material)
    {
        return material switch
        {
            Material.Sand => new Color(255, 200, 0),
            Material.Water => new Color(0, 100, 255),
            Material.Stone => new Color(128, 128, 128),
            Material.Fire => new Color(255, 50, 0),
            Material.Smoke => new Color(200, 200, 200),
            Material.Oil => new Color(50, 50, 50),
            Material.Lava => new Color(255, 100, 0),
            Material.Steam => new Color(240, 240, 255),
            Material.Ice => new Color(200, 240, 255),
            Material.Wood => new Color(139, 90, 43),
            _ => Color.Black
        };
    }
}
