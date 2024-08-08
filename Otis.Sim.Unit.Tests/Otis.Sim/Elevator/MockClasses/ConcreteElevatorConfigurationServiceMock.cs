using AutoMapper;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Services;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.MockClasses;

/// <summary>
/// The ConcreteElevatorConfigurationServiceMock class extends the <see cref="ElevatorConfigurationService" /> class.
/// </summary>
public class ConcreteElevatorConfigurationServiceMock : ElevatorConfigurationService
{
    /// <summary>
    /// CallBasePrintLoadedConfiguration
    /// </summary>
    public bool CallBasePrintLoadedConfiguration = false;

    /// <summary>
    /// CalledPrintLoadedConfiguration
    /// </summary>
    public bool CalledPrintLoadedConfiguration = false;

    /// <summary>
    /// ConcreteElevatorConfigurationServiceMock constructor
    /// </summary>
    /// <param name="configurationService"></param>
    /// <param name="mapper"></param>
    public ConcreteElevatorConfigurationServiceMock(OtisConfigurationService configurationService, 
        IMapper mapper)
        : base(configurationService, mapper)
    {
    }

    /// <summary>
    /// CompleteRequest function
    /// </summary>
    /// <param name="requestId"></param>
    protected override void CompleteRequest(Guid requestId)
    {
    }

    /// <summary>
    /// RequeueRequest function
    /// </summary>
    /// <param name="requestId"></param>
    protected override void RequeueRequest(Guid requestId)
    {
    }

    /// <summary>
    /// PrintRequestStatus function
    /// </summary>
    /// <param name="requestId"></param>
    protected override void PrintRequestStatus(string message)
    {
    }

    /// <summary>
    /// PrintLoadedConfiguration function
    /// </summary>
    protected override void PrintLoadedConfiguration()
    {
        CalledPrintLoadedConfiguration = true;
        if (CallBasePrintLoadedConfiguration)
            base.PrintLoadedConfiguration();
    }
}