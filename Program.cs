global using Console = ExtendedConsole.Console;
global using ExtendedConsole;
using McPatch.ConsoleMenu;
using McPatch.Utils;

namespace McPatch;

public static class Program
{
    public static MessageConfig NoPrefixMsg = new MessageConfig(true, true, true, false, false, false);
    public static Menu ConfigMenu = new();
    static void Main(string[] args)
    {
        Console.Config.SetupConsole();
        Console.Log.RegisterColorShortcut("&p", 0.0f, 1.0f, 1.0f); // Prefix
        if (args.Length > 0 && args[0].ToLower() == "--reset-config") Config.ResetConfig();

        // Initialize the config menu
        ConfigMenu = new Menu($"{Console.Log.ColorB}[Config Menu]{Console.Log.R}", Config.ConfigPath);
        ConfigMenu.AddMenuItem("GuiScale", "Gui Scale", 3.0f);
        ConfigMenu.AddMenuItem("AlwaysSprint", "Always Sprint", true);
        ConfigMenu.AddMenuItem("CancelSwing", "Cancel Swing", false);
        ConfigMenu.AddMenuItem("ShowPlayerNametag", "Show Player Nametag", true);
        ConfigMenu.AddMenuItem("ForceShowNametags", "Force Show Nametags", true);
        ConfigMenu.AddMenuItem("ForceShowCoordinates", "Force Show Coordinates", false);
        ConfigMenu.AddMenuItem("McMultiInstance", "Minecraft Multi-Instance", false);
        ConfigMenu.AddMenuItem("SaveConfig", "Save Config", new Action(() =>
        {
            ConfigMenu.SaveJson();
            Console.Log.WriteLine("Main", "Saved config.");
            Thread.Sleep(750);
        }));
        
        bool newConfig = Config.LoadConfig();
        if (!Util.IsDeveloperModeEnabled())
        {
            Console.Log.WriteLine("Patcher", "&cDeveloper mode is not enabled!&r", LogLevel.Warning);
        }
        if (newConfig)
            ConfigPrompt();
        else
        {
            ConsoleKey key = Console.ReadKey("Patcher", "Edit config? ", new Console.KeyOutput[]
            {
                new Console.KeyOutput(ConsoleKey.Y, "&vyes&r"),
                new Console.KeyOutput(ConsoleKey.N, "&vno&r"),
            });
            if (key == ConsoleKey.Y) ConfigPrompt();
        }


        bool success = false;
        Thread patchThread = new(() =>
        {
            try
            {
                success = Patcher.Patch();
                // memory usage is only up to like 500 now :D
                if (success)
                    Console.Log.WriteLine("Patcher", "&aSettings applied successfully!&r", LogLevel.Success);
                else
                    Console.Log.WriteLine("Patcher", "&cFailed to apply settings...&r", LogLevel.Fail);
            }
            catch (Exception ex)
            {
                Console.Log.WriteLine("Patcher", $"An exception was thrown (please report this): {ex}", LogLevel.Error);
            }
        })
        {
            IsBackground = true
        };
        patchThread.Start();
        patchThread.Join();

        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

        if (success) Util.OpenMc();
        Console.WaitForEnter("Press enter to exit...");
    }

    public static void ConfigPrompt()
    { 
        ConfigMenu.ShowMenu();
    }
}