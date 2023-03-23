namespace McPatch;
public static class Objects
{
    public static MemObject[] MemObjects = new MemObject[]
    {
        // Each MemObject is a patch that will be applied to the game.
        // The first parameter must have a corresponding config field.
        new MemObject("CancelSwing")
        {
            Sig = "00 ? ? 8B ? ? ? ? ? E8 ? ? ? ? 99 2B",
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
            Sig = "C7 81 ? ? ? ? ? ? ? ? 66 ? ? ? ? ? ? ? ? C6 ? ? ? ? ? ? C7",
            SigOffset = 9,
            NewBytes = "01"
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
/* // For AlwaysSprint, addresses from MC 1.19.70
// ORIGINAL CODE - INJECTION POINT: Minecraft.Windows.exe+6CA0C0

Minecraft.Windows.exe+6CA08A: C7 41 54 00 00 00 00           - mov [rcx+54],00000000
Minecraft.Windows.exe+6CA091: 66 C7 41 5F 00 00              - mov word ptr [rcx+5F],0000
Minecraft.Windows.exe+6CA097: C6 41 5E 00                    - mov byte ptr [rcx+5E],00
Minecraft.Windows.exe+6CA09B: C7 41 6C 00 00 00 00           - mov [rcx+6C],00000000
Minecraft.Windows.exe+6CA0A2: 80 79 70 00                    - cmp byte ptr [rcx+70],00
Minecraft.Windows.exe+6CA0A6: 75 04                          - jne Minecraft.Windows.exe+6CA0AC
Minecraft.Windows.exe+6CA0A8: C6 41 50 00                    - mov byte ptr [rcx+50],00
Minecraft.Windows.exe+6CA0AC: 66 C7 41 4B 00 00              - mov word ptr [rcx+4B],0000
Minecraft.Windows.exe+6CA0B2: C6 41 4A 00                    - mov byte ptr [rcx+4A],00
Minecraft.Windows.exe+6CA0B6: C7 81 9A 00 00 00 00 00 00 00  - mov [rcx+0000009A],00000000
// ---------- INJECTING HERE ----------
Minecraft.Windows.exe+6CA0C0: C7 81 94 00 00 00 00 00 00 00  - mov [rcx+00000094],00000000
// ---------- DONE INJECTING  ----------
Minecraft.Windows.exe+6CA0CA: 66 C7 81 9F 00 00 00 00 00     - mov word ptr [rcx+0000009F],0000
Minecraft.Windows.exe+6CA0D3: C6 81 9E 00 00 00 00           - mov byte ptr [rcx+0000009E],00
Minecraft.Windows.exe+6CA0DA: C7 81 AC 00 00 00 00 00 00 00  - mov [rcx+000000AC],00000000
Minecraft.Windows.exe+6CA0E4: 75 07                          - jne Minecraft.Windows.exe+6CA0ED
Minecraft.Windows.exe+6CA0E6: C6 81 90 00 00 00 00           - mov byte ptr [rcx+00000090],00
Minecraft.Windows.exe+6CA0ED: C3                             - ret 
Minecraft.Windows.exe+6CA0EE: CC                             - int 3 
Minecraft.Windows.exe+6CA0EF: CC                             - int 3 
Minecraft.Windows.exe+6CA0F0: 40 53                          - push rbx
Minecraft.Windows.exe+6CA0F2: 48 83 EC 20                    - sub rsp,20
*/

