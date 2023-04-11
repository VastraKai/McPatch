using Newtonsoft.Json;
namespace McPatch;
public static class Config
{
    public class Configuration
    {
        public bool CancelSwing;
        public float GuiScale;
        public bool AlwaysSprint;
        public bool ShowPlayerNametag;
        public bool ForceShowNametags;
        public bool ForceShowCoordinates;
        public bool McMultiInstance;
    }

    private static readonly string configPath = Environment.ExpandEnvironmentVariables(
                        "%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\roamingstate\\KaiMod_config.txt");
    // Current Config
    private static Configuration currentConfig = new();
    // just so i can have 0 messages lol
    public static Configuration CurrentConfig { get => currentConfig; set => currentConfig = value; }


    // Save Config
    public static void SaveConfig(bool hideSaveMsg = false, string location = "default")
    {
        try
        {
            if (location == "default") location = configPath;
            string json = JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented);
            File.WriteAllText(location, json);
            if (!hideSaveMsg) Console.Log.WriteLine("Config", "Config saved successfully.", LogLevel.Success);
            
        }
        catch (Exception ex)
        {
            Console.Log.WriteLine("Config", $"&cFailed to save config: {ex.Message}", LogLevel.Error);
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
                Configuration? config = JsonConvert.DeserializeObject<Configuration>(json);
                if (config == null) throw new NullReferenceException("Config was null");
                CurrentConfig = config;
                if (CurrentConfig == null) throw new Exception("Config is null");
                if (CurrentConfig.ForceShowCoordinates && !CurrentConfig.ForceShowCoordinates)
                {
                    CurrentConfig.ForceShowCoordinates = false;
                    SaveConfig(true);
                }
                Console.Log.WriteLine("Config", "&aConfig loaded successfully.&r", LogLevel.Success);

            }
            else
            {
                Console.Log.WriteLine("Config", "&eConfig file not found, creating a new one.&r", LogLevel.Warning);
                CurrentConfig = new()
                {
                    GuiScale = 3,
                    AlwaysSprint = true,
                    CancelSwing = false,
                    ShowPlayerNametag = true,
                    ForceShowNametags = false,
                    ForceShowCoordinates = true,
                    McMultiInstance = false
                };
                SaveConfig(true);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.Log.WriteLine("Config", $"&cFailed to load config: {ex.Message}", LogLevel.Error);
        }
        return false;
    }
    // Method for resetting config
    public static void ResetConfig(string location = "default")
    {
        if (location == "default") location = configPath;
        if (System.IO.File.Exists(location))
            File.Delete(location);
        Console.Log.WriteLine("Config", "&aConfig reset successfully.&r", LogLevel.Success);
        
    }
}
