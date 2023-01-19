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
    }
    private static string configPath = Environment.ExpandEnvironmentVariables(
                        "%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\roamingstate\\KaiMod_config.txt");
    // Current Config
    public static Configuration CurrentConfig = new();
    // Save Config
    public static void SaveConfig(string location = "default")
    {
        try
        {
            if (location == "default") location = configPath;
            string json = JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented);
            System.IO.File.WriteAllText(location, json);
            Console.WriteLine($"{Console.PrefixColor}[Config]{Console.GreenTextColor} Config saved successfully.{Console.R}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{Console.ErrorTextColor}[Config Error]{Console.WarningTextColor} Failed to save config: {ex.Message}{Console.R}");
        }
    }
    // Load Config
    public static bool LoadConfig(string location = "default")
    {
        try
        {
            if (location == "default") location = configPath;
            if (System.IO.File.Exists(location))
            {
                string json = System.IO.File.ReadAllText(location);
                CurrentConfig = JsonConvert.DeserializeObject<Configuration>(json);
                Console.WriteLine($"{Console.PrefixColor}[Config]{Console.GreenTextColor} Config loaded successfully.{Console.R}");
            }
            else
            {
                Console.WriteLine($"{Console.PrefixColor}[Config]{Console.WarningTextColor} Config file not found, creating a new one.{Console.R}");
                CurrentConfig = new();
                CurrentConfig.GuiScale = 3;
                CurrentConfig.AutoSprint = true;
                CurrentConfig.FastSwing = false;
                CurrentConfig.ShowNametag = true;
                CurrentConfig.ShowMobTag = false;
                SaveConfig();
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{Console.ErrorTextColor}[Config Error]{Console.WarningTextColor} Failed to load config: {ex.Message}{Console.R}");
        }
        return false;
    }
    // Method for resetting config
    public static void ResetConfig(string location = "default")
    {
        if (location == "default") location = configPath;
        if (System.IO.File.Exists(location))
            File.Delete(location);
        Console.WriteLine($"{Console.PrefixColor}[Config]{Console.GreenTextColor} Config reset successfully.{Console.R}");
    }
}
