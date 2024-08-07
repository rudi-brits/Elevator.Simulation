using AutoMapper;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Utilities.Extensions;
using System.Diagnostics;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorModel : ElevatorConfigurationBase
    {
        public int Id { get; set; }
        public new int LowestFloor { get; set; }
        public new int HighestFloor { get; set; }
        public int CurrentFloor { get; set; } = 0;
        public int? NextFloor
        {
            get
            {
                if (_primaryDirectionQueue.Any())
                {
                    _isPrimaryDirection = true;

                    if (_currentStatus == ElevatorStatus.MovingUp)
                        return _primaryDirectionQueue.First();

                    return _primaryDirectionQueue.OrderByDescending(x => x).First();
                }
                if (_secondaryDirectionQueue.Any())
                {
                    _isPrimaryDirection = false;

                    if (_currentStatus == ElevatorStatus.MovingUp)
                        return _secondaryDirectionQueue.First();

                    return _secondaryDirectionQueue.OrderByDescending(x => x).First();
                }

                return null;
            }
        }
        public int CurrentLoad { get; set; } = 0;
        public int MaximumLoad { get; set; }
        public int Capacity => MaximumLoad - CurrentLoad;

        private ElevatorDirection? _elevatorDirection;
        private ElevatorDirection? _currentDirection
        {
            get
            {
                _elevatorDirection = CurrentStatus switch
                {
                    ElevatorStatus.Idle => null,
                    ElevatorStatus.MovingUp => ElevatorDirection.Up,
                    ElevatorStatus.MovingDown => ElevatorDirection.Down,
                    _ => _elevatorDirection
                };

                return _elevatorDirection;
            }
        }

        private ElevatorStatus _currentStatus { get; set; } = ElevatorStatus.Idle;
        public ElevatorStatus CurrentStatus => _currentStatus;

        private bool _isPrimaryDirection { get; set; } = true;
        private SortedSet<int> _primaryDirectionQueue = new SortedSet<int>();
        private SortedSet<int> _secondaryDirectionQueue = new SortedSet<int>();

        private List<ElevatorAcceptedRequest> _acceptedRequests = new List<ElevatorAcceptedRequest>();

        public int FloorMoveTime { get; set; } = 5000;
        public int DoorsOpenTime { get; set; } = 2500;

        private Timer _floorMoveTimer;
        private Timer _doorsOpenTimer;

        public delegate void CompleteRequestDelegate(Guid requestId);
        public CompleteRequestDelegate CompleteRequest;

        public delegate void RequeueRequestDelegate(Guid requestId);
        public RequeueRequestDelegate RequeueRequest;

        public delegate void PrintRequestStatusDelegate(string message);
        public PrintRequestStatusDelegate PrintRequestStatus;

        private readonly IMapper _mapper;

        public ElevatorModel(IMapper mapper)
        {
            _mapper = mapper;

            _floorMoveTimer = new Timer(MoveElevator, null, Timeout.Infinite, FloorMoveTime);
            _doorsOpenTimer = new Timer(CloseDoors, null, Timeout.Infinite, Timeout.Infinite);
        }

        public bool CanAcceptRequest(ElevatorRequest request)
        {
            return
                IsFloorInRange(request.OriginFloor) &&
                IsFloorInRange(request.DestinationFloor) &&
                IsFloorAndDirectionValid(request.OriginFloor, request.Direction);
        }

        private bool IsFloorInRange(int floor) =>
            floor.IsInRange(LowestFloor, HighestFloor);

        private bool IsFloorAndDirectionValid(int originFloor, ElevatorDirection direction)
        {
            var targetQueue = _isPrimaryDirection ? _primaryDirectionQueue : _secondaryDirectionQueue;
            var lastFloor = _currentStatus == ElevatorStatus.MovingUp
                ? targetQueue.LastOrDefault()
                : targetQueue.OrderByDescending(x => x).LastOrDefault();

            Debug.WriteLine($"currentFloor: {CurrentFloor}, originFloor: {originFloor}, direction: {direction}, lastFloor: {lastFloor}");

            if (Capacity == 0)
                return false;

            if (CurrentStatus == ElevatorStatus.Idle)
                return true;

            if (_currentDirection != direction)
                return false;

            if (direction == ElevatorDirection.Up)
                return originFloor > CurrentFloor;

            else if (direction == ElevatorDirection.Down)
                return originFloor < CurrentFloor;

            return false;
        }

        private bool IsSameDirectionOnRoute(int requestOriginFloor, ElevatorDirection requestDirection)
        {
            if (requestDirection == ElevatorDirection.Up)
                return requestOriginFloor > CurrentFloor;

            if (requestDirection == ElevatorDirection.Down)
                return requestOriginFloor < CurrentFloor;

            return false;
        }

        public bool AcceptRequest(ElevatorRequest request)
        {
            if (IsFloorAndDirectionValid(request.OriginFloor, request.Direction))
            {
                if (CurrentStatus == ElevatorStatus.Idle)
                {
                    var primaryDirectionDown = CurrentFloor > request.OriginFloor && request.Direction == ElevatorDirection.Down;
                    var primaryDirectionUp   = CurrentFloor < request.OriginFloor && request.Direction == ElevatorDirection.Up;

                    _primaryDirectionQueue.Add(request.OriginFloor);

                    if (primaryDirectionDown || primaryDirectionUp)
                        _primaryDirectionQueue.Add(request.DestinationFloor);
                    else
                        _secondaryDirectionQueue.Add(request.DestinationFloor);
                }
                else
                {
                    var targetQueue = _isPrimaryDirection ? _primaryDirectionQueue : _secondaryDirectionQueue;
                    targetQueue.Add(request.OriginFloor);
                    targetQueue.Add(request.DestinationFloor);
                }

                var acceptedRequest = _mapper.Map<ElevatorAcceptedRequest>(request);
                acceptedRequest.ElevatorName = Description;

                _acceptedRequests.Add(acceptedRequest);

                PrintRequestStatus(acceptedRequest.ToAcceptedRequestString());

                _floorMoveTimer.Change(0, FloorMoveTime);

                return true;
            }

            return false;
        }

        private void MoveElevator(object? state)
        {
            var nextFloor = NextFloor;
            if (nextFloor == null)
                return;

            if (nextFloor > CurrentFloor)
            {
                _currentStatus = ElevatorStatus.MovingUp;
                CurrentFloor++;
            }
            else if (nextFloor < CurrentFloor)
            {
                _currentStatus = ElevatorStatus.MovingDown;
                CurrentFloor--;
            }
            else if (CurrentFloor == nextFloor)
            {
                if (_isPrimaryDirection)
                    _primaryDirectionQueue.Remove((int)nextFloor);
                else
                    _secondaryDirectionQueue.Remove((int)nextFloor);

                _floorMoveTimer.Change(Timeout.Infinite, Timeout.Infinite);

                OpenDoors();
            }
        }

        private void OpenDoors()
        {
            _currentStatus = ElevatorStatus.DoorsOpen;
            _doorsOpenTimer.Change(DoorsOpenTime, Timeout.Infinite);

            var destinationRequest = _acceptedRequests
                .Where(request => request.DestinationFloor == CurrentFloor)
                .FirstOrDefault();

            if (destinationRequest != null)
            {
                destinationRequest.DestinationFloorServiced = true;
                CurrentLoad -= destinationRequest.NumberOfPeople;

                var statusMessage = destinationRequest.ToDroppedOffRequestString(
                    destinationRequest.NumberOfPeople, Capacity);

                PrintRequestStatus(statusMessage);
                HandleCompletedRequest(destinationRequest);
            }

            var originRequest = _acceptedRequests
                .Where(request => request.OriginFloor == CurrentFloor)
                .FirstOrDefault();

            if (originRequest != null)
            {
                originRequest.OriginFloorServiced = true;

                if (Capacity == 0)
                {
                    HandleRequeueRequest(originRequest);
                    return;
                }

                if (Capacity < originRequest.NumberOfPeople)
                    originRequest.NumberOfPeople = Capacity;

                CurrentLoad += originRequest.NumberOfPeople;

                var statusMessage = originRequest.ToPickedUpRequestString(
                   originRequest.NumberOfPeople, Capacity);

                PrintRequestStatus(statusMessage);
                HandleCompletedRequest(originRequest);
            }
        }

        private void CloseDoors(object? state)
        {
            _doorsOpenTimer.Change(Timeout.Infinite, Timeout.Infinite);
            MoveToNextFloor();
        }

        private void MoveToNextFloor()
        {
            if (NextFloor == null)
            {
                _currentStatus = ElevatorStatus.Idle;
                _floorMoveTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
                _floorMoveTimer.Change(0, FloorMoveTime);
        }

        private void HandleCompletedRequest(ElevatorAcceptedRequest request)
        {
            if (request.Completed)
            {
                RemoveAcceptedRequest(request.Id);

                CompleteRequest.Invoke(request.Id);
                PrintRequestStatus(request.ToCompletedRequestString());
            }
        }

        private void HandleRequeueRequest(ElevatorAcceptedRequest request)
        {
            RemoveAcceptedRequest(request.Id);

            RequeueRequest.Invoke(request.Id);
            PrintRequestStatus(request.ToRequeuedRequestString());
        }

        private void RemoveAcceptedRequest(Guid id)
        {
            _acceptedRequests = _acceptedRequests
               .Where(request => request.Id != id)
               .ToList();
        }
    }
}
