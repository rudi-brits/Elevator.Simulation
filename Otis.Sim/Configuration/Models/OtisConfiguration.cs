namespace Otis.Sim.Configuration.Models;

/// <summary>
/// OtisConfiguration
/// </summary>
public class OtisConfiguration
{
    /// <summary>
    /// BuildingConfiguration
    /// </summary>
    public BuildingConfiguration? BuildingConfiguration { get; set; }
    /// <summary>
    /// ElevatorsConfiguration
    /// </summary>
    public List<ElevatorConfiguration>? ElevatorsConfiguration { get; set; }
}
