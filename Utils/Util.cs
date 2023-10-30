using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using Windows.ApplicationModel;
using Windows.Management.Core;
using Windows.Management.Deployment;

namespace McPatch.Utils;
public static class Util
{
    /// <summary>
    /// Structure for getModuleInMemory
    /// </summary>
    public struct ModBeginEnd
    {
        public IntPtr beginning;
        public IntPtr end;
    }
    public static bool IsAppxInstalled(string Package)
    {
        bool isInstalled = true;
        // powershell.exe Get-AppPackage Microsoft.XboxApp ^| select -expandproperty Name
        try
        {
            string? pkgOutput = "";
            string path = Environment.ExpandEnvironmentVariables("%systemroot%");
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path + "\\system32\\cmd.exe",
                    Arguments = $"/c powershell.exe Get-AppPackage {Package} ^| select -expandproperty Name",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string? line = proc.StandardOutput.ReadLine();
                pkgOutput = line;
            }
            pkgOutput ??= "";
            if (pkgOutput.Trim().Replace(" ", "") == "") isInstalled = false;
            return isInstalled;
        }
        catch
        {
            return false;
        }
    }
    public static string McGetVersion() // gets the current Minecraft Bedrock version (this is a pain in the ass)
    {
        try
        {
            string? version = "Unknown";
            string path = Environment.ExpandEnvironmentVariables("%systemroot%");
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path + "\\system32\\cmd.exe",
                    Arguments = "/c powershell.exe Get-AppPackage -name Microsoft.MinecraftUWP ^| select -expandproperty Version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string? line = proc.StandardOutput.ReadLine();
                version = line;
            }
            version ??= "Unknown";
            return version;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to get version: " + ex);
            return "Unknown";
        }
    }

    /// <summary>
    /// Returns the base address and end address of the requested module.
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns>MinMax</returns>
    public static ModBeginEnd GetModuleInMemory(string moduleName)
    {
        ModBeginEnd modBeginEnd;
        modBeginEnd.beginning = 0;
        modBeginEnd.end = 0;
        Process? mcproc = McProcess;
        if (mcproc != null && mcproc.MainModule != null)
        {
            if (mcproc.ProcessName == moduleName)
            {
                modBeginEnd.beginning = mcproc.MainModule.BaseAddress;
                modBeginEnd.end = mcproc.MainModule.BaseAddress + mcproc.MainModule.ModuleMemorySize;
            }
            else
            {
                ProcessModuleCollection mods = mcproc.Modules;
                foreach (ProcessModule mod in mods)
                {

                    if (mod.ModuleName == moduleName)
                    {
                        modBeginEnd.beginning = mod.BaseAddress;
                        modBeginEnd.end = mod.BaseAddress + mod.ModuleMemorySize;
                    }
                }
            }
        }
        return modBeginEnd;
    }
    public static ModBeginEnd McMem => GetModuleInMemory("Minecraft.Windows");
    public static void OpenMc() => Process.Start("cmd.exe", "/c start \"\" shell:AppsFolder\\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App");
    public static void Kill() => Process.GetCurrentProcess().Kill();

    public static Process? McProcess => Process.GetProcessesByName("Minecraft.Windows").FirstOrDefault();
    public static void GrantAccess(string fullPath)
    {
        try
        {
            DirectoryInfo dInfo = new(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            // Set the owner of the directory
            dSecurity.SetOwner(new NTAccount(Environment.UserName));
            dSecurity.SetAccessRuleProtection(true, false);
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None, AccessControlType.Allow));
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-15-2-1"),
                FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        } catch (Exception ex)
        {
            Console.Log.WriteLine("Utils", "Unable to grant access to " + fullPath + ": " + ex, LogLevel.Error);
        }
    }
    public static bool IsProcOpen(string processName)
    {
        try
        {
            Process? process = Process.GetProcessesByName(processName).FirstOrDefault();
            if (process == null) throw new NullReferenceException("Process is null");
            string proc = process.ProcessName;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    #region These methods are from https://github.com/MCMrARM/mc-w10-version-launcher/blob/master/MCLauncher/MainWindow.xaml.cs
    public static string GetBackupMinecraftDataDir()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string tmpDir = Path.Combine(localAppData, "TmpMinecraftLocalState");
        return tmpDir;
    }

    public static void BackupMinecraftDataForRemoval(string packageFamily)
    {
        var data = ApplicationDataManager.CreateForPackageFamily(packageFamily);
        string tmpDir = GetBackupMinecraftDataDir();
        if (Directory.Exists(tmpDir))
        {
            Process.Start("explorer.exe", tmpDir);
            throw new Exception("Temporary dir exists");
        }
        Directory.Move(data.LocalFolder.Path, tmpDir);
    }

    public static void RestoreMove(string from, string to)
    {
        foreach (var f in Directory.EnumerateFiles(from))
        {
            string ft = Path.Combine(to, Path.GetFileName(f));
            if (File.Exists(ft))
                File.Delete(ft);

            File.Move(f, ft);
        }
        foreach (var f in Directory.EnumerateDirectories(from))
        {
            string tp = Path.Combine(to, Path.GetFileName(f));
            if (!Directory.Exists(tp))
            {
                if (File.Exists(tp))
                    continue;
                Directory.CreateDirectory(tp);
            }
            RestoreMove(f, tp);
        }
    }

    public static void RestoreMinecraftDataFromReinstall(string packageFamily)
    {
        string tmpDir = GetBackupMinecraftDataDir();
        if (!Directory.Exists(tmpDir))
            return;
        var data = ApplicationDataManager.CreateForPackageFamily(packageFamily);
        RestoreMove(tmpDir, data.LocalFolder.Path);
        Directory.Delete(tmpDir, true);
    }

    public static async Task RemovePackage(Package pkg, string packageFamily)
    {
        if (!pkg.IsDevelopmentMode)
        {
            BackupMinecraftDataForRemoval(packageFamily);
            await new PackageManager().RemovePackageAsync(pkg.Id.FullName, 0).AsTask();
        }
        else
        {
            await new PackageManager().RemovePackageAsync(pkg.Id.FullName, RemovalOptions.PreserveApplicationData).AsTask();
        }
    }

    public static string GetPackagePath(Package pkg)
    {
        try
        {
            return pkg.InstalledLocation.Path;
        }
        catch (FileNotFoundException)
        {
            return "";
        }
    }

    public static async Task UnregisterPackage(string packageFamily, string gameDir)
    {
        foreach (var pkg in new PackageManager().FindPackages(packageFamily))
        {
            string location = GetPackagePath(pkg);
            if (location == "" || location == gameDir)
            {
                await RemovePackage(pkg, packageFamily);
            }
        }
    }

    public static async Task ReRegisterPackage(string packageFamily, string gameDir)
    {
        foreach (var pkg in new PackageManager().FindPackages(packageFamily))
        {
            string location = GetPackagePath(pkg);
            if (location == gameDir)
            {
                return;
            }
            await RemovePackage(pkg, packageFamily);
        }
        string manifestPath = Path.Combine(gameDir, "AppxManifest.xml");
        new PackageManager().RegisterPackageAsync(new Uri(manifestPath), null, DeploymentOptions.DevelopmentMode).AsTask().Wait();
        RestoreMinecraftDataFromReinstall(packageFamily);
    }
    #endregion

    // Check if developer mode is enabled with the key name "AllowDevelopmentWithoutDevLicense"
    public static bool IsDeveloperModeEnabled()
    {
        try
        {
            // Check if dev mode is enabled using the error code returned from cmd /c "reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock /v AllowDevelopmentWithoutDevLicense | findstr /I /C:"0x1""
            // Create a process to run the command
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\cmd.exe"),
                    Arguments =
                        "/c \"reg add HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AppModelUnlock /v AllowDevelopmentWithoutDevLicense /t REG_DWORD /d 1 /f\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            p.WaitForExit();

            p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\cmd.exe"),
                    Arguments =
                        "/c \"reg query HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AppModelUnlock /v AllowDevelopmentWithoutDevLicense | findstr /I /C:\"0x1\"\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            p.WaitForExit();
            // Get the output
            string output = p.StandardOutput.ReadToEnd();
            // Check if the output contains "0x1"
            return output.Contains("0x1");
        }
        catch (Exception ex)
        {
            Console.Log.WriteLine("Utils", "&cAn exception was caught while checking if developer mode is enabled: " + ex, LogLevel.Error);
            return true;
        }
    }
}

