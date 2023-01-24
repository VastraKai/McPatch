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
    public static class Address
    {
        public static string? FastSwing => M.mem.GetAddressFromSig(Sigs.FastSwing, 1).ToString("X");
        public static string? GuiScale => M.mem.GetAddressFromSig(Sigs.GuiScale, 0, false, true, false).ToString("X");
        public static string? Sprint => M.mem.GetAddressFromSig(Sigs.Sprint, 4).ToString("X");
        public static string? ShowName => M.mem.GetAddressFromSig(Sigs.ShowName, 0).ToString("X");
        public static string? AlwaysShowMobTag => M.mem.GetAddressFromSig(Sigs.AlwaysShowMobTag, 0).ToString("X");
        public static string? ForceShowCoordinates => M.mem.GetAddressFromSig(Sigs.ForceShowCoordinates, 0xC).ToString("X");
    }
    public static class ReplaceBytes
    {
        public static string FastSwing = "EB";
        public static string GuiScale = "00 00 40 40";
        public static string Sprint = "49 8B C5 90";
        public static string ShowName = "90 90 90 90 90 90";
        public static string AlwaysShowMobTag = "90 90 90 90 90 90";
        public static string ForceShowCoordinates = "90 90 90 90";

    }
}
