namespace McPatch;
public static class Objects
{
    public static MemObject[] MemObjects = new MemObject[]
    {
        // Each MemObject is a patch that will be applied to the game.
        // The first parameter must have a corresponding config field.
        new MemObject("CancelSwing")
        {
            Sig = "00 ? ? 8B ? ? ? ? ? E8 ? ? ? ? 99 2B ?",
            SigOffset = 1,
            NewBytes = "EB"
        },
        new MemObject("GuiScale")
        {
            Sig = "00 00 ? ? 00 00 A0 40 00 00 C0",
            SigOffset = 0,
            NewBytes = "00 00 40 40",
            Executable = false
        },
        new MemObject("AlwaysSprint")
        {
            Sig = "48 ? ? 30 ? ? ? ? 40 ? ? 48 ? ? 0F ? ? ? ? 4?",
            SigOffset = 4,
            NewBytes = "49 8B C5 90"
        },
        new MemObject("ShowPlayerNametag")
        {
            Sig = "? ? ? ? ? ? 49 8B ? 49 8B ? E8 ? ? ? ? 84 ? ? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B",
            SigOffset = 0,
            NewBytes = "90 90 90 90 90 90"
        },
        new MemObject("ForceShowNametags")
        {
            Sig = "? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B ? ? ? ? ? 49 8B ? 48 8B ? ? ? ? ? ? ? ? ? ? ? 49",
            SigOffset = 0,
            NewBytes = "90 90 90 90 90 90"
        },
        new MemObject("ForceShowCoordinates")
        {
            Sig = "? ? ? ? 09 00 00 48 85 C0 74 0D ? ? ? ? 74 07",
            SigOffset = 12,
            NewBytes = "90 90 90 90"
        }
    };
}