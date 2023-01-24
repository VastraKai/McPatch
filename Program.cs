using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Windows.AI.MachineLearning;

namespace McPatch;
public static class Program
{
    static void Main(string[] args)
    {
        Console.SetupConsole();
        if (args.Length > 0 && args[0].ToLower() == "--reset-config") Config.ResetConfig();
        M.Setup(false);
        bool newConfig = Config.LoadConfig();
        if (!Util.IsDeveloperModeEnabled())
            ConfigPrompt();
        else if (newConfig)
            ConfigPrompt();
        else
        {
            Console.Write($"{Console.Prefix("Patcher")} Edit config? ");
            ConsoleKey key = ConsoleKey.NoName;
            while (key != ConsoleKey.Y && key != ConsoleKey.N)
            {
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                {
                    Console.Write("\r                           \r");
                    ConfigPrompt();
                }
                else if (key == ConsoleKey.N)
                {
                    Console.Write("\r                           \r");
                }
            }
        }
        Thread patchThread = new Thread(() =>
        {
            try
            {
                bool success = Patcher.Patch();
                // bruh i get up to 4 gigabytes of memory usage wtf
                // the mc executable is like 100 megabytes it should not be that big
                if (success) Console.WriteLine($"{Console.Prefix("Patcher")} {Console.GreenTextColor}Settings applied successfully!{Console.R}");
                else Console.WriteLine($"{Console.ErrorPrefix("Patcher")} {Console.ErrorTextColor}Failed to apply settings...{Console.R}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Console.ErrorPrefix("Patcher")} An exception was thrown (please report this): {ex}");
                if (ex.GetType().ToString() == "System.ArgumentNullException") Console.WriteLine($"{Console.Prefix("Patcher")} Looks like an \"invalid sig\" error. Make sure your game isn't minimized then try again.");
            }
        });
        patchThread.IsBackground = true; 
        patchThread.Start();
        patchThread.Join();

        GC.WaitForPendingFinalizers(); 
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        
        
        Util.OpenMc();
        Console.WriteLine($"{Console.Prefix("Patcher")} Press enter to exit...");
        Console.WaitForEnter();
    }
    public static void ConfigPrompt()
    {
        bool DevModeEnabled = Util.IsDeveloperModeEnabled();
        while (true)
        {
            Console.SwitchToAlternativeBuffer();
            Console.WriteLine($"{Console.Prefix("Config Menu")}");
            string EnabledText = $"{Console.GreenTextColor}Enabled{Console.R}";
            string DisabledText = $"{Console.ErrorTextColor}Disabled{Console.R}";

            Console.WriteLine($"1) GuiScale: {Console.Value((Config.CurrentConfig.GuiScale.ToString()))}");
            Console.WriteLine($"2) Always Sprint: {(Config.CurrentConfig.AutoSprint ? EnabledText : DisabledText)}");
            Console.WriteLine($"3) Fast Swing: {(Config.CurrentConfig.FastSwing ? EnabledText : DisabledText)}");
            Console.WriteLine($"4) Show Player Nametag: {(Config.CurrentConfig.ShowNametag ? EnabledText : DisabledText)}");
            Console.WriteLine($"5) Show Mob Nametag: {(Config.CurrentConfig.ShowMobTag ? EnabledText : DisabledText)}");
            Console.WriteLine($"6) Force Show Coordinates: {(Config.CurrentConfig.ForceShowCoordinates ? EnabledText : DisabledText)}");
            Console.Write($"7) Minecraft Multi-Instance: {(Config.CurrentConfig.McMultiInstance ? EnabledText : DisabledText)}");
            if (!DevModeEnabled) Console.Write($" {Console.ErrorTextColor}(You must enable developer mode!){Console.R}");
            Console.WriteLine();
            Console.WriteLine($"8) Save Config");
            Console.WriteLine($"9) Exit and patch");
            Console.Write($"Select an option: {Console.ValueColor}");
        ret:
            string selection = Console.ReadKey().KeyChar.ToString().ToLower();
            Console.WriteLine(Console.R);
            switch (selection)
            {
                case "1":
                    (int left, int top) value = Console.GetCursorPosition();
                    Console.SetCursorPosition(13, 1);
                    Console.Write("            ");
                    Console.SetCursorPosition(13, 1);
                    string? input = Console.ReadLine();
                    bool validFloat = float.TryParse(input, out float scale);
                    if (validFloat)
                    {
                        Config.CurrentConfig.GuiScale = scale;
                    }
                    break;
                case "2":
                    Config.CurrentConfig.AutoSprint = !Config.CurrentConfig.AutoSprint;
                    break;
                case "3":
                    Config.CurrentConfig.FastSwing = !Config.CurrentConfig.FastSwing;
                    break;
                case "4":
                    Config.CurrentConfig.ShowNametag = !Config.CurrentConfig.ShowNametag;
                    break;
                case "5":
                    Config.CurrentConfig.ShowMobTag = !Config.CurrentConfig.ShowMobTag;
                    break;
                case "6":
                    Config.CurrentConfig.ForceShowCoordinates = !Config.CurrentConfig.ForceShowCoordinates;
                    break;
                case "7":
                    Config.CurrentConfig.McMultiInstance = !Config.CurrentConfig.McMultiInstance;
                    DevModeEnabled = Util.IsDeveloperModeEnabled();
                    if (!DevModeEnabled)
                    {
                        Console.WriteLine($"{Console.WarningPrefix("Config Menu")} You must enable developer mode to use multi-instance. Please enable it before you continue.");
                        Console.WriteLine($"{Console.Prefix("Config Menu")} Press enter to continue...");
                        Console.WaitForEnter();
                    }
                    break;
                case "8":
                    Config.SaveConfig();
                    Thread.Sleep(1000);
                    break;
                case "9":
                    Console.SwitchToMainBuffer();
                    Config.SaveConfig();
                    return;
                default:
                    if (Console.CursorLeft == 0) Console.CursorTop -= 1;
                    Console.CursorLeft = 18;
                    Console.Write($"{Console.ErrorTextColor}{selection}{Console.R}");
                    Console.CursorLeft = 18;
                    goto ret;
            }
        }
    }
}