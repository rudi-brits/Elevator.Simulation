using Otis.Sim.Interface.Interfaces;
using Terminal.Gui;
using TerminalGuiApplication = Terminal.Gui.Application;

namespace Otis.Sim.Interface.Models;

public class SimTerminalGuiApplication : ISimTerminalGuiApplication
{
    public void Init()
    {
        TerminalGuiApplication.Init();
    }

    public Toplevel Top => TerminalGuiApplication.Top;

    public void Run()
    {
        TerminalGuiApplication.Run();
    }
}
