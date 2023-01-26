using System.Reflection;

namespace McPatch;
public static class Patcher
{
    public static void BackupCheck()
    {
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} LastMcVersionPath: {Console.Value(LastMcVersionPath)}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} CurrentMcVersion: {Console.Value(CurrentMcVersion)}");

        string LastMcVersion;
        if (!File.Exists(LastMcVersionPath))
            LastMcVersion = CurrentMcVersion;
        else
            LastMcVersion = File.ReadAllText(LastMcVersionPath).Replace("\n", "");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} LastMcVersion: {Console.Value(LastMcVersion)}");

        if (LastMcVersion != CurrentMcVersion && File.Exists(mcExeBak))
        {
            Console.WriteLine($"{Console.WarningPrefix("Patcher")} Minecraft version changed from {Console.Value(LastMcVersion)} to {Console.Value(CurrentMcVersion)}, recreating backup file.{Console.R}");
            File.Delete(mcExeBak);
            File.WriteAllText(LastMcVersionPath, CurrentMcVersion);
        }
    }
    public static void BackupRestore()
    {
        Console.WriteLine($"{Console.Prefix("Patcher")} {Console.Value("Restoring from backup...")}");
        //Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {Console.Value(mcPath)}");
        //Util.GrantAccess(mcPath);
        M.Dispose();
        while (Util.IsProcOpen("Minecraft.Windows")) { }
        Console.Write($"{Console.Prefix("Patcher Debug")} Restoring...\r");
        File.Copy(mcExeBak, mcExe, true);
        Console.Write($"\r           \r");

        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Resetting memory...");
        M.Setup(false);
        Console.WriteLine($"{Console.Prefix("Patcher")} {Console.GreenTextColor}Restore completed!{Console.R}");
    }
    public static void DoBackupStuff()
    {
        bool BackupExists = File.Exists(mcExeBak);

        BackupCheck();
        if (!File.Exists(mcExeBak))
        {
            File.Copy(mcExe, mcExeBak, true);
        }
        if (BackupExists)
        {
            BackupRestore();
        }
        else
            Console.WriteLine($"{Console.WarningPrefix("Patcher")} Backup will not be used!");
    }
    private static string mcPath = string.Empty;
    private static string mcExe = string.Empty;
    private static string mcExeBak = string.Empty;
    private static string LastMcVersionPath = string.Empty;
    private static string CurrentMcVersion = string.Empty;

    public static void GetMcStuff()
    {
        if (Util.McProcess != null && Util.McProcess.MainModule != null)
            mcPath = Path.GetFullPath(Util.McProcess.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        mcExe = mcPath + "Minecraft.Windows.exe";
        mcExeBak = mcExe + ".bak";
        LastMcVersionPath = Environment.ExpandEnvironmentVariables("%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\roamingstate\\lastMcVersion.txt");
        CurrentMcVersion = Util.McGetVersion();
    }
    public static bool Patch()
    {
        bool success = true;
        M.Setup(false);
        Console.WriteLine($"{Console.Prefix("Patcher")} Preparing...");
        GetMcStuff();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcPath: {Console.Value(mcPath)}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExe: {Console.Value(mcExe)}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExeBak: {Console.Value(mcExeBak)}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {Console.Value(mcPath)}");
        Util.GrantAccess(mcPath);
        DoBackupStuff();

        string mcHex = File.ReadAllBytes(mcExe).ToHexStringO();
        Console.WriteLine($"{Console.Prefix("Patcher")} Applying settings...");
        int i = 0;
        foreach (PropertyInfo property in typeof(Fields.Address).GetProperties())
        {
            string? PropertyName = property.Name;
            object? a = property.GetValue(null);

            string? Address;
            if (a != null) Address = a.ToString();
            else Address = "0";
            Address ??= "0";
            if (!Address.StartsWith("7F"))
            {
                success = false;
                FieldInfo? fieldInfo = typeof(Sigs).GetField(PropertyName);

                if (fieldInfo == null) break;
                object? sigObj = fieldInfo.GetValue(null);
                if (sigObj == null) break;
                string? sig = sigObj.ToString();
                Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Address for {Console.Value($"'{PropertyName}'")} not found, sig: {Console.Value($"'{sig}'")} {Console.ErrorTextColor}(Please report this!){Console.R}");
            }
            else
            {
                string? bytes = M.Mem.ReadBytes(Address, 11).ToHexStringO();
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Field Info for {Console.Value($"'{PropertyName}'")}: {Console.Value($"'{Address}'")}:{Console.Value($"'{M.Mem.ReadBytes(Address, 11).ToHexString()}'")} ");
                switch (PropertyName)
                {
                    case "GuiScale":
                        Fields.GuiScale = Config.CurrentConfig.GuiScale;
                        Console.WriteLine($"{Console.Prefix("Patcher")} GuiScale has been set to {Console.Value($"{Config.CurrentConfig.GuiScale}")}");
                        break;
                    case "Sprint":
                        if (!Config.CurrentConfig.AutoSprint) break;
                        M.Mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.Sprint);
                        Console.WriteLine($"{Console.Prefix("Patcher")} AutoSprint has been enabled");
                        break;
                    case "FastSwing":
                        if (!Config.CurrentConfig.FastSwing) break;
                        M.Mem.WriteMemory(Address, "byte", Fields.ReplaceBytes.FastSwing);
                        Console.WriteLine($"{Console.Prefix("Patcher")} FastSwing has been enabled");
                        break;
                    case "ShowName":
                        if (!Config.CurrentConfig.ShowNametag) break;
                        M.Mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.ShowName);
                        Console.WriteLine($"{Console.Prefix("Patcher")} ShowName has been enabled");
                        break;
                    case "AlwaysShowMobTag":
                        if (!Config.CurrentConfig.ShowMobTag) break;
                        M.Mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.AlwaysShowMobTag);
                        Console.WriteLine($"{Console.Prefix("Patcher")} ShowMobtag has been enabled");
                        break;
                    case "ForceShowCoordinates":
                        if (!Config.CurrentConfig.ForceShowCoordinates) break;
                        M.Mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.ForceShowCoordinates);
                        Console.WriteLine($"{Console.Prefix("Patcher")} ForceShowCoordinates has been enabled");
                        break;
                    default:
                        Console.WriteLine($"{Console.WarningPrefix("Patcher")} Case for property " + PropertyName + " not found!");
                        break;
                }
                string? newBytes = M.Mem.ReadBytes(Address, 11).ToHexStringO();
                mcHex = mcHex.Replace(bytes, newBytes);
            }
            i++;
        }

        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Killing Minecraft");
        M.Dispose();
        //Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {Console.Value(mcPath)}");
        //Util.GrantAccess(mcPath);
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new bytes to EXE");
        FileUtils.WriteFile(mcExe, mcHex.FromHexStringO());
        MultiInstancePatch();

        File.WriteAllText(LastMcVersionPath, CurrentMcVersion);
        return success;
    }
    public static void MultiInstancePatch()
    {
        if (Util.IsDeveloperModeEnabled())
        {
            if (Config.CurrentConfig.McMultiInstance)
            {
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Enabling multi instance, patching AppxManifest file");
                bool ReregisterNeeded = ClientUtils.EnableMultiInstance(mcPath);
                if (ReregisterNeeded)
                {
                    Console.WriteLine($"{Console.Prefix("Patcher Debug")} Re-registering appx");
                    Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", mcPath).Wait();
                }
                else
                {
                    Console.WriteLine($"{Console.Prefix("Patcher Debug")} Multi-instance is already enabled.");
                }
            }
            else
            {
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Disabling multi instance, patching AppxManifest file");
                bool ReregisterNeeded = ClientUtils.DisableMultiInstance(mcPath);
                if (ReregisterNeeded)
                {
                    Console.WriteLine($"{Console.Prefix("Patcher Debug")} Re-registering appx");
                    Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", mcPath).Wait();
                }
                else
                {
                    Console.WriteLine($"{Console.Prefix("Patcher Debug")} Multi-instance is already disabled.");
                }
            }
        }
        else
        {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")}{Console.ErrorTextColor} Unable to use multi-instance: Developer mode is not enabled.{Console.R}");
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")}{Console.ErrorTextColor} Please enable developer mode in ms-settings:developers.{Console.R}");
        }
    }
}

