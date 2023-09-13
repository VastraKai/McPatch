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
using McPatch.ConsoleMenu;
using McPatch.Utils;
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
    public string CurrentBytes => Mu.M.ReadBytes(Address.ToString("X"), ByteReadLength).ToHexStringO();

    /// <summary>
    /// If the memory object has already been scanned for.
    /// </summary>
    public bool Scanned { get; private set; } = false;

    public dynamic? ConfigValue = null;
    public Type? ConfigType = null;

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
        if (ConfigValue != null && ConfigType == typeof(bool) && (bool)ConfigValue! && OriginalBytes == CurrentBytes)
            Console.Log.WriteLine("Patcher", $"(in &v{PropertyName}&r) OgBytes are the same as CurrentBytes!", LogLevel.Warning);
        else if (OriginalBytes == CurrentBytes)
            Console.Log.WriteLine("Patcher", $"No action was performed for &v{PropertyName}&r", LogLevel.Warning);
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
        MenuItem? MenuItem = Program.ConfigMenu.GetMenuItem(PropertyName);
        // Check if the config field is null
        if (MenuItem == null)
        {
            Console.Log.WriteLine("Patcher", $"Unable to apply patch: MemObject &v'{PropertyName}'&r does not have a corresponding value in Config.CurrentConfig", LogLevel.Error);
            return false;
        }

        // Get the ConfigField's Value

        string type = "";

        if (MenuItem is BoolMenuItem boolMenuItem)
        {
            ConfigValue = boolMenuItem.Value;
            ConfigType = typeof(bool);
            type = "byte";
        }
        else if (MenuItem is IntMenuItem intMenuItem)
        {
            ConfigValue = intMenuItem.Value;
            ConfigType = typeof(int);
            type = "int";
        }
        else if (MenuItem is FloatMenuItem floatMenuItem)
        {
            ConfigValue = floatMenuItem.Value;
            ConfigType = typeof(float);
            type = "float";
        }
        else
        {
            // Unsupported type
            Console.Log.WriteLine("Patcher", $"Unable to apply patch: MemObject &v'{PropertyName}'&r has an unsupported type", LogLevel.Error);
            return false;
        }
        
        if (ConfigValue == null)
        {
            Console.Log.WriteLine("Patcher", $"Unable to apply patch: The correspondig value for MemObject &v'{PropertyName}'&r is unassigned", LogLevel.Error);
            return false;
        }

         
        // Check if the ConfigField is of type bool
        if (ConfigType == typeof(bool))
        {
            // If it is, check if it's true
            if ((bool)ConfigValue)
            {
                // If it is, apply the patch
                bool written = Mu.M.WriteMemory(Address.ToString("X"), "bytes", NewBytes);
                if (!written)
                    Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
                Console.Log.WriteLine("Patcher", $"&v{PropertyName}&r has been enabled");
                return written;
            }
            return true; // nothing went wrong so we still need to return true
        }
        else // If it's not a bool, apply the patch using the value as the new bytes
        { // Convert the type string to a type that Mem. can use
            bool written = Mu.M.WriteMemory(Address.ToString("X"), type, $"{ConfigValue}");
            if (!written) Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
            Console.Log.WriteLine("Patcher", $"&v{PropertyName}&r has been set to &v'{ConfigValue}'&r");
            return written;
        }
    }

    /// <summary>
    /// Scans for the address of the sig and writes it to the Address property.
    /// </summary>
    public void Scan()
    {
        //Address = Mu.M.GetAddressFromSig(Sig, SigOffset, Executable, Readable, Writable); // Original
        Address = Mu.ScanForSig(Sig, Executable, Readable, Writable, 1).FirstOrDefault();
        Address += SigOffset;
        if (Address.IsValidStaticAddress())
        {
            Console.Log.WriteLine("Patcher", $"Object info for &v'{PropertyName}'&r: &v'{Address:X}'&r:&v'{Mu.M.ReadBytes(Address.ToString("X"), 11).ToHexString()}'&r");
            OriginalBytes = Mu.M.ReadBytes(Address.ToString("X"), ByteReadLength).ToHexStringO();
        }
        else
        {
            Console.Log.WriteLine("Patcher", $"Address for &v'{PropertyName}'&r not found, sig: &v'{Sig}'&r &c(Please report this!)&r", LogLevel.Error);
        }

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
        bool written = Mu.M.WriteMemory(Address.ToString("X"), "bytes", NewBytes);
        if (!written) Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
        return written;
    }

    #endregion
}