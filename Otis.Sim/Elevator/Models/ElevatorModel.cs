namespace Otis.Sim.Elevator.Models
{
    public class ElevatorModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int LowestFloor { get; set; }
        public int HighestFloor { get; set; }
        public int CurrentFloor { get; set; } = 0;
        public int? NextFloor { get; set; }
        public int CurrentLoad { get; set; } = 0;
        public int MaximumLoad { get; set; }

        public delegate void CompleteRequestDelegate(Guid requestId);
        public CompleteRequestDelegate CompleteRequest;

        public override string ToString()
        {
            return $"Id: {Id}, Description: {Description}, LowestFloor: {LowestFloor}, " +
                $"HighestFloor: {HighestFloor}, MaximumLoad: {MaximumLoad}";
        }
    }
}
