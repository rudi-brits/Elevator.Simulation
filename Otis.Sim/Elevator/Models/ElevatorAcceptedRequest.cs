using static Otis.Sim.Elevator.Enums.ElevatorEnum;
using UiConstants = Otis.Sim.Interface.Constants.TerminalUiConstants;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorAcceptedRequest : ElevatorRequestBase
    {
        public ElevatorDirection RequestDirection { get; set; }
        public bool OriginFloorServiced { get; set; }
        public bool DestinationFloorServiced { get; set; }
        public bool Completed => OriginFloorServiced && DestinationFloorServiced;

        public string ToAcceptedRequestString()
        {
            return
                $"{nameof(Id)}: {Id}, " +
                $"{UiConstants.OriginFloorName}: {OriginFloor}, " +
                $"{UiConstants.DestinationFloorName}: {DestinationFloor}, " +
                $"{UiConstants.NumberOfPeopleName}: {Capacity}, " +
                $"Status: Accepted, " +
                $"Direction: {RequestDirection}";
        }
    }
}
