using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Interfaces;
using Otis.Sim.Interface.Models;
using Otis.Sim.Interface.Services;
using Otis.Sim.MappingProfiles;

namespace Otis.Sim.Services;

/// <summary>
/// OtisSimulationService class
/// </summary>
public class OtisSimulationService
{
    /// <summary>
    /// successPrefix
    /// </summary>
    protected string SuccessPrefix = "Success - ";

    /// <summary>
    /// _serviceProvider
    /// </summary>
    protected ServiceProvider? _serviceProvider;

    /// <summary>
    /// OtisSimulationService constructor
    /// </summary>
    public OtisSimulationService()
    {
        RunSimulation();
    }

    /// <summary>
    /// RunSimulation
    /// </summary>
    public virtual void RunSimulation()
    {
        var initialisedSuccess = false;

        try
        {
            SetupServiceCollection();

            LoadAppConfiguration();
            LoadElevatorControllerConfiguration();

            initialisedSuccess = true;
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

        DisplayUserInputMessage($"Press any key to {(initialisedSuccess ? "continue" : "exit")}...");

        if (initialisedSuccess)
            InitialiseTerminalUi();
    }

    /// <summary>
    /// SetupServiceCollection
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected virtual void SetupServiceCollection()
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
            serviceCollection.AddSingleton<ISimTerminalGuiApplication, SimTerminalGuiApplication>();
            serviceCollection.AddSingleton<TerminalUiService>();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            WriteLineToConsole(SetupServiceCollectionMessage());
        }
        catch (Exception exc)
        {
            throw new Exception(SetupServiceCollectionMessage(false), exc);
        }
    }    

    protected virtual void LoadAppConfiguration()
    {
        try
        {
            var otisConfigurationService = _serviceProvider?.GetService<OtisConfigurationService>();
            otisConfigurationService?.LoadConfiguration();

            Console.WriteLine(LoadAppConfigurationMessage());
        }
        catch (Exception exc)
        {
            throw new Exception(LoadAppConfigurationMessage(false), exc);
        }
    }

    protected virtual void LoadElevatorControllerConfiguration()
    {
        try
        {
            var elevatorControllerService = _serviceProvider?.GetService<ElevatorControllerService>();
            elevatorControllerService?.LoadConfiguration();

            Console.WriteLine(LoadElevatorControllerConfigurationMessage());
        }
        catch (Exception exc)
        {
            throw new Exception(LoadElevatorControllerConfigurationMessage(false), exc);
        }
    }

    protected virtual void InitialiseTerminalUi()
    {
        try
        { 
            var terminalUiService = _serviceProvider?.GetService<TerminalUiService>();
            terminalUiService?.InitialiseUi();
        }
        catch (Exception exc)
        {
            DisplayUserInputMessage($"UI initialisation failed. Press any key to exit - {exc.Message}");
        }
    }

    protected virtual string SetupServiceCollectionMessage(bool isSuccess = true)
        => $"{GetPrefix(isSuccess)} setup services and service provider";

    protected virtual string LoadAppConfigurationMessage(bool isSuccess = true)
        => $"{GetPrefix(isSuccess)} load app configuration from appsettings";

    protected virtual string LoadElevatorControllerConfigurationMessage(bool isSuccess = true)
        => $"{GetPrefix(isSuccess)} load elevator configuration";

    protected virtual string GetPrefix(bool isSuccess = true)
        => isSuccess ? SuccessPrefix : "";

    protected virtual void WriteLineToConsole(string message)
        => Console.WriteLine(message);

    protected virtual void DisplayUserInputMessage(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey();
    }
}
