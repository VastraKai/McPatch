namespace McPatch.ConsoleMenu;

/// <summary>
/// Performs an action when selected.
/// </summary>
public class ActionMenuItem : MenuItem
{
    public Action ExecutableAction { get; set; }
    
    public override void Action()
    {
        ExecutableAction();
    }
    public ActionMenuItem(string name, string friendlyName, Action action) : base(name, friendlyName)
    {
        ExecutableAction = action;
    }
}