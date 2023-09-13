namespace McPatch.ConsoleMenu;

/// <summary>
/// A menu item that stores a float.
/// </summary>
public class FloatMenuItem : MenuItem
{
    public float Value { get; set; }
    
    public override void Action()
    {
        Console.Write($"Enter a new value for {Name}: ");
        string? input = Console.ReadLine();
        if (float.TryParse(input, out float result))
        {
            Value = result;
        }
    }
    
    public FloatMenuItem(string name, string friendlyName, float value) : base(name, friendlyName)
    {
        Value = value;
    }
}