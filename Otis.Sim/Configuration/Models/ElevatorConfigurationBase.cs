namespace Otis.Sim.Configuration.Models
{
    public abstract class ElevatorConfigurationBase
    {
        public string Description { get; set; } = string.Empty;
        public int? LowestFloor { get; set; }
        public int? HighestFloor { get; set; }
    }
}
