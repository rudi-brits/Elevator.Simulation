using Terminal.Gui;

namespace Otis.Sim.Interface.Interfaces
{
    public interface ISimTerminalGuiApplication
    {
        void Init();
        Toplevel Top { get; }
        void Run();
    }
}
