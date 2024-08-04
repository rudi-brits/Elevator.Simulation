using Otis.Sim.Services;

namespace Elevator.Simulation
{
    internal class Program
    {
        private static OtisSimulationService _otisSimulationService;

        static void Main(string[] args)
        { 
            _otisSimulationService = new OtisSimulationService();
        }
    }
}