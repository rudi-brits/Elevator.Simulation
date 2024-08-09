namespace Otis.Sim.Configuration.Models;

/// <summary>
/// Class ElevatorConfiguration extends the <see cref="ElevatorConfigurationBase" /> class.
/// </summary>
public class ElevatorConfiguration : ElevatorConfigurationBase
{
    /// <summary>
    /// LowestFloor
    /// </summary>
    public int? LowestFloor { get; set; }
    /// <summary>
    /// HighestFloor
    /// </summary>
    public int? HighestFloor { get; set; }
}
