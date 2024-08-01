namespace Otis.Sim.Elevator.Models
{
    public abstract class ElevatorRequestBase
    {
        public Guid Id { get; set; }
        public int OriginFloor { get; set; }
        public int DestinationFloor { get; set; }
        public int NumberOfPeople { get; set; }
    }
}
