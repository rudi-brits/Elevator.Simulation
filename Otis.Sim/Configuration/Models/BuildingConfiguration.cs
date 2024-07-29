namespace Otis.Sim.Configuration.Models
{
    public class BuildingConfiguration
    {
        public int LowestFloor { get; set; }
        public int HighestFloor { get; set; }
        public int MaximumElevatorLoad { get; set; }

        public override string ToString()
        {
            return $"LowestFloor: {LowestFloor}, HighestFloor: {HighestFloor}, MaximumElevatorLoad: {MaximumElevatorLoad}";
        }
    }
}
