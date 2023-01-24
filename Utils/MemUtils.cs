using Memory;

namespace McPatch;
public static class MemUtils
{
    /// <summary>
    /// Returns the address of the first occurrence of a given signature.
    /// The start & end paramaters will use McMem's start and end addresses by default.
    /// </summary>
    /// <param name="sig">The signature to scan for.</param>
    /// <param name="shift">The amount in bytes to add on to the returned address.</param>
    /// <param name="executable">If the target address area is executable</param>
    /// <param name="readable">If the target address area is readable</param>
    /// <param name="writable">If the target address area is writable</param>
    /// <param name="start">Start of the module in memory</param>
    /// <param name="end">End of the module in memory</param>
    /// <returns>Address of signature</returns>
    public static long GetAddressFromSig(this Mem M, string sig, long shift, bool executable = true, bool readable = true, bool writable = false, long start = 0, long end = 0)
    {
        if (start == 0) start = Util.McMem.beginning;
        if (end == 0) end = Util.McMem.end;
        long? SigAddr = M.AoBScan(start, end, sig, true, false, true).Result.FirstOrDefault();
        if (SigAddr == null)
            throw new Exception($"The sig '{sig}' wasn't found!");
        return (long)(SigAddr + shift);
    }
}
