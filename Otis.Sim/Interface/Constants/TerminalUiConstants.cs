using Attribute = Terminal.Gui.Attribute;
using Color = Terminal.Gui.Color;

namespace Otis.Sim.Interface.Constants;

/// <summary>
/// The TerminalUiConstants class.
/// </summary>
public class TerminalUiConstants
{
    /// <summary>
    /// The GlobalColorAttribute used as the global, default state.
    /// </summary>
    public static Attribute GlobalColorAttribute = Attribute.Make(Color.White, Color.Black);
    /// <summary>
    /// The IdleColorAttribute used to display an idle state.
    /// </summary>
    public static Attribute IdleColorAttribute   = Attribute.Make(Color.BrightCyan, Color.Black);
}
