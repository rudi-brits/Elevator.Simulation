using AutoMapper;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Validators;
using Otis.Sim.Utilities.Helpers;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Elevator.Services
{
    public class ElevatorControllerService : ElevatorConfigurationService
    {
        public List<string> ElevatorTableHeaders => ReflectionHelper.GetFormattedPropertyNames<ElevatorDataRow>();
        public List<ElevatorDataRow> ElevatorDataRows => _mapper.Map<List<ElevatorDataRow>>(_elevators);

        public string StatusFieldName => nameof(ElevatorDataRow.Status);

        private Queue<ElevatorRequest> _requestQueue;
        private HashSet<Guid> _completedRequestIds;
        private readonly object _lockRequestQueue;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _requestConsumerTask;

        public delegate void UpdateRequestStatusDelegate(string message);
        public UpdateRequestStatusDelegate UpdateRequestStatus;

        private readonly IMapper _mapper;

        public ElevatorControllerService(OtisConfigurationService configurationService,
            IMapper mapper): base(configurationService)
        {   
            _mapper = mapper;

            _requestQueue = new Queue<ElevatorRequest>();
            _completedRequestIds = new HashSet<Guid>();
            _lockRequestQueue = new object();

            _cancellationTokenSource = new CancellationTokenSource();
            _requestConsumerTask = Task.Run(() => RequestQueueConsumer(_cancellationTokenSource.Token));
        }

        public void Exit()
        {
            _cancellationTokenSource.Cancel();
            _requestConsumerTask.Wait();
        }

        public ElevatorRequestResult RequestElevator(UserInputRequest userInputRequest)
        {
            var userInputValidationResult = ValidateUserInputRequest(userInputRequest);
            if (userInputValidationResult != null)
                return userInputValidationResult;

            var elevatorRequest = new ElevatorRequest(userInputRequest);

            return ValidateElevatorRequest(elevatorRequest)
                ?? ValidateDuplicateElevatorRequest(elevatorRequest)
                ?? new ElevatorRequestResult(true);
        }

        private ElevatorRequestResult? ValidateUserInputRequest(UserInputRequest request)
        {
            var validationResult = new UserInputRequestValidator()
                .Validate(request);

            if (!validationResult.IsValid)
                return new ElevatorRequestResult(validationResult.Errors);

            return null;
        }

        private ElevatorRequestResult? ValidateElevatorRequest(ElevatorRequest request)
        {
            var validationResult = new ElevatorRequestValidator(_elevatorRequestValidationValues)
                .Validate(request);

            if (!validationResult.IsValid)
                return new ElevatorRequestResult(validationResult.Errors);

            return null;
        }

        private ElevatorRequestResult? ValidateDuplicateElevatorRequest(ElevatorRequest request)
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
                    request.ToDuplicateRequestString());

                return new ElevatorRequestResult(false, message);
            }

            RequestQueueProducer(request);

            return null;
        }
        private void RequestQueueProducer(ElevatorRequest request)
        {
            lock (_lockRequestQueue)
            {
                _requestQueue.Enqueue(request);
            }

            PrintRequestStatus(request.ToQueuedRequestString());
        }

        private void RequestQueueConsumer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                lock (_lockRequestQueue)
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
                                    //var elevator = AssignElevator(request);
                                    //if (elevator != null)
                                    //{
                                    //    request.ElevatorId = elevator.Id;

                                    //    //UpdateQueueStatus.Invoke($"Request Id: {request.Id} " +
                                    //    //    $"assigned to {elevator.Description}");
                                    //}
                                }

                                _requestQueue.Enqueue(request);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        PrintRequestStatus($"Error processing request {exc}");
                    }

                    Thread.Sleep(150);
                }
            }
        }

        private void PrintRequestStatus(string message)
        {
            UpdateRequestStatus?.Invoke($"{DateTime.Now} - {message}");
        }

        public override void CompleteRequest(Guid requestId)
        {
            lock (_lockRequestQueue)
            {
                if (!_completedRequestIds.Any(id => id == requestId))
                    _completedRequestIds.Add(requestId);
            }
        }
    }
}
