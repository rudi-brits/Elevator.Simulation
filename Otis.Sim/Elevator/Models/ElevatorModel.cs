using Otis.Sim.Configuration.Models;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorModel : ElevatorConfigurationBase
    {
        public int Id { get; set; }
        public new int LowestFloor { get; set; }
        public new int HighestFloor { get; set; }
        public int CurrentFloor { get; set; } = 0;
        public int? NextFloor { get; set; }
        public int CurrentLoad { get; set; } = 0;
        public int MaximumLoad { get; set; }
        public int Capacity => MaximumLoad - CurrentLoad;

        private ElevatorStatus _currentStatus { get; set; } = ElevatorStatus.Idle;
        public ElevatorStatus CurrentStatus => _currentStatus;

        public delegate void CompleteRequestDelegate(Guid requestId);
        public CompleteRequestDelegate CompleteRequest;

        public override string ToString()
        {
            return $"Id: {Id}, Description: {Description}, LowestFloor: {LowestFloor}, " +
                $"HighestFloor: {HighestFloor}, MaximumLoad: {MaximumLoad}";
        }
    }
}
