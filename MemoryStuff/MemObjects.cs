namespace McPatch;
public static class Objects
{
    public static MemObject[] MemObjects = new MemObject[]
    {
        // Each MemObject is a patch that will be applied to the game.
        // The first parameter must have a corresponding config field.
        new("CancelSwing")
        {
            Sig = "48 81 ? ? ? ? ? 48 8b ? ? ? ? ? 48 33 c4 48 89 ? ? ? 48 8b d9 80 ? ? ? 00 00 00",
            SigOffset = 0x20,
            NewBytes = "EB"
        },
        new("GuiScale")
        {
            Sig = "00 00 ? ? 00 00 A0 40 00 00 C0",
            SigOffset = 0,
            NewBytes = "00 00 40 40",
            Executable = false
        }, 
        new("AlwaysSprint")
        {
            Sig = "C7 41 ? ? ? ? ? 66 C7 ? ? ? ? C6 41 ? ? C7 41 ? ? ? ? ? 84 ? 75 ? 88 ? ? C3",
            SigOffset = 06,
            NewBytes = "01"
        },
        new("ShowPlayerNametag")
        {
            Sig = "0F 84 ? ? ? ? 49 8B ? 48 8B ? E8 ? ? ? ? 84 C0 0F 84 ? ? ? ? 49 8B",
            SigOffset = 0,
            NewBytes = "90 90 90 90 90 90"
        },
        new("ForceShowNametags")
        {
            Sig = "? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B ? ? ? ? ? 49 8B ? 48 8B ? ? ? ? ? ? ? ? ? ? ? 49",
            SigOffset = 0,
            NewBytes = "90 90 90 90 90 90"
        },
        new("ForceShowCoordinates")
        {
            Sig = "09 00 00 48 85 C0 74 ? ? ? ? ? 74",
            SigOffset = 12,
            NewBytes = "90 90"
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


// For CancelSwing, addresses from MC 1.19.70
// ORIGINAL CODE - INJECTION POINT: Minecraft.Windows.exe+72A932

Minecraft.Windows.exe+72A90D: CC                             - int 3 
Minecraft.Windows.exe+72A90E: CC                             - int 3 
Minecraft.Windows.exe+72A90F: CC                             - int 3 
Minecraft.Windows.exe+72A910: 40 53                          - push rbx
Minecraft.Windows.exe+72A912: 48 81 EC 80 00 00 00           - sub rsp,00000080
Minecraft.Windows.exe+72A919: 48 8B 05 70 3A F4 03           - mov rax,[Minecraft.Windows.exe+466E390]
Minecraft.Windows.exe+72A920: 48 33 C4                       - xor rax,rsp
Minecraft.Windows.exe+72A923: 48 89 44 24 70                 - mov [rsp+70],rax
Minecraft.Windows.exe+72A928: 48 8B D9                       - mov rbx,rcx
Minecraft.Windows.exe+72A92B: 80 B9 D0 05 00 00 00           - cmp byte ptr [rcx+000005D0],00
// ---------- INJECTING HERE ----------
Minecraft.Windows.exe+72A932: 74 18                          - je Minecraft.Windows.exe+72A94C
// ---------- DONE INJECTING  ----------
Minecraft.Windows.exe+72A934: E8 47 BC 08 02                 - call Minecraft.Windows.exe+27B6580
Minecraft.Windows.exe+72A939: 99                             - cdq 
Minecraft.Windows.exe+72A93A: 2B C2                          - sub eax,edx
Minecraft.Windows.exe+72A93C: D1 F8                          - sar eax,1
Minecraft.Windows.exe+72A93E: 8B 8B D4 05 00 00              - mov ecx,[rbx+000005D4]
Minecraft.Windows.exe+72A944: 3B C8                          - cmp ecx,eax
Minecraft.Windows.exe+72A946: 7D 04                          - jnl Minecraft.Windows.exe+72A94C
Minecraft.Windows.exe+72A948: 85 C9                          - test ecx,ecx
Minecraft.Windows.exe+72A94A: 79 11                          - jns Minecraft.Windows.exe+72A95D
Minecraft.Windows.exe+72A94C: C7 83 D4 05 00 00 FF FF FF FF  - mov [rbx+000005D4],FFFFFFFF

// For ShowPlayerNametag, addresses from MC 1.19.71
// ORIGINAL CODE - INJECTION POINT: Minecraft.Windows.exe+E89947

Minecraft.Windows.exe+E89914: 48 85 C0                          - test rax,rax
Minecraft.Windows.exe+E89917: 0F 84 65 05 00 00                 - je Minecraft.Windows.exe+E89E82
Minecraft.Windows.exe+E8991D: 4D 8B AE A0 01 00 00              - mov r13,[r14+000001A0]
Minecraft.Windows.exe+E89924: 4C 89 6D 90                       - mov [rbp-70],r13
Minecraft.Windows.exe+E89928: 49 8B 5D 00                       - mov rbx,[r13+00]
Minecraft.Windows.exe+E8992C: 49 3B DD                          - cmp rbx,r13
Minecraft.Windows.exe+E8992F: 0F 84 AD 03 00 00                 - je Minecraft.Windows.exe+E89CE2
Minecraft.Windows.exe+E89935: 66 66 66 0F 1F 84 00 00 00 00 00  - nop word ptr [rax+rax+00000000]
Minecraft.Windows.exe+E89940: 4C 8B 7B 40                       - mov r15,[rbx+40]
// ---------- INJECTING HERE ----------
Minecraft.Windows.exe+E89944: 4C 3B F8                          - cmp r15,rax
// ---------- DONE INJECTING  ----------
Minecraft.Windows.exe+E89947: 0F 84 66 03 00 00                 - je Minecraft.Windows.exe+E89CB3
Minecraft.Windows.exe+E8994D: 49 8B D7                          - mov rdx,r15
Minecraft.Windows.exe+E89950: 49 8B CE                          - mov rcx,r14
Minecraft.Windows.exe+E89953: E8 D8 FB FF FF                    - call Minecraft.Windows.exe+E89530
Minecraft.Windows.exe+E89958: 84 C0                             - test al,al
Minecraft.Windows.exe+E8995A: 0F 84 53 03 00 00                 - je Minecraft.Windows.exe+E89CB3
Minecraft.Windows.exe+E89960: 49 8B 07                          - mov rax,[r15]
Minecraft.Windows.exe+E89963: 48 8B 54 24 70                    - mov rdx,[rsp+70]
Minecraft.Windows.exe+E89968: 48 8B 92 E0 00 00 00              - mov rdx,[rdx+000000E0]
Minecraft.Windows.exe+E8996F: 49 8B CF                          - mov rcx,r15
Minecraft.Windows.exe+E89972: 48 8B 80 18 02 00 00              - mov rax,[rax+00000218]
*/

