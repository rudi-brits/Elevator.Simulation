using AutoMapper;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Elevator.Services;

/// <summary>
/// ElevatorConfigurationService class
/// </summary>
public abstract class ElevatorConfigurationService
{
    /// <summary>
    /// _elevators
    /// </summary>
    protected List<ElevatorModel> _elevators = new();
    /// <summary>
    /// _elevatorRequestValidationValues
    /// </summary>
    protected ElevatorRequestValidationValues? _elevatorRequestValidationValues;

    /// <summary>
    /// _configurationService
    /// </summary>
    protected OtisConfigurationService _configurationService;
    /// <summary>
    /// _mapper
    /// </summary>
    protected IMapper _mapper;

    /// <summary>
    /// ElevatorConfigurationService constructor
    /// </summary>
    /// <param name="configurationService"></param>
    /// <param name="mapper"></param>
    protected ElevatorConfigurationService(OtisConfigurationService configurationService,
        IMapper mapper)
    {
        _configurationService = configurationService;
        _mapper = mapper;
    }

    /// <summary>
    /// LoadConfiguration function
    /// </summary>
    public virtual void LoadConfiguration()
    {
        var elevatorId = 0;
        var buildingConfiguration = _configurationService.BuildingConfiguration!;

        _configurationService.ElevatorsConfiguration!.ForEach(elevatorConfiguration =>
        {
            _elevators.Add(new ElevatorModel(_mapper)
            {
                Id                 = ++elevatorId,
                Description        = elevatorConfiguration.Description.Trim(),
                LowestFloor        = elevatorConfiguration.LowestFloor.ApplyHigherValue(buildingConfiguration.LowestFloor),
                HighestFloor       = elevatorConfiguration.HighestFloor.ApplyLowerValue(buildingConfiguration.HighestFloor),
                MaximumLoad        = buildingConfiguration.MaximumElevatorLoad,
                CompleteRequest    = CompleteRequest,
                RequeueRequest     = RequeueRequest,
                PrintRequestStatus = PrintRequestStatus
            });
        });

        _elevatorRequestValidationValues = new ElevatorRequestValidationValues()
        {
            LowestFloor  = _elevators.Min(elevator => elevator.LowestFloor),
            HighestFloor = _elevators.Max(elevator => elevator.HighestFloor),
            MaximumLoad  = buildingConfiguration.MaximumElevatorLoad
        };

        PrintLoadedConfiguration();
    }

    /// <summary>
    /// CompleteRequest function
    /// </summary>
    protected abstract void CompleteRequest(Guid requestId);
    /// <summary>
    /// RequeueRequest function
    /// </summary>
    protected abstract void RequeueRequest(Guid requestId);
    /// <summary>
    /// PrintRequestStatus function
    /// </summary>
    protected abstract void PrintRequestStatus(string message);

    /// <summary>
    /// PrintLoadedConfiguration
    /// </summary>
    protected virtual void PrintLoadedConfiguration()
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
}
