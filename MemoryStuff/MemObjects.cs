namespace McPatch;
public static class Objects
{
    public static MemObject[] MemObjects = new MemObject[]
    {
        new MemObject(
            "CancelSwing",
            "00 ? ? 8B ? ? ? ? ? E8 ? ? ? ? 99 2B ?",
            1,
            "EB"),
        new MemObject(
            "GuiScale",
            "00 00 ? ? 00 00 A0 40 00 00 C0",
            0,
            "00 00 40 40",
            false),
        new MemObject(
            "AlwaysSprint", 
            "48 ? ? 30 ? ? ? ? 40 ? ? 48 ? ? 0F ? ? ? ? 4?",
            4, 
            "49 8B C5 90"),
        new MemObject(
            "ShowPlayerNametag",
            "? ? ? ? ? ? 49 8B ? 49 8B ? E8 ? ? ? ? 84 ? ? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B",
            0,
            "90 90 90 90 90 90"),
        new MemObject(
            "ForceShowNametags",
            "? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B ? ? ? ? ? 49 8B ? 48 8B ? ? ? ? ? ? ? ? ? ? ? 49",
            0,
            "90 90 90 90 90 90"),
        new MemObject(
            "ForceShowCoordinates",
            "? ? ? ? 09 00 00 48 85 C0 74 0D 80 78 04 00 74 07",
            0xC,
            "90 90 90 90")
    };
}