using Sim;

namespace Game;

public class Camera
{
    public float x;
    public float y;
    private const float FollowSpeed = 0.15f; // Коэффициент плавности следования

    public Camera(float startX, float startY)
    {
        x = startX;
        y = startY;
    }

    public void Follow(Player player)
    {
        // Плавное следование за игроком
        float targetX = player.x - 160.0f; // Центрируем по X (половина ширины экрана)
        float targetY = player.y - 90.0f;  // Центрируем по Y (половина высоты экрана)

        x += (targetX - x) * FollowSpeed;
        y += (targetY - y) * FollowSpeed;
    }

    public Microsoft.Xna.Framework.Point WorldToScreen(float worldX, float worldY, int scale)
    {
        int screenX = (int)((worldX - x) * scale);
        int screenY = (int)((worldY - y) * scale);
        return new Microsoft.Xna.Framework.Point(screenX, screenY);
    }

    public Microsoft.Xna.Framework.Point? ScreenToWorld(int screenX, int screenY, int scale)
    {
        float worldX = screenX / (float)scale + x;
        float worldY = screenY / (float)scale + y;
        return new Microsoft.Xna.Framework.Point((int)worldX, (int)worldY);
    }
}
