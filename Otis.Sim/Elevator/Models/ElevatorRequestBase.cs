namespace Otis.Sim.Elevator.Models;

/// <summary>
/// ElevatorRequestBase abstract class
/// </summary>
public abstract class ElevatorRequestBase
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// OriginFloor
    /// </summary>
    public int OriginFloor { get; set; }
    /// <summary>
    /// DestinationFloor
    /// </summary>
    public int DestinationFloor { get; set; }
    /// <summary>
    /// NumberOfPeople
    /// </summary>
    public int NumberOfPeople { get; set; }
}
