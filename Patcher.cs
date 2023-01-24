using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace McPatch;
public static class Patcher
{
    public static void BackupCheck()
    {
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} LastMcVersionPath: {LastMcVersionPath}");
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
    }
    public static void BackupRestore()
    {
        Console.WriteLine($"{Console.Prefix("Patcher")} {Console.Value("Restoring from backup...")}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {Console.Value(mcPath)}");
        Util.GrantAccess(mcPath);
        M.Dispose();
        while (Util.IsProcOpen("Minecraft.Windows")) { }
        Console.Write($"{Console.Prefix("Patcher Debug")} Restoring...\r");
        File.Copy(mcExeBak, mcExe, true);
        Console.Write($"\r      \r");

        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Resetting memory...");
        M.Setup(false);
        Console.WriteLine($"{Console.Prefix("Patcher")} {Console.GreenTextColor}Restore completed!{Console.R}");
    }
    public static void Backup()
    {
        bool BackupExists = File.Exists(mcExeBak);

        BackupCheck();
        if (!File.Exists(mcExeBak))
        {
            File.Copy(mcExe, mcExeBak, true);
        }
        if (BackupExists)
        {
            if (File.ReadAllBytes(mcExeBak) != File.ReadAllBytes(mcExe))
            {
                BackupRestore();
            }
            else
                Console.WriteLine($"{Console.WarningPrefix("Patcher")} Backup will not be used, Backup file is the same as original file.");
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
        GetMcStuff();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcPath: {mcPath}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExe: {mcExe}");
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} mcExeBak: {mcExeBak}");
        Backup();
        string mcHex = File.ReadAllBytes(mcExe).ToHexString().Replace(" ", "");
        Console.WriteLine($"{Console.Prefix("Patcher")} Applying settings...");
        int i = 0;
        foreach (PropertyInfo property in typeof(Fields.Address).GetProperties())
        {
            string? PropertyName = property.Name;
            string? Address = property.GetValue(null).ToString();
            if (!Address.StartsWith("7F"))
            {
                success = false;
                string? sig = typeof(Sigs).GetField(PropertyName).GetValue(null).ToString();
                Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Address for {Console.ValueColor}'{PropertyName}'{Console.R} not found, sig: {Console.ValueColor}'{sig}'{Console.R} {Console.ErrorTextColor}(Please report this!){Console.R}");
            }
            else
            {
                string? bytes = M.mem.ReadBytes(Address, 11).ToHexString().Replace(" ", "");
                Console.WriteLine($"{Console.Prefix("Patcher Debug")} Field Info for {PropertyName}: {Address}:{M.mem.ReadBytes(Address, 11).ToHexString()}");
                switch (PropertyName)
                {
                    case "GuiScale":
                        Fields.GuiScale = Config.CurrentConfig.GuiScale;
                        Console.WriteLine($"{Console.Prefix("Patcher")} GuiScale has been set to {Config.CurrentConfig.GuiScale}");
                        break;
                    case "Sprint":
                        if (!Config.CurrentConfig.AutoSprint) break;
                        M.mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.Sprint);
                        Console.WriteLine($"{Console.Prefix("Patcher")} AutoSprint has been enabled");
                        break;
                    case "FastSwing":
                        if (!Config.CurrentConfig.FastSwing) break;
                        M.mem.WriteMemory(Address, "byte", Fields.ReplaceBytes.FastSwing);
                        Console.WriteLine($"{Console.Prefix("Patcher")} FastSwing has been enabled");
                        break;
                    case "ShowName":
                        if (!Config.CurrentConfig.ShowNametag) break;
                        M.mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.ShowName);
                        Console.WriteLine($"{Console.Prefix("Patcher")} ShowName has been enabled");
                        break;
                    case "AlwaysShowMobTag":
                        if (!Config.CurrentConfig.ShowMobTag) break;
                        M.mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.AlwaysShowMobTag);
                        Console.WriteLine($"{Console.Prefix("Patcher")} ShowMobtag has been enabled");
                        break;
                    case "ForceShowCoordinates":
                        if (!Config.CurrentConfig.ForceShowCoordinates) break;
                        M.mem.WriteMemory(Address, "bytes", Fields.ReplaceBytes.ForceShowCoordinates);
                        Console.WriteLine($"{Console.Prefix("Patcher")} ForceShowCoordinates has been enabled");
                        break;
                    default:
                        Console.WriteLine($"{Console.WarningPrefix("Patcher")} Case for property " + PropertyName + " not found!");
                        break;
                }
                string? newBytes = M.mem.ReadBytes(Address, 11).ToHexString().Replace(" ", "");
                mcHex = mcHex.Replace(bytes, newBytes);
            }
            i++;
        }

        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Killing Minecraft");
        M.Dispose();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Setting access permissions for path {mcPath}");
        Util.GrantAccess(mcPath); 
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new bytes to mcExe");
        File.WriteAllBytesAsync(mcExe, mcHex.FromHexString()).Wait();
        MultiInstancePatch();
        Console.WriteLine($"{Console.Prefix("Patcher Debug")} Writing new version file");
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

