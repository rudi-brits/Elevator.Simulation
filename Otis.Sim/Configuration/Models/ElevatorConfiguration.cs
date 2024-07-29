namespace Otis.Sim.Configuration.Models
{
    public class ElevatorConfiguration
    {
        public string Description { get; set; } = string.Empty;
        public int? LowestFloor { get; set; }
        public int? HighestFloor { get; set; }
    }
}
