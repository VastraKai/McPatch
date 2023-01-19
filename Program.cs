using System.Reflection;

namespace McPatch;
public static class Program
{
    static void Main(string[] args)
    {
        Console.SetupConsole(); // lol oops
        try
        {
            if (args.Length > 0 && args[0].ToLower() == "--reset-config") Config.ResetConfig();

            Patcher();
            // bruh i get up to 4 gigabytes of memory usage wtf
            // the mc executable is like 100 megabytes it should not be that big
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine($"{Console.Prefix("Patcher")} {Console.GreenTextColor}Settings applied successfully!{Console.R}");
            Util.OpenMc();
            Console.WriteLine($"{Console.Prefix("Patcher")} Press any key to exit...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} An exception was thrown (please report this): {ex}");
            if(ex.GetType().ToString() == "System.ArgumentNullException") Console.WriteLine($"{Console.Prefix("Patcher")} Looks like an \"invalid sig\" error. Make sure your game isn't minimized then try again.");
            Console.WriteLine($"{Console.Prefix("Patcher")} Press enter to exit...");
            Console.WaitForEnter();
        }
    }
    private static void Patcher()
    {
        M.Setup(false);

        bool newConfig = Config.LoadConfig();
        if (!Util.IsDeveloperModeEnabled())
            ConfigPrompt();
        else if (newConfig)
            ConfigPrompt();
        else
        {
            Console.Write($"{Console.Prefix("Patcher")} Edit config? ");
            string key = Console.ReadKey(true).KeyChar.ToString().ToLower();
            Console.Write("\r                           \r");
            if (key == "y")
                ConfigPrompt();
        }
        Console.WriteLine($"{Console.Prefix("Patcher")} Preparing...");
        string mcPath = Path.GetFullPath(Util.McProcess.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcPath: {mcPath}");
        string mcExe = mcPath + "Minecraft.Windows.exe";
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExe: {mcExe}");
        string mcExeBak = mcExe + ".bak";
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExeBak: {mcExeBak}");

        #region backup stuff
        string LastMcVersionPath = Environment.ExpandEnvironmentVariables("%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\roamingstate\\lastMcVersion.txt");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} LastMcVersionPath: {LastMcVersionPath}");
        string CurrentMcVersion = Util.McGetVersion();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} CurrentMcVersion: {CurrentMcVersion}");

        string LastMcVersion;
        if (!File.Exists(LastMcVersionPath))
            LastMcVersion = CurrentMcVersion;
        else
            LastMcVersion = File.ReadAllText(LastMcVersionPath).Replace("\n", "");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} LastMcVersion: {LastMcVersion}");

        if (LastMcVersion != CurrentMcVersion && File.Exists(mcExeBak))
        {
            Console.WriteLine($"{Console.WarningPrefix("Patcher")} Minecraft version changed from {LastMcVersion} to {CurrentMcVersion}, recreating backup file.{Console.R}");
            File.Delete(mcExeBak);
            File.WriteAllText(LastMcVersionPath, CurrentMcVersion);
        }
        if (!File.Exists(mcExeBak)) File.Copy(mcExe, mcExeBak, true);
        #endregion

        string mcHex = File.ReadAllBytes(mcExe).ToHexString().Replace(" ", "");
        Console.WriteLine($"{Console.Prefix("Patcher")} Applying settings...");

        foreach (PropertyInfo property in typeof(Fields.Address).GetProperties())
        {
            string? PropertyName = property.Name;
            string? Address = property.GetValue(null).ToString();
            if (Address == "0")
            {
                Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Unable to find address for {PropertyName}, skipping... (dead sig?)");
            }
            else
            {
                string? bytes = M.mem.ReadBytes(Address, 11).ToHexString().Replace(" ", "");
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Field Info for {PropertyName}: {Address}:{M.mem.ReadBytes(Address, 11).ToHexString()}");
                switch (PropertyName)
                {
                    case "GuiScaleAddr":
                        Fields.GuiScale = Config.CurrentConfig.GuiScale;
                        Console.WriteLine($"{Console.Prefix("Patcher")} GuiScale has been set to {Config.CurrentConfig.GuiScale}");
                        break;
                    case "SprintAddr":
                        if (!Config.CurrentConfig.AutoSprint) break;
                        M.mem.WriteMemory(Address, "bytes", "49 8B C5 90");
                        Console.WriteLine($"{Console.Prefix("Patcher")} SprintAddr has been enabled");
                        break;
                    case "FastSwingAddr":
                        if (!Config.CurrentConfig.FastSwing) break;
                        M.mem.WriteMemory(Address, "byte", "EB");
                        Console.WriteLine($"{Console.Prefix("Patcher")} FastSwing has been enabled");
                        break;
                    case "ShowNameAddr":
                        if (!Config.CurrentConfig.ShowNametag) break;
                        M.mem.WriteMemory(Address, "bytes", "90 90 90 90 90 90");
                        Console.WriteLine($"{Console.Prefix("Patcher")} ShowName has been enabled");
                        break;
                    case "ShowMobtagAddr":
                        if (!Config.CurrentConfig.ShowMobTag) break;
                        M.mem.WriteMemory(Address, "bytes", "90 90 90 90 90 90");
                        Console.WriteLine($"{Console.Prefix("Patcher")} ShowMobtag has been enabled");
                        break;
                    default:
                        Console.WriteLine($"{Console.WarningPrefix("Patcher")} Case for property " + PropertyName + " not found!");
                        break;
                }
                string? newBytes = M.mem.ReadBytes(Address, 11).ToHexString().Replace(" ", "");
                mcHex = mcHex.Replace(bytes, newBytes);
            }
        }

        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Killing Minecraft");
        Util.McProcess.Kill();
        M.mem.CloseProcess();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {mcPath}");
        Util.GrantAccess(mcPath); // Make the original file writable
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new bytes to mcExe");
        File.WriteAllBytesAsync(mcExe, mcHex.FromHexString()).Wait(); // Write the new contents to the temp file
        if (Util.IsDeveloperModeEnabled())
        {
            if (Config.CurrentConfig.McMultiInstance)
            {
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Enabling multi instance, patching AppxManifest file");
                ClientUtils.EnableMultiInstance(mcPath);
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Re-registering appx");
                Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", mcPath).Wait();
            }
            else
            {
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Disabling multi instance, patching AppxManifest file");
                ClientUtils.DisableMultiInstance(mcPath);
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Re-registering appx");
                Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", mcPath).Wait();
            }
        }
        else
        {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")}{Console.ErrorTextColor} Unable to use multi-instance: Developer mode is not enabled.{Console.R}");
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")}{Console.ErrorTextColor} Please enable developer mode in ms-settings:developers.{Console.R}");
        }
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new version file");
        File.WriteAllText(LastMcVersionPath, CurrentMcVersion);
    }
    public static void ConfigPrompt()
    {
        bool DevModeEnabled = Util.IsDeveloperModeEnabled();
        while (true)
        {
            Console.SwitchToAlternativeBuffer();
            Console.WriteLine($"{Console.Prefix("Config Menu")}");
            Console.WriteLine($"1) GuiScale: {Config.CurrentConfig.GuiScale}");
            Console.WriteLine($"2) Always Sprint: {(Config.CurrentConfig.AutoSprint ? "Enabled" : "Disabled")}");
            Console.WriteLine($"3) Fast Swing: {(Config.CurrentConfig.FastSwing ? "Enabled" : "Disabled")}");
            Console.WriteLine($"4) Show Player Nametag: " + (Config.CurrentConfig.ShowNametag ? "Enabled" : "Disabled"));
            Console.WriteLine($"5) Show Mob Nametag: " + (Config.CurrentConfig.ShowMobTag ? "Enabled" : "Disabled"));
            if (DevModeEnabled) Console.WriteLine($"6) Minecraft Multi-Instance: " + (Config.CurrentConfig.McMultiInstance ? "Enabled" : "Disabled"));
            else Console.WriteLine($"6) Minecraft Multi-Instance: " + (Config.CurrentConfig.McMultiInstance ? "Enabled" : "Disabled") + $" {Console.ErrorTextColor}(You must enable developer mode!){Console.R}");
            Console.WriteLine($"7) Save Config");
            Console.WriteLine($"8) Exit Menu");
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
                    Config.CurrentConfig.McMultiInstance = !Config.CurrentConfig.McMultiInstance;
                    DevModeEnabled = Util.IsDeveloperModeEnabled();
                    if (!DevModeEnabled)
                    {
                        Console.WriteLine($"{Console.WarningPrefix("Config Menu")} You must enable developer mode to use multi-instance. Please enable it before you continue.");
                        Console.WriteLine($"{Console.Prefix("Config Menu")} Press enter to continue...");
                        Console.WaitForEnter();
                    }
                    break;
                case "7":
                    Config.SaveConfig();
                    Thread.Sleep(1000);
                    break;
                case "8":
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