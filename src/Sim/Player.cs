namespace Sim;

public class Player
{
    public float x;
    public float y;
    public float velocityX;
    public float velocityY;
    public const float Width = 8.0f;
    public const float Height = 12.0f;
    public const float Speed = 100.0f;
    public const float JumpSpeed = -200.0f;
    public const float Gravity = 400.0f;
    public bool isOnGround;

    public Player(float startX, float startY)
    {
        x = startX;
        y = startY;
        velocityX = 0;
        velocityY = 0;
        isOnGround = false;
    }

    public void Update(float dt, World world)
    {
        // Применяем гравитацию
        if (!isOnGround)
        {
            velocityY += Gravity * dt;
        }

        // Ограничиваем максимальную скорость падения
        if (velocityY > 300.0f)
        {
            velocityY = 300.0f;
        }

        // Вычисляем новую позицию
        float newX = x + velocityX * dt;
        float newY = y + velocityY * dt;

        // Проверяем коллизии по X
        if (CheckCollisionX(newX, y, world))
        {
            velocityX = 0;
        }
        else
        {
            x = newX;
        }

        // Проверяем коллизии по Y
        if (CheckCollisionY(x, newY, world))
        {
            if (velocityY > 0) // Падение
            {
                isOnGround = true;
                velocityY = 0;
            }
            else // Прыжок вверх
            {
                velocityY = 0;
            }
        }
        else
        {
            y = newY;
            isOnGround = false;
        }

        // Применяем трение к горизонтальной скорости
        velocityX *= 0.85f;
    }

    private bool CheckCollisionX(float checkX, float checkY, World world)
    {
        int left = (int)checkX;
        int right = (int)(checkX + Width);
        int top = (int)checkY;
        int bottom = (int)(checkY + Height);

        for (int y = top; y <= bottom; y++)
        {
            if (IsSolid(world.GetCell(left, y)))
                return true;
            if (IsSolid(world.GetCell(right, y)))
                return true;
        }
        return false;
    }

    private bool CheckCollisionY(float checkX, float checkY, World world)
    {
        int left = (int)checkX;
        int right = (int)(checkX + Width);
        int top = (int)checkY;
        int bottom = (int)(checkY + Height);

        for (int x = left; x <= right; x++)
        {
            if (IsSolid(world.GetCell(x, top)))
                return true;
            if (IsSolid(world.GetCell(x, bottom)))
                return true;
        }
        return false;
    }

    private bool IsSolid(Material material)
    {
        return material == Material.Stone || material == Material.Wood || material == Material.Ice;
    }

    public void MoveLeft()
    {
        velocityX = -Speed;
    }

    public void MoveRight()
    {
        velocityX = Speed;
    }

    public void Jump()
    {
        if (isOnGround)
        {
            velocityY = JumpSpeed;
            isOnGround = false;
        }
    }
}
