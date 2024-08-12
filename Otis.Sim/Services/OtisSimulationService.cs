using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Constants;
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
            WriteLineToConsole($"{OtisSimConstants.ApplicationStartupFailedMessage} {exc.Message}");
            
            if (!string.IsNullOrWhiteSpace(exc.InnerException?.Message))
            {
                WriteLineToConsole(exc.InnerException.Message);
            }
        }

        if (initialisedSuccess)
        {
            DisplayUserInputMessage(OtisSimConstants.PressAnyKeyToContinueMessage);
            InitialiseTerminalUi();
        } 
        else
            DisplayUserInputMessage(OtisSimConstants.PressAnyKeyToExitMessage);
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

    /// <summary>
    /// LoadAppConfiguration
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected virtual void LoadAppConfiguration()
    {
        try
        {
            var otisConfigurationService = _serviceProvider!.GetService<OtisConfigurationService>();
            otisConfigurationService!.LoadConfiguration();

            WriteLineToConsole(LoadAppConfigurationMessage());
        }
        catch (Exception exc)
        {
            throw new Exception(LoadAppConfigurationMessage(false), exc);
        }
    }

    /// <summary>
    /// LoadElevatorControllerConfiguration
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected virtual void LoadElevatorControllerConfiguration()
    {
        try
        {
            var elevatorControllerService = _serviceProvider!.GetService<ElevatorControllerService>();
            elevatorControllerService!.LoadConfiguration();

            WriteLineToConsole(LoadElevatorControllerConfigurationMessage());
        }
        catch (Exception exc)
        {
            throw new Exception(LoadElevatorControllerConfigurationMessage(false), exc);
        }
    }

    /// <summary>
    /// InitialiseTerminalUi
    /// </summary>
    protected virtual void InitialiseTerminalUi()
    {
        try
        { 
            var terminalUiService = _serviceProvider!.GetService<TerminalUiService>();
            terminalUiService!.InitialiseUi();
        }
        catch (Exception exc)
        {
            DisplayUserInputMessage($"{OtisSimConstants.ErrorInitialiseTerminalUi} {exc.Message}");
        }
    }

    /// <summary>
    /// SetupServiceCollectionMessage
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <returns></returns>
    protected virtual string SetupServiceCollectionMessage(bool isSuccess = true)
        => $"{GetPrefix(isSuccess)} {OtisSimConstants.SetupServiceCollectionResultMessage}";

    /// <summary>
    /// LoadAppConfigurationMessage
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <returns></returns>
    protected virtual string LoadAppConfigurationMessage(bool isSuccess = true)
        => $"{GetPrefix(isSuccess)} {OtisSimConstants.LoadAppConfigurationResultMessage}";

    /// <summary>
    /// LoadElevatorControllerConfigurationMessage
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <returns></returns>
    protected virtual string LoadElevatorControllerConfigurationMessage(bool isSuccess = true)
        => $"{GetPrefix(isSuccess)} {OtisSimConstants.LoadElevatorConfigurationResultMessage}";

    /// <summary>
    /// GetPrefix
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <returns></returns>
    protected virtual string GetPrefix(bool isSuccess = true)
        => isSuccess ? SuccessPrefix : "";

    /// <summary>
    /// WriteLineToConsole
    /// </summary>
    /// <param name="message"></param>
    protected virtual void WriteLineToConsole(string message)
        => Console.WriteLine(message);

    /// <summary>
    /// DisplayUserInputMessage
    /// </summary>
    /// <param name="message"></param>
    protected virtual void DisplayUserInputMessage(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey();
    }
}
