namespace McPatch;
public static class Fields
{
    public static class Sprint
    {
        public static void Enable()
        {
            OrigSprintBytes = M.mem.ReadBytes(Address.SprintAddr, 4);
            if (OrigSprintBytes.ToHexString() == "49 8B C5 90")
                Console.WriteLine($"{Console.WarningPrefix("AlwaysSprint")} AlwaysSprint seems to already be enabled!");
            M.mem.WriteMemory(Address.SprintAddr, "bytes", "49 8B C5 90");
            Console.WriteLine($"{Console.Prefix("AlwaysSprint")} Enabled");
            Config.CurrentConfig.AutoSprint = true;
        }
        public static void Disable()
        {
            OrigSprintBytes = M.mem.ReadBytes(Address.SprintAddr, 4);
            if (OrigSprintBytes.ToHexString() != "49 8B C5 90")
                Console.WriteLine($"{Console.WarningPrefix("AlwaysSprint")} AlwaysSprint seems to already be disabled!");
            M.mem.WriteMemory(Address.SprintAddr, "bytes", OrigSprintBytes.ToString());
            Console.WriteLine($"{Console.Prefix("AlwaysSprint")} Enabled");
            Config.CurrentConfig.AutoSprint = false;
        }
        private static byte[] OrigSprintBytes = Array.Empty<byte>();
        //private static byte[] SprintBytes = M.mem.ReadBytes(Address.SprintAddr, 4);
        //public static bool AutoSprint = false;
    }
    public static class FastSwing
    {
        public static void Enable()
        {
            if (M.mem.ReadByte(Address.FastSwingAddr).ToHexString() == "EB")
                Console.WriteLine($"{Console.WarningPrefix("FastSwing")} FastSwing seems to already be enabled!");
            M.mem.WriteMemory(Address.FastSwingAddr, "byte", "0xEB");
            Console.WriteLine($"{Console.Prefix("FastSwing")} Enabled");
            Config.CurrentConfig.FastSwing = true;
        }
        public static void Disable()
        {
            if (M.mem.ReadByte(Address.FastSwingAddr).ToHexString() == "74")
                Console.WriteLine($"{Console.WarningPrefix("FastSwing")} FastSwing seems to already be disabled!");
            M.mem.WriteMemory(Address.FastSwingAddr, "byte", "0x74");
            Console.WriteLine($"{Console.Prefix("FastSwing")} Disabled");
            Config.CurrentConfig.FastSwing = false;
        }
        //public static bool FastSwingEnabled = false;
    }
    public static class ShowNametag
    {
        public static void Enable()
        {
            byte[] currentBytes = M.mem.ReadBytes(Address.ShowNameAddr, 6);
            if (currentBytes.ToHexString() != "90 90 90 90 90 90")
            {
                OrigNametagBytes = currentBytes;
                M.mem.WriteMemory(Address.ShowNameAddr, "bytes", "90 90 90 90 90 90");
            }
            else
            {
                if (currentBytes == OrigNametagBytes)
                    Console.WriteLine($"{Console.WarningPrefix("ShowNametag")} ShowNametag seems to already be enabled!");
            }
            Console.WriteLine($"{Console.Prefix("ShowNametag")} Enabled");
            Config.CurrentConfig.ShowNametag = true;
        }
        public static void Disable()
        {
            byte[] currentBytes = M.mem.ReadBytes(Address.ShowNameAddr, 6);
            if (currentBytes == OrigNametagBytes)
                Console.WriteLine($"{Console.WarningPrefix("ShowNametag")} ShowNametag seems to already be disabled!");
            M.mem.WriteMemory(Address.ShowNameAddr, "bytes", OrigNametagBytes.ToHexString());
            Console.WriteLine($"{Console.Prefix("ShowNametag")} Disabled");
            Config.CurrentConfig.ShowNametag = false;
        }
        private static byte[] OrigNametagBytes = Array.Empty<byte>();
    }
    public static class ShowMobtag
    {
        public static void Enable()
        {
            byte[] currentBytes = M.mem.ReadBytes(Address.ShowMobtagAddr, 4);
            if (currentBytes.ToHexString() != "90 90 90 90 90 90")
            {
                OrigMobtagBytes = currentBytes;
                M.mem.WriteMemory(Address.ShowMobtagAddr, "bytes", "90 90 90 90 90 90");
            }
            else
            {
                Console.WriteLine($"{Console.WarningPrefix("ShowMobtag")} ShowMobtag seems to already be enabled!");
            }
            Config.CurrentConfig.ShowMobTag = true;
            Console.WriteLine($"{Console.Prefix("ShowMobtag")} Enabled");
        }

        public static void Disable()
        {
            byte[] currentBytes = M.mem.ReadBytes(Address.ShowNameAddr, 6);
            if (currentBytes == OrigMobtagBytes)
                Console.WriteLine($"{Console.WarningPrefix("ShowMobtag")} ShowMobtag seems to already be disabled!");
            M.mem.WriteMemory(Address.ShowMobtagAddr, "bytes", OrigMobtagBytes.ToHexString());
            Config.CurrentConfig.ShowMobTag = false;
            Console.WriteLine($"{Console.Prefix("ShowMobtag")} Disabled");
        }
        private static byte[] OrigMobtagBytes = Array.Empty<byte>();
    }
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
