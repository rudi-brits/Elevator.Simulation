using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorDataRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int CurrentFloor { get; set; }
        public int NextFloor { get; set; }
        public int CurrentLoad { get; set; }
        public int Capacity { get; set; }
        public ElevatorStatus Status { get; set; }
    }
}
