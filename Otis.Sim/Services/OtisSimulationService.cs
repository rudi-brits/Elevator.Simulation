using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Services;
using Otis.Sim.MappingProfiles;

namespace Otis.Sim.Services
{
    public class OtisSimulationService
    {
        const string successPrefix = "Success - ";

        public OtisSimulationService()
        {
            RunSimulation();
        }

        private void RunSimulation()
        {
            try
            {
                var serviceProvider = SetupServiceCollection();

                LoadAppConfiguration(serviceProvider);
                LoadElevatorControllerConfiguration(serviceProvider);
            }
            catch (Exception exc)
            {
                Console.WriteLine();
                Console.WriteLine($"Application startup failed: {exc.Message}");
                
                if (!string.IsNullOrWhiteSpace(exc.InnerException?.Message))
                {                    
                    Console.WriteLine(exc.InnerException.Message);
                }
            }
        }

        private ServiceProvider SetupServiceCollection()
        {
            try
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

                Console.WriteLine(SetupServiceCollectionMessage());

                return serviceProvider;
            }
            catch (Exception exc)
            {
                throw new Exception(SetupServiceCollectionMessage(false), exc);
            }
        }

        private void LoadAppConfiguration(ServiceProvider serviceProvider)
        {
            try
            {
                var otisConfigurationService = serviceProvider.GetService<OtisConfigurationService>();
                otisConfigurationService.LoadConfiguration();

                Console.WriteLine(LoadAppConfigurationMessage());
            }
            catch (Exception exc)
            {
                throw new Exception(LoadAppConfigurationMessage(false), exc);
            }
        }

        private void LoadElevatorControllerConfiguration(ServiceProvider serviceProvider)
        {
            try
            {
                var elevatorControllerService = serviceProvider.GetService<ElevatorControllerService>();
                elevatorControllerService.LoadConfiguration();

                Console.WriteLine(LoadElevatorControllerConfigurationMessage());
            }
            catch (Exception exc)
            {
                throw new Exception(LoadElevatorControllerConfigurationMessage(false), exc);
            }
        }

        private string SetupServiceCollectionMessage(bool isSuccess = true)
            => $"{GetPrefix(isSuccess)}setup services and service provider";

        private string LoadAppConfigurationMessage(bool isSuccess = true)
            => $"{GetPrefix(isSuccess)} load app configuration from appsettings";

        private string LoadElevatorControllerConfigurationMessage(bool isSuccess = true)
            => $"{GetPrefix(isSuccess)} load elevator configuration";

        private string GetPrefix(bool isSuccess = true)
            => isSuccess ? successPrefix : "";
    }
}
