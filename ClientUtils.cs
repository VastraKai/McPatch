using System.Diagnostics;

namespace McPatch;
public static class ClientUtils
{
    private static string orig1 = "<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\">";
    private static string rep1 = "<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\" desktop4:SupportsMultipleInstances=\"true\">";
    private static string orig2 = "<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/2014/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\">";
    private static string rep2 = "<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/201/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\" xmlns:desktop4=\"http://schemas.microsoft.com/appx/manifest/desktop/windows10/4\">";
    public static bool ToggleMultiInstance()
    {
        string McPath = Path.GetFullPath(Util.McProcess.MainModule.FileName).Replace("Minecraft.Windows.exe", "");
        string McManContent = File.ReadAllText(McPath + "\\AppxManifest.xml");

        if (McManContent.Contains(orig1) && McManContent.Contains(orig2))
        {

            McManContent = McManContent.Replace(orig1, rep1);
            McManContent = McManContent.Replace(orig2, rep2);
            File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
            return true;
        }
        else if (McManContent.Contains(rep1) && McManContent.Contains(rep2))
        {
            McManContent = McManContent.Replace(rep1, orig1);
            McManContent = McManContent.Replace(rep2, orig2);
            File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
            return false;
        }
        return false;

    }
    public static void EnableMultiInstance(string McPath)
    {
        string McManContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        McManContent = McManContent.Replace(orig1, rep1);
        McManContent = McManContent.Replace(orig2, rep2);
        File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
    }
    public static void DisableMultiInstance(string McPath)
    {
        string McManContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        McManContent = McManContent.Replace(rep1, orig1);
        McManContent = McManContent.Replace(rep2, orig2);
        File.WriteAllText(McPath + "\\AppxManifest.xml", McManContent);
    }

    public static void WaitForMC()
    {
        try
        {
            Stopwatch loadTime = new();
            loadTime.Start();
            Process Minecraft = Process.GetProcessesByName("Minecraft.Windows").First();
            if (Minecraft.Modules.Count >= 165)
            {
                loadTime.Stop();
                Console.WriteLine($"{Console.PrefixColor}[Memory]{Console.GreenTextColor} Minecraft is already loaded" + Console.R);
                return;
            }
            string ModulesRead = "";
            while (true)
            {

                foreach (ProcessModule Module in Minecraft.Modules)
                {
                    if (!ModulesRead.Contains(Module.ModuleName + "\n"))
                    {
                        if (Minecraft.Modules.Count <= 165) Console.WriteLine($"{Console.PrefixColor}[Memory] {Console.ValueColor}{Module.ModuleName}{Console.GenericTextColor} loaded at {Console.ValueColor}{Module.BaseAddress.ToString("X")}{Console.GenericTextColor} in {Console.ValueColor}{Minecraft.MainModule.ModuleName}{Console.R}");
                        ModulesRead = ModulesRead + Module.ModuleName + "\n";
                    }
                    Console.Write($"{Console.PrefixColor}[Memory] {Console.ValueColor}{Minecraft.Modules.Count}{Console.GenericTextColor} modules loaded, {Console.ValueColor}{loadTime.Elapsed}{Console.GenericTextColor} seconds elapsed\r" + Console.R);
                }
                if (Minecraft.Modules.Count >= 165)
                {
                    loadTime.Stop();
                    Console.WriteLine($"{Console.PrefixColor}[Memory] {Console.GenericTextColor}Minecraft took {Console.ValueColor}{loadTime.Elapsed}{Console.GenericTextColor} to load                                             \r" + Console.R);
                    return;
                }
                Minecraft.Refresh();


                Thread.Sleep(10);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
