using Otis.Sim.Constants;
using Otis.Sim.Interface.Validators.Helpers;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorRequest : ElevatorRequestBase
    {
        public int? ElevatorId { get; set; }
        public RequestStatus RequestStatus
        {
            get
            {
                if (ElevatorId == null)
                {
                    return RequestStatus.Pending;
                }

                return ElevatorId > 0
                    ? RequestStatus.Assigned
                    : RequestStatus.Complete;
            }
        }
        public ElevatorDirection Direction
        {
            get
            {
                return (OriginFloor < DestinationFloor)
                    ? ElevatorDirection.Up
                    : ElevatorDirection.Down;
            }
        }

        public ElevatorRequest(UserInputRequest userInputRequest)
        {
            Id               = Guid.NewGuid();
            OriginFloor      = (int)UStringHelper.ToInteger(userInputRequest.OriginFloorInput)!;
            DestinationFloor = (int)UStringHelper.ToInteger(userInputRequest.DestinationFloorInput)!;
            NumberOfPeople   = (int)UStringHelper.ToInteger(userInputRequest.CapacityInput)!;
        }

        public string ToDuplicateRequestString()
        {
            return
                $"{OtisSimConstants.OriginFloorName}: {OriginFloor}, " +
                $"{OtisSimConstants.DestinationFloorName}: {DestinationFloor}, " +
                $"{nameof(Direction)}: {Direction}";
        }

        public string ToQueuedRequestString()
        {
            return
                $"{nameof(Id)}: {Id}, " +
                $"{OtisSimConstants.OriginFloorName}: {OriginFloor}, " + 
                $"{OtisSimConstants.DestinationFloorName}: {DestinationFloor}, " + 
                $"{OtisSimConstants.PeopleName}: {NumberOfPeople}, " +
                $"{OtisSimConstants.Status}: {RequestStatus}, " +
                $"{nameof(Direction)}: {Direction}";
        }
    }
}
