namespace Otis.Sim.Elevator.Enums
{
    public class ElevatorEnum
    {
        public enum RequestStatus
        {
            Pending,
            Assigned,
            Complete
        }

        public enum ElevatorStatus
        {
            Idle,
            MovingUp,
            MovingDown,
            DoorsOpen
        }

        public enum ElevatorDirection
        {
            Up,
            Down
        }
    }
}
