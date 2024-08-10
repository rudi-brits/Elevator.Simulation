using Otis.Sim.Constants;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models;

/// <summary>
/// ElevatorAcceptedRequest extends the <see cref="ElevatorRequestBase" /> class.
/// </summary>
public class ElevatorAcceptedRequest : ElevatorRequestBase
{
    /// <summary>
    /// ElevatorName
    /// </summary>
    public string ElevatorName { get; set; } = "";
    /// <summary>
    /// RequestDirection
    /// </summary>
    public ElevatorDirection RequestDirection { get; set; }
    /// <summary>
    /// OriginFloorServiced
    /// </summary>
    public bool OriginFloorServiced { get; set; }
    /// <summary>
    /// DestinationFloorServiced
    /// </summary>
    public bool DestinationFloorServiced { get; set; }
    /// <summary>
    /// Completed
    /// </summary>
    public bool Completed => OriginFloorServiced && DestinationFloorServiced;

    /// <summary>
    /// ToAcceptedRequestString
    /// </summary>
    /// <returns></returns>
    public string ToAcceptedRequestString()
        => ToStatusString(OtisSimConstants.Accepted);
    /// <summary>
    /// ToPickedUpRequestString
    /// </summary>
    /// <param name="numberOfPeople"></param>
    /// <param name="capacity"></param>
    /// <returns></returns>
    public string ToPickedUpRequestString(int numberOfPeople, int capacity)
        => ToEmbarkDisembarkString(OtisSimConstants.PickUp, numberOfPeople, capacity);

    public string ToDroppedOffRequestString(int numberOfPeople, int capacity)
        => ToEmbarkDisembarkString(OtisSimConstants.DropOff, numberOfPeople, capacity);
    /// <summary>
    /// ToEmbarkDisembarkString
    /// </summary>
    /// <param name="description"></param>
    /// <param name="numberOfPeople"></param>
    /// <param name="capacity"></param>
    /// <returns></returns>
    private string ToEmbarkDisembarkString(string description, int numberOfPeople, int capacity)
        => $"{ToStatusString($"{ElevatorStatus.DoorsOpen} ({description})")}, " +
           $"{OtisSimConstants.PeopleName}: {numberOfPeople}, " +
           $"{OtisSimConstants.Capacity}: {capacity}, ";
    /// <summary>
    /// ToCompletedRequestString
    /// </summary>
    /// <returns></returns>
    public string ToCompletedRequestString()
        => ToStatusString(OtisSimConstants.Completed);
    /// <summary>
    /// ToRequeuedRequestString
    /// </summary>
    /// <returns></returns>
    public string ToRequeuedRequestString()
        => ToStatusString(OtisSimConstants.Requeued);
    /// <summary>
    /// ToStatusString
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    private string ToStatusString(string status)
    {
        return
            $"{nameof(Id)}: {Id}, " +
            $"{OtisSimConstants.OriginFloorName}: {OriginFloor}, " +
            $"{OtisSimConstants.DestinationFloorName}: {DestinationFloor}, " +
            $"{OtisSimConstants.PeopleName}: {NumberOfPeople}, " +
            $"{OtisSimConstants.Status}: {status}, " +
            $"{nameof(RequestDirection)}: {RequestDirection}, " +
            $"{OtisSimConstants.Elevator}: {ElevatorName}";
    }
}
