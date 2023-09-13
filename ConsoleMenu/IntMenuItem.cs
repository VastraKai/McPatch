namespace McPatch.ConsoleMenu;

/// <summary>
/// A menu item that stores an int.
/// </summary>
public class IntMenuItem : MenuItem
{
    public int Value { get; set; }
    
    public override void Action()
    {
        Console.Write($"Enter a new value for {Name}: ");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int result))
        {
            Value = result;
        }
    }
    
    public IntMenuItem(string name, string friendlyName, int value) : base(name, friendlyName)
    {
        Value = value;
    }
}