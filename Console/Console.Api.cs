using System.Runtime.InteropServices;

namespace CustomConsole;
public static partial class Console
{
    public static class Api
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern nint GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        internal static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll")]
        internal static extern nint GetConsoleWindow();

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        internal static void SetWindowState(IntPtr hWnd, Config.ConsoleVisibility nCmdShow) => ShowWindow(hWnd, (int)nCmdShow);
    }
}