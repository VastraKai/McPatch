using System.Diagnostics;
using System.Runtime.InteropServices;
using McPatch.Utils;
using Memory;

namespace McPatch;
public static class Mu
{
    public static Mem M { get; set; } = new();

    public static void Setup()
    {
        //MemSetup = true;
        M = new Mem();
        if (!Util.IsProcOpen("Minecraft.Windows")) Util.OpenMc();
        while (!Util.IsProcOpen("Minecraft.Windows")) { }
        _process = Util.McProcess;
        M.OpenProcess("Minecraft.Windows");
        _procHandle = Api.OpenProcess(Api.ProcessAccessFlags.All, 1, (uint)Util.McProcess!.Id);
        ScanForSig("00 00 ? ? 00 00 A0 40 00 00 C0", false, true, false, 1); // just to cache the memory
        Console.Log.WriteLine("Memory", "&aInitialized successfully.&r", LogLevel.Success);
    }

    private static Process? _process;
    public static void Dispose()
    {
        try
        {
            //MemSetup = false;
            Util.McProcess?.Kill();
            Mu.M.CloseProcess();
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        }
        catch { }
    }
    private static Dictionary<string, byte[]> _memoryCache = new();
    public static IEnumerable<long> ScanForSig(string sig, bool executable, bool readable, bool writable, int resultLimit = 0, int bytesPerTask = 20000000, string module = "Minecraft.Windows.exe")
    {
        try
        {
            // Get the start and end address of the module
            _process?.Refresh();
            sig = sig.Replace('*', '?');
            sig = sig.Trim();
            while (sig.EndsWith(" ?") || sig.EndsWith(" ??"))
            {
                if (sig.EndsWith(" ??")) sig = sig.Substring(0, sig.Length - 3);
                if (sig.EndsWith(" ?")) sig = sig.Substring(0, sig.Length - 2);
            }


            long startAddress = 0;
            long endAddress = 0x7fffffffffff;
            if (module == "Minecraft.Windows.exe")
            {
                startAddress = _process!.MainModule!.BaseAddress.ToInt64();
                endAddress = startAddress + _process.MainModule!.ModuleMemorySize;
            }
            else
            {
                foreach (ProcessModule module1 in _process!.Modules)
                {
                    if (Path.GetFileName(module1.FileName) != module) continue;
                    startAddress = module1.BaseAddress.ToInt64();
                    endAddress = startAddress + module1.ModuleMemorySize;
                }
            }

            // Parse the signature
            byte[] sigBytes = ParseSig(sig, out byte[] maskBytes);
            List<long> addresses = new();

            byte[] buffer;
            if (_memoryCache.All(x => x.Key != module))
            {
                buffer = new byte[endAddress - startAddress];
                Api.ReadProcessMemory(_procHandle, startAddress, buffer, buffer.Length, out _);
                _memoryCache.Add(module, buffer);
            }

            buffer = _memoryCache[module];

            List<Task> tasks = new();
            nint foundCountPtr = Marshal.AllocHGlobal(16);
            Marshal.WriteInt32(foundCountPtr, 0);
            for (int i = 0; i < buffer.Length; i += bytesPerTask)
            {
                int i1 = i;
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < bytesPerTask; j++)
                    {
                        if (i1 + j >= buffer.Length) break;
                        bool found = true;
                        for (int k = 0; k < sigBytes.Length; k++)
                        {
                            if ((buffer[i1 + j + k] & maskBytes[k]) != (sigBytes[k] & maskBytes[k]))
                            {
                                found = false;
                                break;
                            }
                        }

                        if (resultLimit != 0 && Marshal.ReadInt32(foundCountPtr) >= resultLimit) return;
                        if (!found) continue;
                        // increment the found count and verify the value
                        int c = Marshal.ReadInt32(foundCountPtr);
                        if (resultLimit != 0 && c >= resultLimit) break;
                        Marshal.WriteInt32(foundCountPtr, c + 1);
                        addresses.Add(startAddress + i1 + j);
                    }
                }));
            }

            while (tasks.Any(x => !x.IsCompleted))
            {
                Thread.Sleep(1);
            }
            Marshal.FreeHGlobal(foundCountPtr);
            if (!readable && !writable && !executable) return addresses;
            List<long> addressesToRemove = new();
            foreach (long address in addresses)
            {
                Imps.MEMORY_BASIC_INFORMATION mbi = new();
                Api.VirtualQueryEx(_procHandle, address, out mbi, (uint)Marshal.SizeOf(mbi));
                if (readable && (mbi.Protect & Api.PageReadonly) == 0 &&
                    (mbi.Protect & Api.PageReadwrite) == 0 &&
                    (mbi.Protect & Api.PageWritecopy) == 0 &&
                    (mbi.Protect & Api.PageExecuteRead) == 0 &&
                    (mbi.Protect & Api.PageExecuteReadwrite) == 0 &&
                    (mbi.Protect & Api.PageExecuteWritecopy) == 0)
                {
                    addressesToRemove.Add(address);
                    continue;
                }

                if (writable && (mbi.Protect & Api.PageReadwrite) == 0 &&
                    (mbi.Protect & Api.PageWritecopy) == 0 &&
                    (mbi.Protect & Api.PageExecuteReadwrite) == 0 &&
                    (mbi.Protect & Api.PageExecuteWritecopy) == 0)
                {
                    addressesToRemove.Add(address);
                    continue;
                }

                if (executable && (mbi.Protect & Api.PageExecute) == 0 &&
                    (mbi.Protect & Api.PageExecuteRead) == 0 &&
                    (mbi.Protect & Api.PageExecuteReadwrite) == 0 &&
                    (mbi.Protect & Api.PageExecuteWritecopy) == 0)
                {
                    addressesToRemove.Add(address);
                    continue;
                }
            }

            // foreach (long address in addressesToRemove) addresses.Remove(address);
            return addresses;
        }
        catch (Exception e)
        {
            Console.Log.WriteLine("Memory", $"&cError while scanning for &e{sig}&c in &e{module}&c: &e{e}&r",
                LogLevel.Error);
            return new List<long>();
        }
    }
    
    private static byte[] ParseSig(string sig, out byte[] mask)
    {
        string[] stringByteArray = sig.Split(' ');

        byte[] sigPattern = new byte[stringByteArray.Length];
        mask = new byte[stringByteArray.Length];

        for (int i = 0; i < stringByteArray.Length; i++)
        {
            string ba = stringByteArray[i];

            if (ba == "??" || (ba.Length == 1 && ba == "?"))
            {
                mask[i] = 0x00;
                stringByteArray[i] = "0x00";
            }
            else if (char.IsLetterOrDigit(ba[0]) && ba[1] == '?')
            {
                mask[i] = 0xF0;
                stringByteArray[i] = ba[0] + "0";
            }
            else if (char.IsLetterOrDigit(ba[1]) && ba[0] == '?')
            {
                mask[i] = 0x0F;
                stringByteArray[i] = "0" + ba[1];
            }
            else
                mask[i] = 0xFF;
        }

        for (int i = 0; i < stringByteArray.Length; i++)
            sigPattern[i] = (byte)(Convert.ToByte(stringByteArray[i], 16) & mask[i]);
        return sigPattern;
    }
    private static IntPtr _procHandle = IntPtr.Zero;
    // Private class (with the name Api) for memory-related functions in kernel32.dll including but not limited to ReadProcessMemory, VirtualQueryEx, etc.
    private static class Api
    {
        public const uint PageNoaccess = 0x01;
        public const uint PageReadonly = 0x02;
        public const uint PageReadwrite = 0x04;
        public const uint PageWritecopy = 0x08;
        public const uint PageExecute = 0x10;
        public const uint PageExecuteRead = 0x20;
        public const uint PageExecuteReadwrite = 0x40;
        public const uint PageExecuteWritecopy = 0x80;
        public const uint PageGuard = 0x100;
        public const uint PageNocache = 0x200;
        public const uint PageWritecombine = 0x400;


        public static class ProcessAccessFlags
        {
            public const uint ProcessVmOperation = 0x0008;
            public const uint ProcessVmRead = 0x0010;
            public const uint ProcessVmWrite = 0x0020;
            public const uint ProcessQueryInformation = 0x0400;
            public const uint All = ProcessVmOperation | ProcessVmRead | ProcessVmWrite | ProcessQueryInformation;
        }

        // Abort thread
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int TerminateThread(IntPtr hThread, uint dwExitCode);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize,
            uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size,
            int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] buffer, int size,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(IntPtr hProcess, long lpAddress,
            out Imps.MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualProtectEx(IntPtr hProcess, long lpAddress, int dwSize, uint flNewProtect,
            out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualFreeEx(IntPtr hProcess, long lpAddress, int dwSize, uint dwFreeType);
    }
}
