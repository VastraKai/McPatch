using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace McPatch;
public class MemObject
{
    // Properties for the object
    public long Address = 0;
    public string PropertyName = string.Empty;
    public string Sig = string.Empty;
    public string NewBytes = string.Empty;
    public int SigOffset = 0;
    public bool Executable = true;
    public bool Readable = true;
    public bool Writable = false;
    private bool Scanned = false;
    // Constructors for the object
    public MemObject(string propertyName, string sig, int sigOffset, string newBytes, bool executable = true, bool readable = true, bool writable = false)
    {
        Executable = executable;
        Readable = readable;
        Writable = writable;
        Sig = sig;
        SigOffset = sigOffset;
        NewBytes = newBytes;
        PropertyName = propertyName;
    }
    // Methods for the object
    public bool ApplyPatch()
    {
        if (!Scanned) Scan();
        if (Address == 0)
            return false;
        bool written = M.Mem.WriteMemory(Address.ToString("X"), "bytes", NewBytes);
        if (!written) Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
        return written;
    }
    public bool ApplyPatchIf()
    {
        if (!Scanned) Scan();
        if (Address == 0)
            return false;
        // Get the corresponding config field
        FieldInfo? ConfigField = Config.CurrentConfig.GetType().GetField(PropertyName);
        // Check if the config field is null
        if (ConfigField == null)
        {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Unable to apply patch: MemObject {Console.Value($"'{PropertyName}'")} does not have a corresponding value in Config.CurrentConfig");
            return false;
        }
        // Get the ConfigField's Value
        dynamic? FieldValue = ConfigField.GetValue(Config.CurrentConfig);
        if (FieldValue == null)
        {
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} The corresponding value for MemObject {Console.Value($"'{PropertyName}'")} is unassigned");
            return false;
        }
        // Check if the ConfigField is of type bool
        if (ConfigField.FieldType == typeof(bool))
        {

            // If it is, check if it's true
            if ((bool)FieldValue)
            {
                // If it is, apply the patch
                bool written = M.Mem.WriteMemory(Address.ToString("X"), "bytes", NewBytes);
                if (!written) Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
                Console.WriteLine($"{Console.Prefix("Patcher")} {Console.Value($"{PropertyName}")} has been enabled");
                return written;
            }
            else
                return false;  // If it's not, don't apply the patch
        }
        else // If it's not a bool, apply the patch using the value as the new bytes
        {
            
            string type = ConfigField.FieldType.ToString();

            switch (type)
            {
                case "System.Int32":
                    type = "int";
                    break;
                case "System.Int64":
                    type = "long";
                    break;
                case "System.Single":
                    type = "float";
                    break;
                case "System.Double":
                    type = "double";
                    break;
                case "System.String":
                    type = "string";
                    break;
                case "System.Byte":
                    type = "byte";
                    break;
                default:
                    Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Property {Console.Value($"'{PropertyName}'")} has an unsupported type");
                    return false;
            }
            bool written = M.Mem.WriteMemory(Address.ToString("X"), type, $"{FieldValue}");
            if (!written) Debug.WriteLine($"Memory not written, Address: '{Address:X}', sig: '{Sig}'(+{SigOffset})");
            Console.WriteLine($"{Console.Prefix("Patcher")} {Console.Value($"{PropertyName}")} has been set to {Console.Value($"'{FieldValue}'")}");
            return written;
        }
    }

    public void Scan()
    {
        Address = M.Mem.GetAddressFromSig(Sig, SigOffset, Executable, Readable, Writable);
        if (Address.ToString("X").StartsWith("7F"))
            Console.WriteLine($"{Console.Prefix("Patcher Debug")} Field Info for {Console.Value($"'{PropertyName}'")}: {Console.Value($"'{Address:X}'")}:{Console.Value($"'{M.Mem.ReadBytes(Address.ToString("X"), 11).ToHexString()}'")} ");
        else
            Console.WriteLine($"{Console.ErrorPrefix("Patcher")} Address for {Console.Value($"'{PropertyName}'")} not found, sig: {Console.Value($"'{Sig}'")} {Console.ErrorTextColor}(Please report this!){Console.R}");
        Scanned = true;
    }
}

