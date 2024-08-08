using Otis.Sim.Constants;
using Otis.Sim.Interface.Validators.Helpers;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorRequest extends the <see cref="ElevatorRequestBase" /> class.
/// </summary>
public class ElevatorRequest : ElevatorRequestBase
{
    /// <summary>
    /// ElevatorId
    /// </summary>
    public int? ElevatorId { get; set; }
    /// <summary>
    /// RequestStatus
    /// </summary>
    public RequestStatus RequestStatus
    {
        get
        {
            if (ElevatorId == null)
                return RequestStatus.Pending;

            return ElevatorId > 0
                ? RequestStatus.Assigned
                : RequestStatus.Complete;
        }
    }
    /// <summary>
    /// Direction
    /// </summary>
    public ElevatorDirection Direction
    {
        get
        {
            return (OriginFloor < DestinationFloor)
                ? ElevatorDirection.Up
                : ElevatorDirection.Down;
        }
    }

    /// <summary>
    /// ElevatorRequest constructor
    /// </summary>
    /// <param name="userInputRequest"></param>
    public ElevatorRequest(UserInputRequest userInputRequest)
    {
        Id               = Guid.NewGuid();
        OriginFloor      = (int)UStringHelper.ToInteger(userInputRequest.OriginFloorInput)!;
        DestinationFloor = (int)UStringHelper.ToInteger(userInputRequest.DestinationFloorInput)!;
        NumberOfPeople   = (int)UStringHelper.ToInteger(userInputRequest.CapacityInput)!;
    }

    /// <summary>
    /// ToDuplicateRequestString function
    /// </summary>
    public string ToDuplicateRequestString()
    {
        return
            $"{OtisSimConstants.OriginFloorName}: {OriginFloor}, " +
            $"{OtisSimConstants.DestinationFloorName}: {DestinationFloor}, " +
            $"{nameof(Direction)}: {Direction}";
    }

    /// <summary>
    /// ToQueuedRequestString function
    /// </summary>
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
