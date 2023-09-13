namespace McPatch.ConsoleMenu;

/// <summary>
/// A menu item that toggles between true and false.
/// </summary>
public class BoolMenuItem : MenuItem
{
    public bool Value { get; set; }
    
    public override void Action()
    {
        Value = !Value;
    }

    public override string ToString()
    {
        return Value ? "Enabled" : "Disabled";
    }

    public BoolMenuItem(string name, string friendlyName, bool value) : base(name, friendlyName)
    {
        Value = value;
    }
}