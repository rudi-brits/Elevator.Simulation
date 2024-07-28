using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Services;
using Otis.Sim.MappingProfiles;

namespace Otis.Sim.Services
{
    public class OtisSimulationService
    {
        public OtisSimulationService()
        {
            // TODO: Add error handling for all parts here
            RunSimulation();
        }

        private void RunSimulation()
        {
            var serviceProvider = SetupServiceCollection();

            LoadConfiguration(serviceProvider);
            RunElevatorControllerService(serviceProvider);
        }

        private ServiceProvider SetupServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            var configuration = new MapperConfiguration(config =>
            {
                config.AddProfile<OtisMappingProfile>();
            });

            IMapper mapper = configuration.CreateMapper();

            serviceCollection.AddSingleton(mapper);
            serviceCollection.AddSingleton<OtisConfigurationService>();
            serviceCollection.AddSingleton<ElevatorControllerService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        private void LoadConfiguration(ServiceProvider serviceProvider)
        {
            var otisConfigurationService = serviceProvider.GetService<OtisConfigurationService>();
            otisConfigurationService.LoadConfiguration();
        }

        private void RunElevatorControllerService(ServiceProvider serviceProvider)
        {
            var elevatorControllerService = serviceProvider.GetService<ElevatorControllerService>();
            elevatorControllerService.LoadConfiguration();
        }
    }
}
