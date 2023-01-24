using Memory;

namespace McPatch;
public static class M
{

    private static Mem MemI = new();

    public static Mem Mem { get => MemI; set => MemI = value; }

    //public static bool MemSetup = false;
    public static void Setup(bool waitForMcLoad)
    {
        //MemSetup = true;
        Mem = new Mem();
        if (!Util.IsProcOpen("Minecraft.Windows")) Util.OpenMc();
        while (!Util.IsProcOpen("Minecraft.Windows")) { }
        if (waitForMcLoad) ClientUtils.WaitForMC();
        Mem.OpenProcess("Minecraft.Windows");
        Console.WriteLine($"{Console.PrefixColor}[Memory]{Console.GreenTextColor} Initialized successfully.{Console.R}");
    }
    public static void Dispose()
    {
        try
        {
            //MemSetup = false;
            Util.McProcess?.Kill();
            M.Mem.CloseProcess();
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        }
        catch { }
    }
}
