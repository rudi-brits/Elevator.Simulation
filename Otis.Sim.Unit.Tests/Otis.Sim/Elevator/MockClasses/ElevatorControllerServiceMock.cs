using AutoMapper;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Services;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.MockClasses;

public class ElevatorControllerServiceMock : ElevatorControllerService
{
    /// <summary>
    /// CalledUpdateRequestStatus
    /// </summary>
    public bool CalledUpdateRequestStatus = false;
    /// <summary>
    /// CalledRunRequestQueueConsumer
    /// </summary>
    public bool CalledRunRequestQueueConsumer = false;
    /// <summary>
    /// CalledValidateUserInputRequest
    /// </summary>
    public bool CalledValidateUserInputRequest = false;
    /// <summary>
    /// CalledValidateElevatorRequest
    /// </summary>
    public bool CalledValidateElevatorRequest = false;
    /// <summary>
    /// CalledRequestQueueProducer
    /// </summary>
    public bool CalledRequestQueueProducer = false;
    /// <summary>
    /// CalledValidateDuplicateElevatorRequest
    /// </summary>
    public bool CalledValidateDuplicateElevatorRequest = false;
    /// <summary>
    /// CalledRequestElevator
    /// </summary>
    public bool CalledRequestElevator = false;
    /// <summary>
    /// CalledRequestElevatorAssignment
    /// </summary>
    public bool CalledRequestElevatorAssignment = false;

    /// <summary>
    /// CallBaseRunRequestQueueConsumer
    /// </summary>
    public bool CallBaseRunRequestQueueConsumer = false;
    /// <summary>
    /// CallBaseValidateUserInputRequest
    /// </summary>
    public bool CallBaseValidateUserInputRequest = false;
    /// <summary>
    /// CallBaseValidateElevatorRequest
    /// </summary>
    public bool CallBaseValidateElevatorRequest = false;
    /// <summary>
    /// CallBaseValidateDuplicateElevatorRequest
    /// </summary>
    public bool CallBaseValidateDuplicateElevatorRequest = false;
    // <summary>
    /// CallBaseRequestQueueProducer
    /// </summary>
    public bool CallBaseRequestQueueProducer = false;
    // <summary>
    /// CallBaseRequestElevator
    /// </summary>
    public bool CallBaseRequestElevator = false;
    // <summary>
    /// CallBaseRequestElevator
    /// </summary>
    public bool CallBaseRequestElevatorAssignment = false;

    /// <summary>
    /// ValidateUserInputRequestReturnValue
    /// </summary>
    public ElevatorRequestResult? ValidateUserInputRequestReturnValue = null;
    /// <summary>
    /// ValidateElevatorRequestReturnValue
    /// </summary>
    public ElevatorRequestResult? ValidateElevatorRequestReturnValue = null;
    /// <summary>
    /// ValidateDuplicateElevatorRequestReturnValue
    /// </summary>
    public ElevatorRequestResult? ValidateDuplicateElevatorRequestReturnValue = null;
    /// <summary>
    /// RequestElevatorReturnValue
    /// </summary>
    public int? RequestElevatorReturnValue = null;
    /// <summary>
    /// CalledUpdateStatusMessage
    /// </summary>
    public string? CalledUpdateRequestStatusMessage;

    /// <summary>
    /// ElevatorControllerServiceMock constructor
    /// </summary>
    /// <param name="configurationService"></param>
    /// <param name="mapper"></param>
    public ElevatorControllerServiceMock(OtisConfigurationService configurationService, IMapper mapper)
        : base(configurationService, mapper)
    {
        UpdateRequestStatus = (string message) =>
        {
            CalledUpdateRequestStatus = true;
            CalledUpdateRequestStatusMessage = message;
        };
    }

    /// <summary>
    /// RunRequestQueueConsumer
    /// </summary>
    /// <returns></returns>
    protected override Task RunRequestQueueConsumer()
    {
        CalledRunRequestQueueConsumer = true;
        if (CallBaseRunRequestQueueConsumer)
            return base.RunRequestQueueConsumer();

        return Task.CompletedTask;
    }

    /// <summary>
    /// ValidateUserInputRequest
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override ElevatorRequestResult? ValidateUserInputRequest(UserInputRequest request)
    {
        CalledValidateUserInputRequest = true;
        if (CallBaseValidateUserInputRequest)
            return base.ValidateUserInputRequest(request);

        return ValidateUserInputRequestReturnValue;
    }

    /// <summary>
    /// ValidateElevatorRequest
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override ElevatorRequestResult? ValidateElevatorRequest(ElevatorRequest request)
    {
        CalledValidateElevatorRequest = true;
        if (CallBaseValidateElevatorRequest)
            return base.ValidateElevatorRequest(request);

        return ValidateElevatorRequestReturnValue;
    }

    /// <summary>
    /// RequestQueueProducer
    /// </summary>
    /// <param name="request"></param>
    protected override void RequestQueueProducer(ElevatorRequest request)
    {
        CalledRequestQueueProducer = true;
        if (CallBaseRequestQueueProducer)
            base.RequestQueueProducer(request);
    }

    /// <summary>
    /// ValidateDuplicateElevatorRequest
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override ElevatorRequestResult? ValidateDuplicateElevatorRequest(ElevatorRequest request)
    {
        CalledValidateDuplicateElevatorRequest = true;
        if (CallBaseValidateDuplicateElevatorRequest)
            return base.ValidateDuplicateElevatorRequest(request);

        return ValidateDuplicateElevatorRequestReturnValue;
    }

    /// <summary>
    /// RequestElevatorAssignment
    /// </summary>
    protected override void RequestElevatorAssignment()
    {
        CalledRequestElevatorAssignment = true;
        if (CallBaseRequestElevatorAssignment)
            base.RequestElevatorAssignment();
    }

    /// <summary>
    /// RequestElevator
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override int? RequestElevator(ElevatorRequest request)
    {
        CalledRequestElevator = true;
        if (CallBaseRequestElevator)
            return base.RequestElevator(request);

        return RequestElevatorReturnValue;
    }
}
