namespace Otis.Sim.Configuration.Models;

/// <summary>
/// BuildingConfiguration
/// </summary>
public class BuildingConfiguration
{
    /// <summary>
    /// LowestFloor
    /// </summary>
    public int LowestFloor { get; set; }
    /// <summary>
    /// HighestFloor
    /// </summary>
    public int HighestFloor { get; set; }
    /// <summary>
    /// MaximumElevatorLoad
    /// </summary>
    public int MaximumElevatorLoad { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns>The string result</returns>
    public override string ToString()
        => $"LowestFloor: {LowestFloor}, HighestFloor: {HighestFloor}, MaximumElevatorLoad: {MaximumElevatorLoad}";
}
