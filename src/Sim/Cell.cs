namespace Sim;

public class Cell
{
    public Material material;
    public int temperature;
    public int age;

    public Cell()
    {
        material = Material.Empty;
        temperature = 0;
        age = 0;
    }

    public Cell(Material material, int temperature, int age)
    {
        this.material = material;
        this.temperature = temperature;
        this.age = age;
    }
}
