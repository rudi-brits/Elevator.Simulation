using Otis.Sim.Interface.Validators.Helpers;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorRequest
    {
        public Guid Id { get; set; }
        public int OriginFloor { get; set; }
        public int DestinationFloor { get; set; }
        public int Capacity { get; set; }
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
            OriginFloor      = (int)UStringHelper.ToInteger(userInputRequest.OriginFloorInput);
            DestinationFloor = (int)UStringHelper.ToInteger(userInputRequest.DestinationFloorInput);
            Capacity         = (int)UStringHelper.ToInteger(userInputRequest.CapacityInput);
        }
    }
}
