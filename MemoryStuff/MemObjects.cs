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
            Sig = "00 00 ? ? 00 00 A0 40 00 00 C0 40",
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
            // if you have ShowPlayerNametag in your client, you should update it to use this sig
            Sig = "? ? ? ? ? ? 49 8B ? 48 8B ? E8 ? ? ? ? 84 C0 ? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B",
            SigOffset = 0,
            NewBytes = "90 90 90 90 90 90"
        },
        new("ForceShowNametags")
        {
            Sig = "? ? ? ? ? ? 49 8B ? 48 8B ? E8 ? ? ? ? 84 C0 ? ? ? ? ? ? 49 8B ? 48 8B ? ? ? 48 8B",
            SigOffset = 0x13,
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
/* // For AlwaysSprint, addresses from MC 1.20.12
// ORIGINAL CODE - INJECTION POINT: Minecraft.Windows.exe+1DF3923

Minecraft.Windows.exe+1DF38F4: C7 41 04 00 00 00 00  - mov [rcx+04],00000000
Minecraft.Windows.exe+1DF38FB: 66 C7 41 0F 00 00     - mov word ptr [rcx+0F],0000
Minecraft.Windows.exe+1DF3901: C6 41 0E 00           - mov byte ptr [rcx+0E],00
Minecraft.Windows.exe+1DF3905: C7 41 1C 00 00 00 00  - mov [rcx+1C],00000000
Minecraft.Windows.exe+1DF390C: 84 C0                 - test al,al
Minecraft.Windows.exe+1DF390E: 75 02                 - jne Minecraft.Windows.exe+1DF3912
Minecraft.Windows.exe+1DF3910: 88 01                 - mov [rcx],al
Minecraft.Windows.exe+1DF3912: 66 C7 41 7F 00 00     - mov word ptr [rcx+7F],0000
Minecraft.Windows.exe+1DF3918: C6 41 7E 00           - mov byte ptr [rcx+7E],00
Minecraft.Windows.exe+1DF391C: C7 41 2A 00 00 00 00  - mov [rcx+2A],00000000
// ---------- INJECTING HERE ----------
Minecraft.Windows.exe+1DF3923: C7 41 24 00 00 00 00  - mov [rcx+24],00000000
// ---------- DONE INJECTING  ----------
Minecraft.Windows.exe+1DF392A: 66 C7 41 2F 00 00     - mov word ptr [rcx+2F],0000
Minecraft.Windows.exe+1DF3930: C6 41 2E 00           - mov byte ptr [rcx+2E],00
Minecraft.Windows.exe+1DF3934: C7 41 3C 00 00 00 00  - mov [rcx+3C],00000000
Minecraft.Windows.exe+1DF393B: 84 C0                 - test al,al
Minecraft.Windows.exe+1DF393D: 75 03                 - jne Minecraft.Windows.exe+1DF3942
Minecraft.Windows.exe+1DF393F: 88 41 20              - mov [rcx+20],al
Minecraft.Windows.exe+1DF3942: C3                    - ret 
Minecraft.Windows.exe+1DF3943: CC                    - int 3 
Minecraft.Windows.exe+1DF3944: CC                    - int 3 
Minecraft.Windows.exe+1DF3945: CC                    - int 3 


// For CancelSwing, addresses from MC 1.20.12
// ORIGINAL CODE - INJECTION POINT: Minecraft.Windows.exe+7A1B72

Minecraft.Windows.exe+7A1B4D: CC                             - int 3 
Minecraft.Windows.exe+7A1B4E: CC                             - int 3 
Minecraft.Windows.exe+7A1B4F: CC                             - int 3 
Minecraft.Windows.exe+7A1B50: 40 53                          - push rbx
Minecraft.Windows.exe+7A1B52: 48 81 EC 80 00 00 00           - sub rsp,00000080
Minecraft.Windows.exe+7A1B59: 48 8B 05 90 25 5C 04           - mov rax,[Minecraft.Windows.exe+4D640F0]
Minecraft.Windows.exe+7A1B60: 48 33 C4                       - xor rax,rsp
Minecraft.Windows.exe+7A1B63: 48 89 44 24 70                 - mov [rsp+70],rax
Minecraft.Windows.exe+7A1B68: 48 8B D9                       - mov rbx,rcx
Minecraft.Windows.exe+7A1B6B: 80 B9 E8 05 00 00 00           - cmp byte ptr [rcx+000005E8],00
// ---------- INJECTING HERE ----------
Minecraft.Windows.exe+7A1B72: 74 18                          - je Minecraft.Windows.exe+7A1B8C
// ---------- DONE INJECTING  ----------
Minecraft.Windows.exe+7A1B74: E8 C7 A5 55 02                 - call Minecraft.Windows.exe+2CFC140
Minecraft.Windows.exe+7A1B79: 99                             - cdq 
Minecraft.Windows.exe+7A1B7A: 2B C2                          - sub eax,edx
Minecraft.Windows.exe+7A1B7C: D1 F8                          - sar eax,1
Minecraft.Windows.exe+7A1B7E: 8B 8B EC 05 00 00              - mov ecx,[rbx+000005EC]
Minecraft.Windows.exe+7A1B84: 3B C8                          - cmp ecx,eax
Minecraft.Windows.exe+7A1B86: 7D 04                          - jnl Minecraft.Windows.exe+7A1B8C
Minecraft.Windows.exe+7A1B88: 85 C9                          - test ecx,ecx
Minecraft.Windows.exe+7A1B8A: 79 11                          - jns Minecraft.Windows.exe+7A1B9D
Minecraft.Windows.exe+7A1B8C: C7 83 EC 05 00 00 FF FF FF FF  - mov [rbx+000005EC],FFFFFFFF

// For ShowPlayerNametag, addresses from MC 1.20.12
// ORIGINAL CODE - INJECTION POINT: Minecraft.Windows.exe+F029A7

Minecraft.Windows.exe+F0297A: 4D 85 F6              - test r14,r14
Minecraft.Windows.exe+F0297D: 0F 84 B0 05 00 00     - je Minecraft.Windows.exe+F02F33
Minecraft.Windows.exe+F02983: 4C 8B AE A0 01 00 00  - mov r13,[rsi+000001A0]
Minecraft.Windows.exe+F0298A: 4C 89 6D 98           - mov [rbp-68],r13
Minecraft.Windows.exe+F0298E: 49 8B 5D 00           - mov rbx,[r13+00]
Minecraft.Windows.exe+F02992: 49 3B DD              - cmp rbx,r13
Minecraft.Windows.exe+F02995: 0F 84 E5 03 00 00     - je Minecraft.Windows.exe+F02D80
Minecraft.Windows.exe+F0299B: 0F 1F 44 00 00        - nop dword ptr [rax+rax+00]
Minecraft.Windows.exe+F029A0: 4C 8B 7B 40           - mov r15,[rbx+40]
Minecraft.Windows.exe+F029A4: 4D 3B FE              - cmp r15,r14
// ---------- INJECTING HERE ----------
Minecraft.Windows.exe+F029A7: 0F 84 BF 03 00 00     - je Minecraft.Windows.exe+F02D6C
// ---------- DONE INJECTING  ----------
Minecraft.Windows.exe+F029AD: 49 8B D7              - mov rdx,r15
Minecraft.Windows.exe+F029B0: 48 8B CE              - mov rcx,rsi
Minecraft.Windows.exe+F029B3: E8 E8 FB FF FF        - call Minecraft.Windows.exe+F025A0
Minecraft.Windows.exe+F029B8: 84 C0                 - test al,al
Minecraft.Windows.exe+F029BA: 0F 84 AC 03 00 00     - je Minecraft.Windows.exe+F02D6C
Minecraft.Windows.exe+F029C0: 49 8B 07              - mov rax,[r15]
Minecraft.Windows.exe+F029C3: 48 8B 54 24 70        - mov rdx,[rsp+70]
Minecraft.Windows.exe+F029C8: 48 8B 92 E0 00 00 00  - mov rdx,[rdx+000000E0]
Minecraft.Windows.exe+F029CF: 49 8B CF              - mov rcx,r15
Minecraft.Windows.exe+F029D2: 48 8B 80 E0 01 00 00  - mov rax,[rax+000001E0]
*/

