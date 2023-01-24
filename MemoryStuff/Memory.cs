using Memory;

namespace McPatch;
public static class M
{
    public static Mem mem = new();
    public static void Setup(bool waitForMcLoad)
    {
        mem = new Mem();
        if (!Util.IsProcOpen("Minecraft.Windows")) Util.OpenMc();
        while (!Util.IsProcOpen("Minecraft.Windows")) { }
        if (waitForMcLoad) ClientUtils.WaitForMC();
        mem.OpenProcess("Minecraft.Windows");
        Console.WriteLine($"{Console.PrefixColor}[Memory]{Console.GreenTextColor} Initialized successfully.{Console.R}");
    }
    public static void Dispose()
    {
        Util.McProcess.Kill();
        M.mem.CloseProcess();
        GC.Collect();
    }
}
