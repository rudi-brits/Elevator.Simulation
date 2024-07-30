using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Services;
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
            var initilaisedSuccess = false;
            ServiceProvider serviceProvider = null;

            try
            {
                serviceProvider = SetupServiceCollection();

                LoadAppConfiguration(serviceProvider);
                LoadElevatorControllerConfiguration(serviceProvider);

                initilaisedSuccess = true;
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

            DisplayUserInputMessage($"Press any key to {(initilaisedSuccess ? "continue" : "exit")}...");

            if (initilaisedSuccess)
            {
                InitialiseTerminalUi(serviceProvider!);
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
                serviceCollection.AddSingleton<TerminalUiService>();

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

        private void InitialiseTerminalUi(ServiceProvider serviceProvider)
        {
            try
            { 
                var terminalUiService = serviceProvider.GetService<TerminalUiService>();
                terminalUiService.InitialiseUi();
            }
            catch (Exception exc)
            {
                DisplayUserInputMessage($"UI initialisation failed. Press any key to exit - {exc.Message}");
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

        private void DisplayUserInputMessage(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey();
        }
    }
}
