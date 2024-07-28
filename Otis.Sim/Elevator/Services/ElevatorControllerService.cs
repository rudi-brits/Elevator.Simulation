using Otis.Sim.Configuration.Services;

namespace Otis.Sim.Elevator.Services
{
    public class ElevatorControllerService
    {
        private OtisConfigurationService _configurationService;

        public ElevatorControllerService(OtisConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public void LoadConfiguration()
        {

        }
    }
}
