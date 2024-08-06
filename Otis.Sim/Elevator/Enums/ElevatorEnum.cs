namespace Otis.Sim.Elevator.Enums;

/// <summary>
/// The ElevatorEnum class.
/// </summary>
public class ElevatorEnum
{
    /// <summary>
    /// The RequestStatus enum.
    /// </summary>
    public enum RequestStatus
    {
        Pending,
        Assigned,
        Complete
    }

    /// <summary>
    /// The ElevatorStatus enum.
    /// </summary>
    public enum ElevatorStatus
    {
        Idle,
        MovingUp,
        MovingDown,
        DoorsOpen
    }

    /// <summary>
    /// The ElevatorDirection enum.
    /// </summary>
    public enum ElevatorDirection
    {
        Up,
        Down
    }
}
