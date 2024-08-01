using static Otis.Sim.Elevator.Enums.ElevatorEnum;
using UiConstants = Otis.Sim.Interface.Constants.TerminalUiConstants;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorAcceptedRequest : ElevatorRequestBase
    {
        public string ElevatorName { get; set; }
        public ElevatorDirection RequestDirection { get; set; }
        public bool OriginFloorServiced { get; set; }
        public bool DestinationFloorServiced { get; set; }
        public bool Completed => OriginFloorServiced && DestinationFloorServiced;

        public string ToAcceptedRequestString()
            => ToStatusString("Accepted");

        public string ToCompletedRequestString()
            => ToStatusString("Completed");

        public string ToRequeuedRequestString()
            => ToStatusString("Requeued");

        private string ToStatusString(string status)
        {
            return
                $"{nameof(Id)}: {Id}, " +
                $"{UiConstants.OriginFloorName}: {OriginFloor}, " +
                $"{UiConstants.DestinationFloorName}: {DestinationFloor}, " +
                $"{UiConstants.NumberOfPeopleName}: {NumberOfPeople}, " +
                $"Status: {status}, " +
                $"Direction: {RequestDirection}, " +
                $"Elevator: {ElevatorName}";
        }
    }
}
