using NStack;

namespace Otis.Sim.Elevator.Models;

/// <summary>
/// UserInputRequest class
/// </summary>
public class UserInputRequest
{
    /// <summary>
    /// OriginFloorInput
    /// </summary>
    public ustring? OriginFloorInput { get; set; }
    /// <summary>
    /// DestinationFloorInput
    /// </summary>
    public ustring? DestinationFloorInput { get; set; }
    /// <summary>
    /// CapacityInput
    /// </summary>
    public ustring? CapacityInput { get; set; }
}
