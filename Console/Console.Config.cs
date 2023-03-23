namespace CustomConsole;

public static partial class Console
{
    public static class Config
    {
        public enum ConsoleVisibility
        {
            Hidden,
            Shown = 5
        }

        private const int StdOutputHandle = -11;
        private const uint EnableVirtualTerminalProcessing = 4;

        /// <summary>
        /// Use this to set up the console for ANSI escape codes
        /// </summary>
        public static void SetupConsole()
        {
            IntPtr handle = Api.GetStdHandle(StdOutputHandle);
            Api.GetConsoleMode(handle, out uint mode);
            mode |= EnableVirtualTerminalProcessing;
            Api.SetConsoleMode(handle, mode);
        }

        public static void HideConsole()
        {
            IntPtr handle = Api.GetConsoleWindow();
            Api.SetWindowState(handle, ConsoleVisibility.Hidden);
        }

        public static void ShowConsole()
        {
            IntPtr handle = Api.GetConsoleWindow();
            Api.SetWindowState(handle, ConsoleVisibility.Shown);
        }
    }
}
