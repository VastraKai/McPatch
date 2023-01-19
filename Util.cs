using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace McPatch;
public static class Util
{
    [DllImport("user32.dll")]
    private static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void FocusProcess(string procName)
    {
        foreach (var pr in Process.GetProcesses())
        {
            if (pr.ProcessName != procName) continue;
            var hWnd = pr.MainWindowHandle;
            ShowWindow(hWnd, 3);
            SetForegroundWindow(hWnd);
        }
    }
    /// <summary>
    /// Structure for getModuleInMemory
    /// </summary>
    public struct ModBeginEnd
    {
        public IntPtr beginning;
        public IntPtr end;
    }
    
    public static string McGetVersion() // gets the current Minecraft Bedrock version (this is a pain in the ass)
    {
        try
        {
            string version = "Unknown";
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
                string line = proc.StandardOutput.ReadLine();
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
    public static void OpenMc() => Process.Start("explorer.exe", "Minecraft://");
    public static void Kill() => Process.GetCurrentProcess().Kill();
    public static ModBeginEnd McMem
    {
        get
        {
            return GetModuleInMemory("Minecraft.Windows");
        }
    }
    public static Process? McProcess
    {
        get
        {
            return Process.GetProcessesByName("Minecraft.Windows").FirstOrDefault();
        }
    }
    public static void GrantAccess(string fullPath)
    {
        DirectoryInfo dInfo = new(fullPath);
        DirectorySecurity dSecurity = dInfo.GetAccessControl();
        // Set the owner of the directory
        dSecurity.SetOwner(new NTAccount(Environment.UserName));
        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
        dInfo.SetAccessControl(dSecurity);
    }
    public static bool IsProcOpen(string process)
    {
        try
        {
            string proc = Process.GetProcessesByName(process).FirstOrDefault().ProcessName;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

