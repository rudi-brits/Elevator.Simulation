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

        public ElevatorControllerService(OtisConfigurationService configurationService,
            IMapper mapper): base(configurationService, mapper)
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
                        PrintRequestStatus($"Error processing request {exc}");
                    }

                    Thread.Sleep(150);
                }
            }
        }

        private int? RequestElevator(ElevatorRequest request)
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
}
