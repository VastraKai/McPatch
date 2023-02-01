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

        Console.WriteLine($"{Console.Prefix("Patcher")} Preparing...");
        if (!Util.IsAppxInstalled("Microsoft.Minecraft*"))
        {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Minecraft is not installed! Please install Minecraft using McLauncher then try again.");
            return false;
        }
        M.Setup(false);
        GetMcStuff();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcPath: {Console.Value(mcPath)}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExe: {Console.Value(mcExe)}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExeBak: {Console.Value(mcExeBak)}");
        if (mcPath.Contains("Program Files\\WindowsApps\\Microsoft.Minecraft", StringComparison.CurrentCultureIgnoreCase)) {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} The default Minecraft directory is not writable.");
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Visit https://github.com/VastraKai/McPatch/commit/458c104322c4a22ab20cd2a57ad2a3309776eb84 for more info.");
            return false;
        }
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {Console.Value(mcPath)}");
        Util.GrantAccess(mcPath);
        DoBackupStuff();

        string mcHex = File.ReadAllBytes(mcExe).ToHexStringO();
        Console.WriteLine($"{Console.Prefix("Patcher")} Scanning...");
        foreach (MemObject obj in Objects.MemObjects) if(!obj.Scanned) obj.Scan();
        Console.WriteLine($"{Console.Prefix("Patcher")} Applying settings...");
        foreach (MemObject obj in Objects.MemObjects)
        {
            bool PatchApplied = obj.PatchApply(mcHex, out mcHex);
            if (success) success = PatchApplied;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        }
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Killing Minecraft");
        M.Dispose();
        MultiInstancePatch();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new bytes to EXE");
        FileUtils.WriteFile(mcExe, mcHex.FromHexStringO());
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

