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
    /// The IdleColorAttribute used to display a doors open state.
    /// </summary>
    public static Attribute DoorsOpenColorAttribute = Attribute.Make(Color.BrightRed, Color.Black);
    /// <summary>
    /// The MovingUpColorAttribute used to display a moving up state.
    /// </summary>
    public static Attribute MovingUpColorAttribute = Attribute.Make(Color.BrightCyan, Color.Black);
    /// <summary>
    /// The MovingUpColorAttribute used to display a moving down state.
    /// </summary>
    public static Attribute MovingDownColorAttribute = Attribute.Make(Color.BrightYellow, Color.Black);
}
