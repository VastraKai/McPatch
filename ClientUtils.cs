using System.Diagnostics;

namespace McPatch;
public static class ClientUtils
{
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
