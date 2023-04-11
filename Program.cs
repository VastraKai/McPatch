global using Console = ExtendedConsole.Console;
global using ExtendedConsole;
using McPatch.Utils;

namespace McPatch;

public static class Program
{
    public static MessageConfig NoPrefixMsg = new MessageConfig(true, true, true, false, false, false);
    static void Main(string[] args)
    {
        Console.Config.SetupConsole();
        Console.Log.RegisterColorShortcut("&p", 0.0f, 1.0f, 1.0f); // Prefix
        if (args.Length > 0 && args[0].ToLower() == "--reset-config") Config.ResetConfig();

        bool newConfig = Config.LoadConfig();
        if (!Util.IsDeveloperModeEnabled())
            ConfigPrompt();
        else if (newConfig)
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
        bool DevModeEnabled = Util.IsDeveloperModeEnabled();
        while (true)
        {
            Console.SwitchToAlternativeBuffer();
            
            Console.Log.WriteLine("", "&p[Config Menu]&r", LogLevel.Info, NoPrefixMsg);
            string EnabledText = $"{Console.Log.ColorA}Enabled{Console.Log.R}";
            string DisabledText = $"{Console.Log.ColorC}Disabled{Console.Log.R}";
            Console.Log.WriteLine("", $"1) Gui Scale: &v{Config.CurrentConfig.GuiScale}&r", LogLevel.Info, NoPrefixMsg);
            Console.WriteLine($"2) Always Sprint: {(Config.CurrentConfig.AlwaysSprint ? EnabledText : DisabledText)}");
            Console.WriteLine($"3) Cancel Swing: {(Config.CurrentConfig.CancelSwing ? EnabledText : DisabledText)}");
            Console.WriteLine(
                $"4) Show Player Nametag: {(Config.CurrentConfig.ShowPlayerNametag ? EnabledText : DisabledText)}");
            Console.WriteLine(
                $"5) Force Show Nametags: {(Config.CurrentConfig.ForceShowNametags ? EnabledText : DisabledText)}");
            Console.WriteLine(
                $"6) Force Show Coordinates: {(Config.CurrentConfig.ForceShowCoordinates ? EnabledText : DisabledText)}");
            Console.Write(
                $"7) Minecraft Multi-Instance: {(Config.CurrentConfig.McMultiInstance ? EnabledText : DisabledText)}");
            if (!DevModeEnabled)
                Console.Write($" {Console.Log.ErrorColor}(You must enable developer mode!){Console.Log.R}");
            Console.WriteLine();
            Console.WriteLine($"8) Save Config");
            Console.WriteLine($"9) Exit and patch");
            Console.Write($"Select an option: {Console.Log.ValueColor}");
            ret:
            string selection = Console.ReadKey().KeyChar.ToString().ToLower();
            Console.WriteLine(Console.Log.R);
            switch (selection)
            {
                case "1":
                    Console.SetCursorPosition(14, 1);
                    Console.Write($"{Console.Log.ValueColor}            ");
                    Console.SetCursorPosition(14, 1);
                    string? input = Console.ReadLine();
                    Console.Write(Console.Log.R);
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
                    Console.Write($"{Console.Log.ErrorColor}{selection}{Console.Log.R}");
                    Console.CursorLeft = 18;
                    goto ret;
            }
        }
    }
}