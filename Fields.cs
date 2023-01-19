namespace McPatch;
public static class Fields
{
    public static float GuiScale
    {
        get
        {
            return M.mem.ReadFloat(Address.GuiScaleAddr, "float");
        }
        set
        {
            Config.CurrentConfig.GuiScale = value;
            M.mem.WriteMemory(Address.GuiScaleAddr, "float", value.ToString());
            Console.WriteLine($"{Console.Prefix("GuiScale")} Set to {value}");
        }
    }
    public static class Address
    {
        public static string? GuiScaleAddr => M.mem.GetAddressFromSig(Sigs.GuiScale, 0, false, true, false).ToString("X");
        public static string? SprintAddr => M.mem.GetAddressFromSig(Sigs.Sprint, 4).ToString("X");
        public static string? FastSwingAddr => M.mem.GetAddressFromSig(Sigs.FastSwing, 1).ToString("X");
        public static string? ShowNameAddr => M.mem.GetAddressFromSig(Sigs.ShowName, 0).ToString("X");
        public static string? ShowMobtagAddr => M.mem.GetAddressFromSig(Sigs.AlwaysShowMobTag, 0).ToString("X");
    }
}
