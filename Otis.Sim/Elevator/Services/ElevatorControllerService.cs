using AutoMapper;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Validators;
using Otis.Sim.Utilities.Helpers;
using System.Diagnostics;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Elevator.Services;

/// <summary>
/// Class ElevatorControllerService extends the <see cref="ElevatorConfigurationService" /> class.
/// </summary>
public class ElevatorControllerService : ElevatorConfigurationService
{
    /// <summary>
    /// ElevatorTableHeaders
    /// </summary>
    public List<string> ElevatorTableHeaders => ReflectionHelper.GetFormattedPropertyNames<ElevatorDataRow>();
    /// <summary>
    /// ElevatorDataRows
    /// </summary>
    public List<ElevatorDataRow> ElevatorDataRows => _mapper.Map<List<ElevatorDataRow>>(_elevators);
    /// <summary>
    /// StatusFieldName
    /// </summary>
    public string StatusFieldName => nameof(ElevatorDataRow.Status);
    /// <summary>
    /// _requestQueue
    /// </summary>
    protected Queue<ElevatorRequest> _requestQueue;
    /// <summary>
    /// _completedRequestIds
    /// </summary>
    protected HashSet<Guid> _completedRequestIds;
    /// <summary>
    /// _lockRequestQueue
    /// </summary>
    protected readonly object _lockRequestQueue;
    /// <summary>
    /// _cancellationTokenSource
    /// </summary>
    protected CancellationTokenSource _cancellationTokenSource;
    /// <summary>
    /// _requestConsumerTask
    /// </summary>
    protected Task _requestConsumerTask;
    /// <summary>
    /// UpdateRequestStatusDelegate type
    /// </summary>
    /// <param name="message"></param>
    public delegate void UpdateRequestStatusDelegate(string message);
    /// <summary>
    /// UpdateRequestStatus
    /// </summary>
    public UpdateRequestStatusDelegate? UpdateRequestStatus;

    /// <summary>
    /// ElevatorControllerService constructor
    /// </summary>
    /// <param name="configurationService"></param>
    /// <param name="mapper"></param>
    public ElevatorControllerService(OtisConfigurationService configurationService,
        IMapper mapper): base(configurationService, mapper)
    {   
        _mapper = mapper;

        _requestQueue        = new Queue<ElevatorRequest>();
        _completedRequestIds = new HashSet<Guid>();
        _lockRequestQueue    = new object();

        _cancellationTokenSource = new CancellationTokenSource();
        _requestConsumerTask     = RunRequestQueueConsumer();
    }

    /// <summary>
    /// RunRequestQueueConsumer
    /// </summary>
    /// <returns></returns>
    protected virtual Task RunRequestQueueConsumer()
        => Task.Run(() => RequestQueueConsumer());

    /// <summary>
    /// Exit
    /// </summary>
    public void Exit()
    {
        _cancellationTokenSource.Cancel();
        _requestConsumerTask.Wait();
    }

    /// <summary>
    /// RequestElevator
    /// </summary>
    /// <param name="userInputRequest"></param>
    /// <returns></returns>
    public virtual ElevatorRequestResult RequestElevator(UserInputRequest userInputRequest)
    {
        var userInputValidationResult = ValidateUserInputRequest(userInputRequest);
        if (userInputValidationResult != null)
            return userInputValidationResult;

        var elevatorRequest = new ElevatorRequest(userInputRequest);

        return ValidateElevatorRequest(elevatorRequest)
            ?? ValidateDuplicateElevatorRequest(elevatorRequest)
            ?? new ElevatorRequestResult(true);
    }

    /// <summary>
    /// ValidateUserInputRequest
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected virtual ElevatorRequestResult? ValidateUserInputRequest(UserInputRequest request)
    {
        var validationResult = new UserInputRequestValidator()
            .Validate(request);

        if (!validationResult.IsValid)
            return new ElevatorRequestResult(validationResult.Errors);

        return null;
    }

    /// <summary>
    /// ValidateElevatorRequest
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected virtual ElevatorRequestResult? ValidateElevatorRequest(ElevatorRequest request)
    {
        var validationResult = new ElevatorRequestValidator(_elevatorRequestValidationValues!)
            .Validate(request);

        if (!validationResult.IsValid)
            return new ElevatorRequestResult(validationResult.Errors);

        return null;
    }

    /// <summary>
    /// ValidateDuplicateElevatorRequest
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected virtual ElevatorRequestResult? ValidateDuplicateElevatorRequest(ElevatorRequest request)
    {
        var isAlreadyQueued = _requestQueue.Any(queueRequest =>
            queueRequest.OriginFloor      == request.OriginFloor &&
            queueRequest.DestinationFloor == request.DestinationFloor &&
            queueRequest.Direction        == request.Direction &&
            queueRequest.RequestStatus    == RequestStatus.Pending);

        if (isAlreadyQueued)
        {
            var message = MessageService.FormatMessage(
                MessageService.DuplicateElevatorRequest,
                request.OriginFloor.ToString(),
                request.DestinationFloor.ToString(),
                request.Direction.ToString());

            return new ElevatorRequestResult(false, message);
        }

        RequestQueueProducer(request);

        return null;
    }

    /// <summary>
    /// RequestQueueProducer
    /// </summary>
    /// <param name="request"></param>
    protected virtual void RequestQueueProducer(ElevatorRequest request)
    {
        lock (_lockRequestQueue)
        {
            _requestQueue.Enqueue(request);
        }

        PrintRequestStatus(request.ToQueuedRequestString());
    }

    /// <summary>
    /// RequestQueueConsumer
    /// </summary>
    protected virtual void RequestQueueConsumer()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            lock (_lockRequestQueue)
            {
                RequestElevatorAssignment();
                Thread.Sleep(150);
            }
        }
    }

    /// <summary>
    /// RequestElevatorAssignment
    /// </summary>
    protected virtual void RequestElevatorAssignment()
    {
        try
        {
            if (_requestQueue.Count > 0 && _requestQueue.TryDequeue(out ElevatorRequest? request))
            {
                if (_completedRequestIds.Any(id => id == request.Id))
                    _completedRequestIds.Remove(request.Id);

                else
                {
                    if (request.RequestStatus == RequestStatus.Pending)
                    {
                        var elevatorId = RequestElevator(request);
                        if (elevatorId != null)
                        {
                            request.ElevatorId = elevatorId;
                        }
                    }

                    _requestQueue.Enqueue(request);
                }
            }
        }
        catch (Exception exc)
        {
            PrintRequestStatus($"{OtisSimConstants.ErrorProcessingRequest} {exc}");
        }
    }

    protected virtual int? RequestElevator(ElevatorRequest request)
    {
        var elevator = _elevators
            .Where
            (
                elevator => elevator.CanAcceptRequest(request)
            )
            .OrderBy(elevator => Math.Abs(elevator.CurrentFloor - request.OriginFloor))
            .FirstOrDefault();

        if (elevator != null && elevator.AcceptRequest(request))
            return elevator.Id;

        return null;
    }

    protected override void CompleteRequest(Guid requestId)
    {
        lock (_lockRequestQueue)
        {
            if (!_completedRequestIds.Any(id => id == requestId))
                _completedRequestIds.Add(requestId);
        }
    }

    protected override void RequeueRequest(Guid requestId)
    {
        lock (_lockRequestQueue)
        {
            var request = _requestQueue
                .FirstOrDefault(request => request.Id == requestId);

            if (request != null)
                request.ElevatorId = null;
        }
    }

    protected override void PrintRequestStatus(string message)
    {
        UpdateRequestStatus?.Invoke($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")} - {message}");
    }
}
