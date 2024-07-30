using System.Runtime.InteropServices;

namespace Otis.Sim.Interface.Services
{
    public class ConsoleFullScreenService
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int GWL_STYLE = -16;
        private const int SW_MAXIMIZE = 3;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SIZEBOX = 0x40000;

        protected void InitialiseFullScreen()
        {
            IntPtr consoleWindow = GetConsoleWindow();

            if (consoleWindow != IntPtr.Zero)
            {
                ShowWindow(consoleWindow, SW_MAXIMIZE);
            }

            int style = GetWindowLong(consoleWindow, GWL_STYLE);
            style &= ~(WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_SIZEBOX);

            SetWindowLong(consoleWindow, GWL_STYLE, style);
        }
    }
}
