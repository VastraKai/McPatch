namespace McPatch;
public static class Program
{
    static void Main(string[] args)
    {
        Console.SetupConsole();
        if (args.Length > 0 && args[0].ToLower() == "--reset-config") Config.ResetConfig();

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
        bool success = false;
        Thread patchThread = new(() =>
        {
            try
            {
                success = Patcher.Patch();
                // memory usage is only up to like 500 now :D
                if (success) Console.WriteLine($"{Console.Prefix("Patcher")} {Console.GreenTextColor}Settings applied successfully!{Console.R}");
                else Console.WriteLine($"{Console.ErrorPrefix("Patcher")} {Console.ErrorTextColor}Failed to apply settings...{Console.R}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Console.ErrorPrefix("Patcher")} An exception was thrown (please report this): {ex}");
                if (ex.GetType().ToString() == "System.ArgumentNullException") Console.WriteLine($"{Console.Prefix("Patcher")} Looks like an \"invalid sig\" error. Make sure your game isn't minimized then try again.");
            }
        })
        {
            IsBackground = true
        };
        patchThread.Start();
        patchThread.Join();

        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

        if(success) Util.OpenMc();
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
            Console.WriteLine($"1) Gui Scale: {Console.Value((Config.CurrentConfig.GuiScale.ToString()))}");
            Console.WriteLine($"2) Always Sprint: {(Config.CurrentConfig.AlwaysSprint ? EnabledText : DisabledText)}");
            Console.WriteLine($"3) Cancel Swing: {(Config.CurrentConfig.CancelSwing ? EnabledText : DisabledText)}");
            Console.WriteLine($"4) Show Player Nametag: {(Config.CurrentConfig.ShowPlayerNametag ? EnabledText : DisabledText)}");
            Console.WriteLine($"5) Force Show Nametags: {(Config.CurrentConfig.ForceShowNametags ? EnabledText : DisabledText)}");
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
                    Console.SetCursorPosition(14, 1);
                    Console.Write($"{Console.ValueColor}            ");
                    Console.SetCursorPosition(14, 1);
                    string? input = Console.ReadLine();
                    Console.Write(Console.R);
                    bool validFloat = float.TryParse(input, out float scale);
                    if (validFloat)
                    {
                        Config.CurrentConfig.GuiScale = scale;
                    }
                    break;
                case "2":
                    Config.CurrentConfig.AlwaysSprint = !Config.CurrentConfig.AlwaysSprint;
                    break;
                case "3":
                    Config.CurrentConfig.CancelSwing = !Config.CurrentConfig.CancelSwing;
                    break;
                case "4":
                    Config.CurrentConfig.ShowPlayerNametag = !Config.CurrentConfig.ShowPlayerNametag;
                    break;
                case "5":
                    Config.CurrentConfig.ForceShowNametags = !Config.CurrentConfig.ForceShowNametags;
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