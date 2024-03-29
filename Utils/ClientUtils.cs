﻿using System.Diagnostics;

namespace McPatch.Utils;
public static class ClientUtils
{
    private static readonly string orig1 = "<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\">";
    private static readonly string rep1 = "<Application Id=\"App\" Executable=\"Minecraft.Windows.exe\" EntryPoint=\"Minecraft_Win10.App\" desktop4:SupportsMultipleInstances=\"true\">";
    private static readonly string orig2 = "<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/2014/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\">";
    private static readonly string rep2 = "<Package xmlns=\"http://schemas.microsoft.com/appx/manifest/foundation/windows10\" xmlns:mp=\"http://schemas.microsoft.com/appx/201/phone/manifest\" xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\" xmlns:uap5=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/5\" xmlns:uap4=\"http://schemas.microsoft.com/appx/manifest/uap/windows10/4\" IgnorableNamespaces=\"uap uap4 uap5 mp build\" xmlns:build=\"http://schemas.microsoft.com/developer/appx/2015/build\" xmlns:desktop4=\"http://schemas.microsoft.com/appx/manifest/desktop/windows10/4\">";
    public static bool EnableMultiInstance(string McPath)
    {
        string oldContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        string newContent = oldContent.Replace(orig1, rep1);
        newContent = newContent.Replace(orig2, rep2);
        File.WriteAllText(McPath + "\\AppxManifest.xml", newContent);
        if (oldContent == newContent) return false;
        else return true;
    }
    public static bool DisableMultiInstance(string McPath)
    {
        string oldContent = File.ReadAllText(McPath + "\\AppxManifest.xml");
        string newContent = oldContent.Replace(rep1, orig1);
        newContent = newContent.Replace(rep2, orig2);
        File.WriteAllText(McPath + "\\AppxManifest.xml", newContent);
        if (oldContent == newContent) return false;
        else return true;
    }
}
