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

        public string ToPickedUpRequestString(int numberOfPeople, int capacity)
            => _toEmbarkDisembarkString("Pick up", numberOfPeople, capacity);

        public string ToDroppedOffRequestString(int numberOfPeople, int capacity)
            => _toEmbarkDisembarkString("Drop off", numberOfPeople, capacity);

        private string _toEmbarkDisembarkString(string description, int numberOfPeople, int capacity)
            => $"{ToStatusString($"{ElevatorStatus.DoorsOpen} ({description})")}, " +
               $"{UiConstants.PeopleName}: {numberOfPeople}, " +
               $"Capacity: {capacity}, ";

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
                $"{UiConstants.PeopleName}: {NumberOfPeople}, " +
                $"Status: {status}, " +
                $"Direction: {RequestDirection}, " +
                $"Elevator: {ElevatorName}";
        }
    }
}
