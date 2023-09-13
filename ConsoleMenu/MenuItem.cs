using System.Text.Json.Serialization;

namespace McPatch.ConsoleMenu;

public class MenuItem
{
    [JsonInclude]
    public string Name { get; set; }
    
    [JsonIgnore]
    public string FriendlyName { get; set; }

    public virtual void Action()
    {
        throw new NotImplementedException("Action is not implemented!");
    }
    
    public MenuItem(string name, string friendlyName)
    {
        Name = name;
        FriendlyName = friendlyName;
    }
}