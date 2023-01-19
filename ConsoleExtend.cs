using System.Diagnostics;
using System.Runtime.InteropServices;

namespace McPatch;
// Extended from ConsoleBase
public static partial class Console
{
    internal class WindowsConsoleConfig
    {
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public void SetupConsole()
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleMode(handle, out uint mode);
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(handle, mode);
        }
    }
    /// <summary>
    /// Use this to setup the console for escape codes
    /// </summary>
    public static void SetupConsole() // Use to setup the console and enable escape codes
    {
        if (Environment.OSVersion.Platform.ToString().ToLower().Contains("win"))
        {
            WindowsConsoleConfig ConConfig = new();
            ConConfig.SetupConsole();
        }
    }
    // Hide the console window
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();
    const int SW_HIDE = 0;
    const int SW_SHOW = 5;
    public static void HideConsole()
    {
        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);
    }
    public static void ShowConsole()
    {
        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, SW_SHOW);
    }
    // could be used by other things too
    public static string ESC = "\x001B"; // Escape character
    /// <summary>
    /// escColor: Gets the escape code to set a foreground color, or reset the foreground color.
    /// </summary>
    /// <param name="r">The value to be used for Red</param>
    /// <param name="g">The value to be used for Green</param>
    /// <param name="b">The value to be used for Blue</param>
    /// <returns>Foreground color escape code as string</returns>
    public static string EscColor(float r, float g, float b) // returns a foreground color escape code 
    {                                                                            // (changes the foreground color based on red, green, and blue values)
        int red = (int)Math.Round((float)(255 * r));
        int green = (int)Math.Round((float)(255 * g));
        int blue = (int)Math.Round((float)(255 * b));
        return $"{ESC}[38;2;{red};{green};{blue}m";
    }
    public static string EscColor(string red, string green, string blue) => $"{ESC}[38;2;{red};{green};{blue}m";
    public static string Cl(float r, float g, float b) => EscColor(r, g, b);
    //public static void WriteLineF(string prefixText, string text)
    //{
    //    Console.WriteLine($"{PrefixColor}[{prefixText}]{GenericTextColor} {text}{R}");
    //}
    //public static void WriteErrorF(string prefixText, string text, string textColorStr = "default")
    //{
    //    if (textColorStr == "default") textColorStr = WarningTextColor;
    //    Console.Error.WriteLine($"{ErrorTextColor}[{prefixText}]{textColorStr} {text}{R}");
    //}
    //public static void WriteWarningF(string prefixText, string text, string textColorStr = "default")
    //{
    //    if (textColorStr == "default") textColorStr = WarningTextColor;
    //    Console.Error.WriteLine($"{WarningTextColor}[{prefixText}]{textColorStr} {text}{R}");
    //}

    /// <summary>
    /// Uses cmd's cls instead of the default Console.Clear
    /// </summary>
    public static void ClearAlt()
    {
        Process.Start("cmd.exe", "/c cls").WaitForExit();
    }
    public static void SwitchToAlternativeBuffer()
    {
        Write($"{ESC}[?1049h");
        //CursorLeft -= 8;
        //Write("        ");
        //CursorLeft -= 8;
    }
    public static void SwitchToMainBuffer()
    {
        Write($"{ESC}[?1049l");
        //CursorLeft -= 8;
        //Write("        ");
        //CursorLeft -= 8;
    }
    public static string WarningPrefix(string prefixText) => $"{PrefixColor}[{prefixText} {WarningTextColor}Warning{PrefixColor}]{R}";
    public static string ErrorPrefix(string prefixText) => $"{PrefixColor}[{prefixText} {ErrorTextColor}Error{PrefixColor}]{R}";
    public static string Prefix(string prefixText) => $"{PrefixColor}[{prefixText}]{R}";

    public static string R = "\x001B[0m";
    public static string PrefixColor = Console.EscColor(0.0f, 1.0f, 1.0f);
    public static string WarningTextColor = Console.EscColor(1.0f, 1.0f, 0.0f);
    public static string ErrorTextColor = Console.EscColor(1.0f, 0.0f, 0.0f);
    public static string ValueColor = Console.EscColor(0.0f, 0.53333333333f, 1.0f);
    public static string GreenTextColor = Console.EscColor(0.0f, 1.0f, 0.13333333333f);
    public static string BlueTextColor = Console.EscColor(0.0f, 0.13333333333f, 1.0f);
    public static string GenericTextColor = Console.R;
}
