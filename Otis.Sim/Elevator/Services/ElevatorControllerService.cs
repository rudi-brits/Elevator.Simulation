using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Elevator.Services
{
    public class ElevatorControllerService
    {
        private List<ElevatorModel> _elevators;

        private readonly object _lockRequestQueue;
        private HashSet<Guid> _completedRequestIds;

        private OtisConfigurationService _configurationService;

        public ElevatorControllerService(OtisConfigurationService configurationService)
        {
            _configurationService = configurationService;

            _elevators = new List<ElevatorModel>();
        }

        public void LoadConfiguration()
        {
            var elevatorId = 0;
            var buildingConfiguration = _configurationService.BuildingConfiguration!;

            _configurationService.ElevatorsConfiguration!.ForEach(elevatorConfiguration =>
            {
                _elevators.Add(new ElevatorModel
                {
                    Id              = ++elevatorId,
                    Description     = elevatorConfiguration.Description.Trim(),
                    LowestFloor     = elevatorConfiguration.LowestFloor.ApplyHigherValue(buildingConfiguration.LowestFloor),
                    HighestFloor    = elevatorConfiguration.HighestFloor.ApplyLowerValue(buildingConfiguration.HighestFloor),
                    MaximumLoad     = buildingConfiguration.MaximumElevatorLoad,
                    CompleteRequest = CompleteRequest
                });
            });

            PrintLoadedConfiguration();
        }

        private void PrintLoadedConfiguration()
        {
            Console.WriteLine();

            Console.WriteLine(
                "NOTE: Elevator configuration may be overridden by building configuration where values exceed the bounds of the building");
            Console.WriteLine();

            Console.WriteLine("Building configuration:");
            Console.WriteLine(_configurationService.BuildingConfiguration!.ToString());
            Console.WriteLine();

            Console.WriteLine("Elevators configuration:");
            _elevators.ForEach(elevator => Console.WriteLine(elevator.ToString()));

            Console.WriteLine();
        }

        public void CompleteRequest(Guid requestId)
        {
            lock (_lockRequestQueue)
            {
                if (!_completedRequestIds.Any(id => id == requestId))
                {
                    _completedRequestIds.Add(requestId);
                }
            }
        }
    }
}
