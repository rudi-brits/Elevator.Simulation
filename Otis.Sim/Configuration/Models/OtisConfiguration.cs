namespace Otis.Sim.Configuration.Models
{
    public class OtisConfiguration
    {
        public OtisBuildingConfiguration BuildingConfiguration { get; set; }
        public List<OtisElevatorConfiguration> ElevatorsConfiguration { get; set; }
    }
}
