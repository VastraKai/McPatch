namespace McPatch;
public static class Fields
{
    public static float GuiScale
    {
        get
        {
            return M.mem.ReadFloat(Address.GuiScale, "float");
        }
        set
        {
            Config.CurrentConfig.GuiScale = value;
            M.mem.WriteMemory(Address.GuiScale, "float", value.ToString());
            Console.WriteLine($"{Console.Prefix("GuiScale")} Set to {value}");
        }
    }
    /*
     * public static string FastSwing = "00 ? ? 8B ? ? ? ? ? E8 ? ? ? ? 99 2B ?";
    public static string GuiScale = "00 00 ? ? 00 00 A0 40 00 00 C0";
    public static string Sprint = "48 ? ? 30 ? ? ? ? 40 ? ? 48 ? ? 0F ? ? ? ? 4?";
    public static string ShowName = "? ? ? ? ? ? 49 8B ? 49 8B ? E8 ? ? ? ? 84 ? ? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B";
    public static string AlwaysShowMobTag = "? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B ? ? ? ? ? 49 8B ? 48 8B ? ? ? ? ? ? ? ? ? ? ? 49";*/
    
    public static class Address
    {
        public static string? FastSwing => M.mem.GetAddressFromSig(Sigs.FastSwing, 1).ToString("X");
        public static string? GuiScale => M.mem.GetAddressFromSig(Sigs.GuiScale, 0, false, true, false).ToString("X");
        public static string? Sprint => M.mem.GetAddressFromSig(Sigs.Sprint, 4).ToString("X");
        public static string? ShowName => M.mem.GetAddressFromSig(Sigs.ShowName, 0).ToString("X");
        public static string? AlwaysShowMobTag => M.mem.GetAddressFromSig(Sigs.AlwaysShowMobTag, 0).ToString("X");
    }
}
