using Otis.Sim.Interface.Interfaces;
using Terminal.Gui;
using TerminalGuiApplication = Terminal.Gui.Application;

namespace Otis.Sim.Interface.Models;

/// <summary>
/// The SimTerminalGuiApplication class implements the <see cref="ISimTerminalGuiApplication" /> interface.
/// </summary>
public class SimTerminalGuiApplication : ISimTerminalGuiApplication
{
    /// <summary>
    /// Init function.
    /// </summary>
    public void Init()
    {
        TerminalGuiApplication.Init();
    }
    /// <summary>
    /// Toplevel property.
    /// </summary>
    public Toplevel Top => TerminalGuiApplication.Top;
    /// <summary>
    /// Run function.
    /// </summary>
    public void Run()
    {
        TerminalGuiApplication.Run();
    }
}
