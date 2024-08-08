using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models;

/// <summary>
/// The ElevatorDataRow class
/// </summary>
public class ElevatorDataRow
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// CurrentFloor
    /// </summary>
    public int CurrentFloor { get; set; }
    /// <summary>
    /// NextFloor
    /// </summary>
    public int NextFloor { get; set; }
    /// <summary>
    /// CurrentLoad
    /// </summary>
    public int CurrentLoad { get; set; }
    /// <summary>
    /// Capacity
    /// </summary>
    public int Capacity { get; set; }
    /// <summary>
    /// Status
    /// </summary>
    public ElevatorStatus Status { get; set; }
}
