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

        private ServiceProvider _serviceProvider;

        public OtisSimulationService()
        {
            RunSimulation();
        }

        public void RunSimulation()
        {
            var initilaisedSuccess = false;

            try
            {
                SetupServiceCollection();

                LoadAppConfiguration();
                LoadElevatorControllerConfiguration();

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
                InitialiseTerminalUi();
        }

        private void SetupServiceCollection()
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

                _serviceProvider = serviceCollection.BuildServiceProvider();

                Console.WriteLine(SetupServiceCollectionMessage());
            }
            catch (Exception exc)
            {
                throw new Exception(SetupServiceCollectionMessage(false), exc);
            }
        }

        private void LoadAppConfiguration()
        {
            try
            {
                var otisConfigurationService = _serviceProvider.GetService<OtisConfigurationService>();
                otisConfigurationService.LoadConfiguration();

                Console.WriteLine(LoadAppConfigurationMessage());
            }
            catch (Exception exc)
            {
                throw new Exception(LoadAppConfigurationMessage(false), exc);
            }
        }

        private void LoadElevatorControllerConfiguration()
        {
            try
            {
                var elevatorControllerService = _serviceProvider.GetService<ElevatorControllerService>();
                elevatorControllerService.LoadConfiguration();

                Console.WriteLine(LoadElevatorControllerConfigurationMessage());
            }
            catch (Exception exc)
            {
                throw new Exception(LoadElevatorControllerConfigurationMessage(false), exc);
            }
        }

        private void InitialiseTerminalUi()
        {
            try
            { 
                var terminalUiService = _serviceProvider.GetService<TerminalUiService>();
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
