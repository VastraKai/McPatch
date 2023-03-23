using System.Diagnostics;

namespace CustomConsole;

// Extended from ConsoleBase
public static partial class Console
{
    public static Dictionary<string, string> ColorShortcuts = new();
    /// <summary>
    /// Registers a new shortcut for a color.
    /// </summary>
    /// <param name="shortcut">The shortcut text to replace with the escape code string.</param>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    public static void RegisterColorShortcut(string shortcut, float r, float g, float b) =>
        RegisterColorShortcut(shortcut, EscColor(r, g, b));
    /// <summary>
    /// Gets the escape code for a color shortcut.
    /// </summary>
    /// <param name="shortcut">The shortcut to get.</param>
    /// <param name="escapeCode">The escape code of the shortcut.</param>
    public static void GetColorShortcut(string shortcut, out string escapeCode) =>
        ColorShortcuts.TryGetValue(shortcut, out escapeCode!);
    public static string ReplaceWithColorShortcuts(string text)
    {
        foreach (var shortcut in ColorShortcuts)
        {
            text = text.Replace(shortcut.Key, shortcut.Value);
        }
        return text;
    }
    private static void RegisterColorShortcut(string shortcut, string escapeCode) =>
        ColorShortcuts.Add(shortcut, escapeCode);
    /// <summary>
    /// A constant character to be used with escape codes.
    /// </summary>
    public static char ESC = '\x001B';

    /// <summary>
    /// escColor: Gets the escape code to set a foreground color, or reset the foreground color.
    /// </summary>
    /// <param name="r">The value to be used for Red</param>
    /// <param name="g">The value to be used for Green</param>
    /// <param name="b">The value to be used for Blue</param>
    /// <returns>Foreground color escape code.</returns>
    public static string EscColor(float r, float g, float b) // returns a foreground color escape code 
    {
        // (changes the foreground color based on red, green, and blue values)
        int red = (int)Math.Round((float)(255 * r), MidpointRounding.ToEven);
        int green = (int)Math.Round((float)(255 * g));
        int blue = (int)Math.Round((float)(255 * b));
        return $"{ESC}[38;2;{red};{green};{blue}m";
    }

    /// <summary>
    /// Returns an escape code to set the foreground color to the specified color.
    /// </summary>
    /// <param name="r">Red value</param>
    /// <param name="g">Green value</param>
    /// <param name="b">Blue value</param>
    /// <returns>Foreground color escape code.</returns>
    public static string EscColor(int r, int g, int b) => $"{ESC}[38;2;{r};{g};{b}m";

    /// <summary>
    /// Switches to an alternative console buffer.
    /// </summary>
    public static void SwitchToAlternativeBuffer() => Write($"{ESC}[?1049h");

    /// <summary>
    /// Switches to the default console buffer.
    /// </summary>
    public static void SwitchToMainBuffer() => Write($"{ESC}[?1049l");

    /// <summary>
    /// Waits for the user to press the enter key.
    /// </summary>
    public static void WaitForEnter()
    {
        ConsoleKey key = ConsoleKey.NoName;
        while (key != ConsoleKey.Enter)
        {
            key = Console.ReadKey(true).Key;
        }
    }

    public static string WarningPrefix(string prefixText) =>
        $"{PrefixColor}[{prefixText} {WarningTextColor}Warning{PrefixColor}]{R}";

    public static string ErrorPrefix(string prefixText) =>
        $"{PrefixColor}[{prefixText} {ErrorTextColor}Error{PrefixColor}]{R}";

    public static string Prefix(string prefixText) => $"{PrefixColor}[{prefixText}]{R}";
    public static string Value(string valueText) => $"{ValueColor}{valueText}{R}";
    public static string R = "\x001B[0m";
    public static string PrefixColor = Console.EscColor(0.0f, 1.0f, 1.0f);
    public static string WarningTextColor = Console.EscColor(1.0f, 1.0f, 0.0f);
    public static string ErrorTextColor = Console.EscColor(1.0f, 0.0f, 0.0f);
    public static string ValueColor = Console.EscColor(0.0f, 0.53333333333f, 1.0f);
    public static string GreenTextColor = Console.EscColor(0.0f, 1.0f, 0.13333333333f);
    public static string BlueTextColor = Console.EscColor(0.0f, 0.13333333333f, 1.0f);
    public static string GenericTextColor = Console.R;
}