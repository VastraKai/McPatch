
using McPatch.ConsoleMenu;
using McPatch.Utils;

namespace McPatch;
public static class Patcher
{
    private static string _mcPath = string.Empty;
    private static string _mcExe = string.Empty;
    private static string _mcExeBak = string.Empty;
    public static string RoamingState => Environment.ExpandEnvironmentVariables("%localappdata%\\packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\RoamingState");
    private static string CurrentMcSize => new FileInfo(_mcExe).Length.ToString();
    public static void BackupCheck()
    {
        Console.Log.WriteLine("Patcher", $"Checking backup...", LogLevel.Debug);
        Console.Log.WriteLine("Patcher", $"Current Minecraft size: &v{CurrentMcSize}&r", LogLevel.Debug);
        if (File.Exists(_mcExeBak) && CurrentMcSize != new FileInfo(_mcExeBak).Length.ToString())
        {
            Console.Log.WriteLine("Patcher", $"Backup Minecraft size: &v{new FileInfo(_mcExeBak).Length}&r", LogLevel.Debug);
            Console.Log.WriteLine("Patcher", $"&eMinecraft size changed, recreating backup file.", LogLevel.Warning);
            File.Delete(_mcExeBak);
        }
    }
    public static void BackupRestore()
    {
        Console.Log.WriteLine("Patcher", $"Restoring from backup...", LogLevel.Info);
        Mu.Dispose();
        while (Util.IsProcOpen("Minecraft.Windows")) { }
        Console.Log.Write("Patcher", "Restoring...&r\r", LogLevel.Debug);
        File.Copy(_mcExeBak, _mcExe, true);
        Console.Write($"\r           \r");
        
        Console.Log.WriteLine("Patcher", $"&rResetting memory...", LogLevel.Debug);
        Mu.Setup();
        Console.Log.WriteLine("Patcher", $"&aRestore completed!&r", LogLevel.Success);
    }
    public static void DoBackupStuff()
    {
        bool BackupExists = File.Exists(_mcExeBak);

        BackupCheck();
        if (!File.Exists(_mcExeBak))
        {
            File.Copy(_mcExe, _mcExeBak, true);
        }
        if (BackupExists)
        {
            BackupRestore();
        }
        else
            Console.Log.WriteLine("Patcher", "Backup will not be used!", LogLevel.Warning);
    }

    public static void GetMcStuff()
    {
        if (Util.McProcess != null && Util.McProcess.MainModule != null)
            _mcPath = Path.GetFullPath(Util.McProcess.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        _mcExe = _mcPath + "Minecraft.Windows.exe";
        _mcExeBak = _mcExe + ".bak";
    }
    public static bool Patch()
    {
        bool success = true;
        Console.Log.WriteLine("Patcher", "Preparing...");
        if (!Util.IsAppxInstalled("Microsoft.Minecraft*"))
        {
            Console.Log.WriteLine("Patcher", "Minecraft is not installed! Please install Minecraft using McLauncher then try again.", LogLevel.Error);
            return false;
        }
        Mu.Setup();
        GetMcStuff();
        Console.Log.WriteLine("Patcher", $"mcPath: &v{_mcPath}&r", LogLevel.Debug);
        Console.Log.WriteLine("Patcher", $"mcExe: &v{_mcExe}&r", LogLevel.Debug);
        Console.Log.WriteLine("Patcher", $"mcExeBak: &v{_mcExeBak}&r", LogLevel.Debug);
        if (_mcPath.Contains("Program Files\\WindowsApps\\Microsoft.Minecraft", StringComparison.CurrentCultureIgnoreCase)) {
            Console.Log.WriteLine("Patcher", "The default Minecraft directory is not writable.", LogLevel.Error);
            Console.Log.WriteLine("Patcher", "View the readme for more info.", LogLevel.Error);
            return false;
        }
        Console.Log.WriteLine("Patcher", $"Setting access permissions for path &v{_mcPath}&r", LogLevel.Debug);
        Util.GrantAccess(_mcPath);
        DoBackupStuff();

        string mcHex = File.ReadAllBytes(_mcExe).ToHexStringO();
        Console.Log.WriteLine("Patcher", $"Scanning...", LogLevel.Debug);
        foreach (MemObject obj in Objects.MemObjects) if(!obj.Scanned) obj.Scan();
        Console.Log.WriteLine("Patcher", $"Applying settings...", LogLevel.Debug);
        foreach (MemObject obj in Objects.MemObjects)
        {
            bool PatchApplied = obj.PatchApply(mcHex, out mcHex);
            if (success) success = PatchApplied;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        }
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        Console.Log.WriteLine("Patcher", $"Killing Minecraft", LogLevel.Debug);
        Mu.Dispose();
        MultiInstancePatch();
        Console.Log.WriteLine("Patcher", $"Writing new bytes to EXE", LogLevel.Debug);
        FileUtils.WriteFile(_mcExe, mcHex.FromHexStringO());
        return success;
    }
    public static void MultiInstancePatch()
    {
        if (Util.IsDeveloperModeEnabled())
        {
            if (((BoolMenuItem)Program.ConfigMenu.GetMenuItem("McMultiInstance")!).Value)
            {
                Console.Log.WriteLine("Patcher", $"Enabling multi instance, patching AppxManifest file", LogLevel.Debug);
                bool ReregisterNeeded = ClientUtils.EnableMultiInstance(_mcPath);
                if (ReregisterNeeded)
                {
                    Console.Log.WriteLine("Patcher", $"Re-registering appx", LogLevel.Debug);
                    Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", _mcPath).Wait();
                }
                else
                {
                    Console.Log.WriteLine("Patcher", $"Multi-instance is already enabled.", LogLevel.Debug);
                }
            }
            else
            {
                Console.Log.WriteLine("Patcher", $"Disabling multi instance, patching AppxManifest file", LogLevel.Debug);
                bool ReregisterNeeded = ClientUtils.DisableMultiInstance(_mcPath);
                if (ReregisterNeeded)
                {
                    Console.Log.WriteLine("Patcher", $"Re-registering appx", LogLevel.Debug);
                    Util.ReRegisterPackage("Microsoft.MinecraftUWP_8wekyb3d8bbwe", _mcPath).Wait();
                }
                else
                {
                    Console.Log.WriteLine("Patcher", $"Multi-instance is already disabled.", LogLevel.Debug);
                }
            }
        }
        else
        {
            Console.Log.WriteLine("Patcher", $"&cUnable to use multi-instance: Developer mode is not enabled.&r", LogLevel.Error);
            Console.Log.WriteLine("Patcher", $"&cPlease enable developer mode in ms-settings:developers.&r", LogLevel.Error);
        }
    }
}

