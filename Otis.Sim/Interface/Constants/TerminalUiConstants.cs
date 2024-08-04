using Attribute = Terminal.Gui.Attribute;
using Color = Terminal.Gui.Color;

namespace Otis.Sim.Interface.Constants
{
    public class TerminalUiConstants
    {
        public const string OriginFloorName      = "Origin floor";
        public const string DestinationFloorName = "Destination floor";
        public const string NumberOfPeopleName   = "Number of people";
        public const string PeopleName = "People";

        public static Attribute GlobalColorAttribute = Attribute.Make(Color.White, Color.Black);
        public static Attribute IdleColorAttribute   = Attribute.Make(Color.BrightCyan, Color.Black);
    }
}
