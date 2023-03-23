using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace McPatch;

public class MemObject
{
    #region Constructors

    /// <summary>
    /// Creates a new MemObject to patch memory.
    /// </summary>
    /// <param name="propertyName">The property name of this MemObject. This must have a corresponding value in the config.</param>
    /// <param name="sig">The signature to scan for.</param>
    /// <param name="sigOffset">The amount to add to the result address.</param>
    /// <param name="newBytes">The bytes to write at the result address.</param>
    /// <param name="executable">If the memory at the result address should be executable.</param>
    /// <param name="readable">If the memory at the result address should be readable.</param>
    /// <param name="writable">If the memory at the result address should be writable.</param>
    public MemObject(string propertyName, string sig, int sigOffset, string newBytes, bool executable = true,
        bool readable = true, bool writable = false)
    {
        PropertyName = propertyName;
        Sig = sig;
        SigOffset = sigOffset;
        NewBytes = newBytes;
        Executable = executable;
        Readable = readable;
        Writable = writable;
    }

    /// <summary>
    /// Creates a new MemObject to patch memory.
    /// </summary>
    /// <param name="propertyName">The property name of this MemObject.</param>
    public MemObject(string propertyName)
    {
        PropertyName = propertyName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The property name of this MemObject.
    /// </summary>
    public string PropertyName = string.Empty;

    /// <summary>
    /// The address of the memory to patch.
    /// </summary>
    public long Address { get; private set; } = 0;

    /// <summary>
    /// The signature to scan for.
    /// </summary>
    public string Sig = string.Empty;

    /// <summary>
    /// The amount to add to the result address.
    /// </summary>
    public int SigOffset = 0;

    /// <summary>
    /// The bytes to write at the result address.
    /// </summary>
    public string NewBytes = string.Empty;

    /// <summary>
    /// If the memory at the result address should be executable.
    /// </summary>
    public bool Executable = true;

    /// <summary>
    /// If the memory at the result address should be readable.
    /// </summary>
    public bool Readable = true;

    /// <summary>
    /// If the memory at the result address should be writable.
    /// </summary>
    public bool Writable = false;

    /// <summary>
    /// Sets how much bytes should be read into OriginalBytes and CurrentBytes.
    /// </summary>
    public long ByteReadLength = 12;

    /// <summary>
    /// The original bytes of the result address, before patching.
    /// </summary>
    public string OriginalBytes { get; private set; } = string.Empty;

    /// <summary>
    /// The current bytes of the result address.
    /// </summary>
    public string CurrentBytes => M.Mem.ReadBytes(Address.ToString("X"), ByteReadLength).ToHexStringO();

    /// <summary>
    /// If the memory object has already been scanned for.
    /// </summary>
    public bool Scanned { get; private set; } = false;

    private dynamic? ConfigValue = null;
    private Type? ConfigType = null;

    #endregion

    #region Methods

    public bool PatchApply(in string hexIn, out string hexOut)
    {
        hexOut = hexIn;
        if (!Scanned) Scan();
        if (!Address.IsValidStaticAddress())
            return false;
        bool success = ApplyIf();
        hexOut = hexOut.Replace(OriginalBytes, CurrentBytes);
        if (ConfigValue != null && ConfigType == typeof(bool) && (bool)ConfigValue && OriginalBytes == CurrentBytes)
            Console.WriteLine(
                $"{Console.WarningPrefix("Patcher")} (in {Console.Value($"{PropertyName}")}) OgBytes are the same as CurrentBytes!");
        else if (OriginalBytes == CurrentBytes)
            Console.WriteLine(
                $"{Console.WarningPrefix("Patcher")} No action was performed for {Console.Value($"{PropertyName}")}");
        return success;
    }


    /// <summary>
    /// Patches the memory, according to config.
    /// </summary>
    /// <returns>If the patch was successful.</returns>
    public bool ApplyIf()
    {
        if (!Address.IsValidStaticAddress())
            return false;
        // Get the corresponding config field
        FieldInfo? ConfigField = Config.CurrentConfig.GetType().GetField(PropertyName);
        // Check if the config field is null
        if (ConfigField == null)
        {
            Console.WriteLine(
                $"{Console.ErrorPrefix("Patcher")} Unable to apply patch: MemObject {Console.Value($"'{PropertyName}'")} does not have a corresponding value in Config.CurrentConfig");
            return false;
        }

        // Get the ConfigField's Value
        dynamic? FieldValue = ConfigField.GetValue(Config.CurrentConfig);
        if (FieldValue == null)
        {
            Console.WriteLine(
                $"{Console.ErrorPrefix("Patcher")} The corresponding value for MemObject {Console.Value($"'{PropertyName}'")} is unassigned");
            return false;
        }

        ConfigValue = FieldValue;
        ConfigType = ConfigField.FieldType;
        // Check if the ConfigField is of type bool
        if (ConfigField.FieldType == typeof(bool))
        {
            // If it is, check if it's true
            if ((bool)FieldValue)
            {
                // If it is, apply the patch
                bool written = M.Mem.WriteMemory(Address.ToString("X"), "bytes", NewBytes);
                if (!written)
                    Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
                Console.WriteLine($"{Console.Prefix("Patcher")} {Console.Value($"{PropertyName}")} has been enabled");
                return written;
            }
            else
                return true; // nothing went wrong so we still need to return true
        }
        else // If it's not a bool, apply the patch using the value as the new bytes
        {
            string type = ConfigField.ToMemTypeString(this); // Convert the type string to a type that Mem. can use
            bool written = M.Mem.WriteMemory(Address.ToString("X"), type, $"{FieldValue}");
            if (!written) Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
            Console.WriteLine(
                $"{Console.Prefix("Patcher")} {Console.Value($"{PropertyName}")} has been set to {Console.Value($"'{FieldValue}'")}");
            return written;
        }
    }

    /// <summary>
    /// Scans for the address of the sig and writes it to the Address property.
    /// </summary>
    public void Scan()
    {
        Address = M.Mem.GetAddressFromSig(Sig, SigOffset, Executable, Readable, Writable);
        if (Address.IsValidStaticAddress())
        {
            Console.WriteLine(
                $"{Console.Prefix("Patcher Debug")} Object info for {Console.Value($"'{PropertyName}'")}: {Console.Value($"'{Address:X}'")}:{Console.Value($"'{M.Mem.ReadBytes(Address.ToString("X"), 11).ToHexString()}'")} ");
            OriginalBytes = M.Mem.ReadBytes(Address.ToString("X"), ByteReadLength).ToHexStringO();
        }
        else
            Console.WriteLine(
                $"{Console.ErrorPrefix("Patcher")} Address for {Console.Value($"'{PropertyName}'")} not found, sig: {Console.Value($"'{Sig}'")} {Console.ErrorTextColor}(Please report this!){Console.R}");

        Scanned = true;
    }

    /// <summary>
    /// Patches the memory.
    /// </summary>
    /// <returns>If the patch was successful.</returns>
    public bool Apply()
    {
        if (!Scanned) Scan();
        if (!Address.IsValidStaticAddress())
            return false;
        bool written = M.Mem.WriteMemory(Address.ToString("X"), "bytes", NewBytes);
        if (!written) Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
        return written;
    }

    #endregion
}