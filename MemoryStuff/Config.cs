using Newtonsoft.Json;
namespace McPatch;
public static class Config
{

    public static readonly string ConfigPath = Environment.ExpandEnvironmentVariables(
                        "%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\roamingstate\\KaiMod_config.txt");

    // Save Config
    public static void SaveConfig(bool hideSaveMsg = false, string location = "default")
    {
        try
        {
            if (location == "default") location = ConfigPath;
            Program.ConfigMenu.SaveJson(location);
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
            if (location == "default") location = ConfigPath;
            bool result = Program.ConfigMenu.LoadJson(location);
            string createdOrLoaded = result ? "created" : "loaded";
            Console.Log.WriteLine("Config", $"&aConfig {createdOrLoaded} successfully.&r", LogLevel.Success);
            return result;
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
        if (location == "default") location = ConfigPath;
        if (System.IO.File.Exists(location))
            File.Delete(location);
        Console.Log.WriteLine("Config", "&aConfig reset successfully.&r", LogLevel.Success);
        
    }
}
