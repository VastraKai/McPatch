using System.Reflection;

namespace McPatch;
public static class Program
{
    static void Main(string[] args)
    {
        try
        {
            if (args.Length > 0 && args[0].ToLower() == "--reset-config") Config.ResetConfig();
            Patcher();
            Console.WriteLine($"{Console.Prefix("Patcher")} Settings applied successfully!");
            Util.OpenMc();
            Console.WriteLine($"{Console.Prefix("Patcher")} Press any key to exit...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} An exception was thrown (please report this): {ex}");
        }
    }
    private static void Patcher()
    {
        Console.SetupConsole();
        M.Setup();
        bool newConfig = Config.LoadConfig();
        if (newConfig)
            ConfigPrompt();

        Console.WriteLine($"{Console.Prefix("Patcher")} Preparing...");
        string mcPath = Path.GetFullPath(Util.McProcess.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcPath: {mcPath}");
        string mcExe = mcPath + "Minecraft.Windows.exe";
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExe: {mcExe}");
        string mcExeBak = mcExe + ".bak";
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExeBak: {mcExeBak}");

        #region backup stuff
        string CurrentMcVersion = Util.McGetVersion();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} CurrentMcVersion: {CurrentMcVersion}");
        string LastMcVersionPath = Environment.ExpandEnvironmentVariables("%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\roamingstate\\lastMcVersion.txt");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} LastMcVersionPath: {LastMcVersionPath}");
        string LastMcVersion = "";
        if (!File.Exists(LastMcVersionPath))
            LastMcVersion = CurrentMcVersion;
        else
            LastMcVersion = File.ReadAllText(LastMcVersionPath).Replace("\n", "");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} LastMcVersion: {LastMcVersion}");
        if (LastMcVersion != CurrentMcVersion && File.Exists(mcExeBak))
        {
            Console.WriteLine($"{Console.PrefixColor}[McPatch]{Console.WarningTextColor} Minecraft version changed from {LastMcVersion} to {CurrentMcVersion}, recreating backup file.{Console.R}");
            File.Delete(mcExeBak);
            File.WriteAllText(LastMcVersionPath, CurrentMcVersion);
        }
        if (!File.Exists(mcExeBak)) File.Copy(mcExe, mcExeBak, true);
        #endregion
        //File.Copy(mcExeBak, mcTmpExe, true);
        string mcTmpHex = File.ReadAllBytes(mcExe).ToHexString().Replace(" ", "");
        Console.WriteLine($"{Console.Prefix("Patcher")} Applying settings...");
        // for every string in Fields.Address, output the string
        foreach (PropertyInfo property in typeof(Fields.Address).GetProperties())
        {
            string? PropertyName = property.Name;
            string? Address = property.GetValue(null).ToString();
            string? bytes = M.mem.ReadBytes(Address, 11).ToHexString().Replace(" ", "");
            if (Address == "0")
            {
                Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Unable to find address for {PropertyName}, skipping... (dead sig?)");
                break;
            }
            Console.WriteLine($"{Console.Prefix("Patcher Debug")} Field Info for {PropertyName}: {Address}:{M.mem.ReadBytes(Address, 11).ToHexString()}");
            switch (PropertyName)
            {
                case "GuiScaleAddr":
                    Fields.GuiScale = Config.CurrentConfig.GuiScale;
                    string? newBytes1 = M.mem.ReadBytes(Address, 11).ToHexString().Replace(" ", "");
                    mcTmpHex.Replace(bytes, newBytes1);
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
            mcTmpHex = mcTmpHex.Replace(bytes, newBytes);
        }
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Killing Minecraft");
        Util.McProcess.Kill();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {mcPath}");
        Util.GrantAccess(mcPath); // Make the original file writable
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new bytes to mcExe");
        File.WriteAllBytesAsync(mcExe, mcTmpHex.FromHexString()).Wait(); // Write the new contents to the temp file
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new version file");
        File.WriteAllText(LastMcVersionPath, CurrentMcVersion);
    }
    public static void ConfigPrompt()
    {
        while (true)
        {
            Console.SwitchToAlternativeBuffer();
            Console.WriteLine($"{Console.PrefixColor}[Configuration Menu]{Console.R}");
            Console.WriteLine("1) GuiScale: " + Config.CurrentConfig.GuiScale);
            Console.WriteLine("2) Always Sprint: " + (Config.CurrentConfig.AutoSprint ? "Enabled" : "Disabled"));
            Console.WriteLine("3) Fast Swing: " + (Config.CurrentConfig.FastSwing ? "Enabled" : "Disabled"));
            Console.WriteLine("4) Show Player Nametag: " + (Config.CurrentConfig.ShowNametag ? "Enabled" : "Disabled"));
            Console.WriteLine("5) Show Mob Nametag: " + (Config.CurrentConfig.ShowMobTag ? "Enabled" : "Disabled"));
            Console.WriteLine("6) Save Config");
            Console.WriteLine("7) Exit Menu");
            Console.Write("Select an option: ");
        ret:
            string selection = Console.ReadKey().KeyChar.ToString().ToLower();
            Console.WriteLine();
            if (selection == "1")
            {
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
            }
            else if (selection == "2")
            {
                Config.CurrentConfig.AutoSprint = !Config.CurrentConfig.AutoSprint;
            }
            else if (selection == "3")
            {
                Config.CurrentConfig.FastSwing = !Config.CurrentConfig.FastSwing;
            }
            else if (selection == "4")
            {
                Config.CurrentConfig.ShowNametag = !Config.CurrentConfig.ShowNametag;
            }
            else if (selection == "5")
            {
                Config.CurrentConfig.ShowMobTag = !Config.CurrentConfig.ShowMobTag;
            }
            else if (selection == "6")
            {
                Config.SaveConfig();
                Thread.Sleep(1000);
            }
            else if (selection == "7")
            {
                Console.SwitchToMainBuffer();
                Config.SaveConfig();
                Console.SwitchToMainBuffer();
                return;
            }
            else
            {
                if (Console.CursorLeft == 0) Console.CursorTop -= 1;
                Console.CursorLeft = 18;
                Console.Write($"{Console.ErrorTextColor}{selection}{Console.R}");
                Console.CursorLeft = 18;
                goto ret;
            }
        }
    }
}