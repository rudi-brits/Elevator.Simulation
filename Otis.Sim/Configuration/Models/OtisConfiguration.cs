namespace Otis.Sim.Configuration.Models
{
    public class OtisConfiguration
    {
        public BuildingConfiguration? BuildingConfiguration { get; set; }
        public List<ElevatorConfiguration>? ElevatorsConfiguration { get; set; }
    }
}
