using System;
using System.Runtime.InteropServices;

namespace destiny_chat_client.Classes
{
    /// <summary>
    ///     Used for handling the operation of win32 window api calls
    /// </summary>
    public class WindowHandler
    {
        private const int Restore = 9;

        /// <summary>
        ///     Focus the opened chatroom.
        /// </summary>
        public static void FocusChat()
        {
            var hwnd = FindWindow(null, "destiny.gg chat");
            ShowWindow(hwnd, Restore);
            SetForegroundWindow(hwnd);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);
    }
}