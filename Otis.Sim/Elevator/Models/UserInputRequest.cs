using NStack;

namespace Otis.Sim.Elevator.Models
{
    public class UserInputRequest
    {
        public ustring? OriginFloorInput { get; set; }
        public ustring? DestinationFloorInput { get; set; }
        public ustring? CapacityInput { get; set; }
    }
}
