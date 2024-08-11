using System.Runtime.InteropServices;

namespace Otis.Sim.Interface.Services;

/// <summary>
/// The ConsoleFullScreenService class.
/// </summary>
public class ConsoleFullScreenService
{
    /// <summary>
    /// GetConsoleWindow kernel32.dll function.
    /// </summary>
    /// <returns>The IntPtr result</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetConsoleWindow();
    /// <summary>
    /// GetWindowLong user32.dll function.
    /// </summary>
    /// <returns>The int result</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    /// <summary>
    /// SetWindowLong user32.dll function.
    /// </summary>
    /// <returns>The int result</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    /// <summary>
    /// ShowWindow user32.dll function.
    /// </summary>
    /// <returns>The bool result</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    /// <summary>
    /// GWL_STYLE constant.
    /// </summary>
    private static int GWL_STYLE      = -16;
    /// <summary>
    /// SW_MAXIMIZE constant.
    /// </summary>
    private static int SW_MAXIMIZE    = 3;
    /// <summary>
    /// WS_MAXIMIZEBOX constant.
    /// </summary>
    private static int WS_MAXIMIZEBOX = 0x10000;
    /// <summary>
    /// WS_MINIMIZEBOX constant.
    /// </summary>
    private static int WS_MINIMIZEBOX = 0x20000;
    /// <summary>
    /// WS_SIZEBOX constant.
    /// </summary>
    private static int WS_SIZEBOX     = 0x40000;

    /// <summary>
    /// The InitialiseFullScreen function.
    /// </summary>
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