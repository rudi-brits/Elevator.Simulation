using AutoMapper;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Utilities.Extensions;
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
                    isPrimaryDirection = true;
                    return _primaryDirectionQueue.First();
                }
                if (_secondaryDirectionQueue.Any())
                {
                    isPrimaryDirection = false;
                    return _secondaryDirectionQueue.First();
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

        private bool isPrimaryDirection { get; set; } = true;
        private SortedSet<int> _primaryDirectionQueue = new SortedSet<int>();
        private SortedSet<int> _secondaryDirectionQueue = new SortedSet<int>();

        private List<ElevatorAcceptedRequest> _acceptedRequests = new List<ElevatorAcceptedRequest>();

        public int FloorMoveTime { get; set; } = 2000;
        public int DoorsOpenTime { get; set; } = 2500;

        private Timer _floorMoveTimer;
        private Timer _doorsOpenTimer;

        public delegate void CompleteRequestDelegate(Guid requestId);
        public CompleteRequestDelegate CompleteRequest;

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
            var lastFloor = isPrimaryDirection
                ? _primaryDirectionQueue.LastOrDefault()
                : _secondaryDirectionQueue.LastOrDefault();

            if (Capacity == 0)
                return false;

            if (CurrentStatus == ElevatorStatus.Idle)
                return true;

            if (_currentDirection != direction)
                return false;

            if (direction == ElevatorDirection.Up)
                return originFloor > CurrentFloor && originFloor <= lastFloor;

            else if (direction == ElevatorDirection.Down)
                return originFloor < CurrentFloor && originFloor >= lastFloor;

            return false;
        }

        public bool AcceptRequest(ElevatorRequest request)
        {
            if (IsFloorAndDirectionValid(request.OriginFloor, request.Direction))
            {
                if (CurrentStatus == ElevatorStatus.Idle)
                {
                    var primaryDirectionDown = CurrentFloor > request.OriginFloor && request.DestinationFloor < CurrentFloor;
                    var primaryDirectionUp = CurrentFloor < request.OriginFloor && request.DestinationFloor > request.OriginFloor;

                    _primaryDirectionQueue.Add(request.OriginFloor);

                    if (primaryDirectionDown || primaryDirectionUp)
                        _primaryDirectionQueue.Add(request.DestinationFloor);

                    else
                        _secondaryDirectionQueue.Add(request.DestinationFloor);
                }
                else
                {
                    var targetQueue = isPrimaryDirection ? _primaryDirectionQueue : _secondaryDirectionQueue;
                    targetQueue.Add(request.OriginFloor);
                    targetQueue.Add(request.DestinationFloor);
                }

                // Map here
                var acceptedRequest = _mapper.Map<ElevatorAcceptedRequest>(request);
                _acceptedRequests.Add(acceptedRequest);

                PrintRequestStatus(acceptedRequest.ToAcceptedRequestString());

                _floorMoveTimer.Change(0, FloorMoveTime);

                return true;
            }

            return false;
        }

        private void MoveElevator(object? state)
        {

        }

        private void CloseDoors(object? state)
        {

        }
    }
}
