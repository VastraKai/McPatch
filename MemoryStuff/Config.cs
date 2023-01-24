using Newtonsoft.Json;
namespace McPatch;
public static class Config
{
    public class Configuration
    {
        public float GuiScale;
        public bool AutoSprint;
        public bool FastSwing;
        public bool ShowNametag;
        public bool ShowMobTag;
        public bool ForceShowCoordinates;
        public bool McMultiInstance;
    }

    private static readonly string configPath = Environment.ExpandEnvironmentVariables(
                        "%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\roamingstate\\KaiMod_config.txt");
    // Current Config
    private static Configuration? currentConfig = new();
    // just so i can have 0 messages lol
    public static Configuration? CurrentConfig { get => currentConfig; set => currentConfig = value; }
    

    // Save Config
    public static void SaveConfig(bool hideSaveMsg = false, string location = "default")
    {
        try
        {
            if (location == "default") location = configPath;
            string json = JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented);
            File.WriteAllText(location, json);
            if (!hideSaveMsg) Console.WriteLine($"{Console.Prefix("Config")}{Console.GreenTextColor} Config saved successfully.{Console.R}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{Console.ErrorPrefix("Config")}{Console.WarningTextColor} Failed to save config: {ex.Message}{Console.R}");
        }
    }
    // Load Config
    public static bool LoadConfig(string location = "default")
    {
        try
        {
            if (location == "default") location = configPath;
            if (File.Exists(location))
            {
                string json = File.ReadAllText(location);
                CurrentConfig = JsonConvert.DeserializeObject<Configuration>(json);
                if (CurrentConfig.ForceShowCoordinates && !CurrentConfig.ForceShowCoordinates)
                {
                    CurrentConfig.ForceShowCoordinates = false;
                    SaveConfig(true);
                }
                Console.WriteLine($"{Console.Prefix("Config")}{Console.GreenTextColor} Config loaded successfully.{Console.R}");
                
            }
            else
            {
                Console.WriteLine($"{Console.WarningPrefix("Config")}{Console.WarningTextColor} Config file not found, creating a new one.{Console.R}");
                CurrentConfig = new()
                {
                    GuiScale = 3,
                    AutoSprint = true,
                    FastSwing = false,
                    ShowNametag = true,
                    ShowMobTag = false,
                    ForceShowCoordinates = true,
                    McMultiInstance = false
                };
                SaveConfig(true);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{Console.ErrorPrefix("Config")}{Console.WarningTextColor} Failed to load config: {ex.Message}{Console.R}");
        }
        return false;
    }
    // Method for resetting config
    public static void ResetConfig(string location = "default")
    {
        if (location == "default") location = configPath;
        if (System.IO.File.Exists(location))
            File.Delete(location);
        Console.WriteLine($"{Console.Prefix("Config")}{Console.GreenTextColor} Config reset successfully.{Console.R}");
    }
}
