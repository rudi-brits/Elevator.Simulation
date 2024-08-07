using Terminal.Gui;

namespace Otis.Sim.Interface.Interfaces;

/// <summary>
/// ISimTerminalGuiApplication interface.
/// </summary>
public interface ISimTerminalGuiApplication
{
    /// <summary>
    /// Init function.
    /// </summary>
    void Init();
    /// <summary>
    /// Toplevel property.
    /// </summary>
    Toplevel Top { get; }
    /// <summary>
    /// Run function.
    /// </summary>
    void Run();
}
