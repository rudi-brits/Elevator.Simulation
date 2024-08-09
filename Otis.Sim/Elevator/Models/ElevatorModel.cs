using AutoMapper;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Utilities.Extensions;
using System.Diagnostics;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorModel extends the <see cref="ElevatorConfigurationBase" /> class.
/// </summary>
public class ElevatorModel : ElevatorConfigurationBase
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// LowestFloor
    /// </summary>
    public int LowestFloor { get; set; }
    /// <summary>
    /// HighestFloor
    /// </summary>
    public int HighestFloor { get; set; }
    /// <summary>
    /// CurrentFloor
    /// </summary>
    public int CurrentFloor { get; set; }
    /// <summary>
    /// NextFloor
    /// </summary>
    public int? NextFloor
    {
        get
        {
            _isPrimaryDirection = _primaryDirectionQueue.Any();
            if (_currentDirectionQueue.Any())
            {
                return _currentStatus == ElevatorStatus.MovingDown
                    ? _currentDirectionQueue.OrderByDescending(x => x).First()
                    : _currentDirectionQueue.First();
            }

            return null;
        }
    }
    /// <summary>
    /// LastFloor
    /// </summary>
    public int? LastFloor
    {
        get
        {
            _isPrimaryDirection = _primaryDirectionQueue.Any();
            return _currentStatus == ElevatorStatus.MovingUp
                ? _currentDirectionQueue.LastOrDefault()
                : _currentDirectionQueue.OrderByDescending(x => x).LastOrDefault();
        }
    }

    /// <summary>
    /// CurrentLoad
    /// </summary>
    public int CurrentLoad { get; set; }
    /// <summary>
    /// MaximumLoad
    /// </summary>
    public int MaximumLoad { get; set; }
    /// <summary>
    /// Capacity
    /// </summary>
    public int Capacity => MaximumLoad - CurrentLoad;
    /// <summary>
    /// FloorMoveTime
    /// </summary>
    public int FloorMoveTime { get; set; } = 4500;
    /// <summary>
    /// DoorsOpenTime
    /// </summary>
    public int DoorsOpenTime { get; set; } = 2500;
    /// <summary>
    /// CompleteRequestDelegate definition
    /// </summary>
    /// <param name="requestId"></param>
    public delegate void CompleteRequestDelegate(Guid requestId);
    /// <summary>
    /// CompleteRequest
    /// </summary>
    public CompleteRequestDelegate? CompleteRequest;
    /// <summary>
    /// RequeueRequestDelegate definition
    /// </summary>
    /// <param name="requestId"></param>
    public delegate void RequeueRequestDelegate(Guid requestId);
    /// <summary>
    /// RequeueRequest
    /// </summary>
    public RequeueRequestDelegate? RequeueRequest;
    /// <summary>
    /// PrintRequestStatusDelegate definition
    /// </summary>
    /// <param name="message"></param>
    public delegate void PrintRequestStatusDelegate(string message);
    /// <summary>
    /// PrintRequestStatus
    /// </summary>
    public PrintRequestStatusDelegate? PrintRequestStatus;
    /// <summary>
    /// _isMoving
    /// </summary>
    protected bool _isMoving = false;
    /// <summary>
    /// _currentStatus
    /// </summary>
    protected ElevatorStatus _currentStatus { get; set; } = ElevatorStatus.Idle;
    /// <summary>
    /// CurrentStatus
    /// </summary>
    public ElevatorStatus CurrentStatus => _currentStatus;
    /// <summary>
    /// _elevatorDirection
    /// </summary>
    protected ElevatorDirection? _elevatorDirection;
    /// <summary>
    /// _currentDirection
    /// </summary>
    protected ElevatorDirection? _currentDirection
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
    /// <summary>
    /// _isPrimaryDirection
    /// </summary>
    protected bool _isPrimaryDirection { get; set; } = true;
    /// <summary>
    /// _primaryDirectionQueue
    /// </summary>
    protected SortedSet<int> _primaryDirectionQueue = new SortedSet<int>();
    /// <summary>
    /// _secondaryDirectionQueue
    /// </summary>
    protected SortedSet<int> _secondaryDirectionQueue = new SortedSet<int>();
    /// <summary>
    /// _currentDirectionQueue
    /// </summary>
    protected SortedSet<int> _currentDirectionQueue
        => _isPrimaryDirection ? _primaryDirectionQueue : _secondaryDirectionQueue;
    /// <summary>
    /// _acceptedRequests
    /// </summary>
    protected List<ElevatorAcceptedRequest> _acceptedRequests = new List<ElevatorAcceptedRequest>();
    /// <summary>
    /// _mapper
    /// </summary>
    protected readonly IMapper _mapper;
    /// <summary>
    /// _floorMoveTimer
    /// </summary>
    protected readonly Timer _floorMoveTimer;
    /// <summary>
    /// _doorsOpenTimer
    /// </summary>
    protected readonly Timer _doorsOpenTimer;

    /// <summary>
    /// ElevatorModel constructor
    /// </summary>
    /// <param name="mapper"></param>
    public ElevatorModel(IMapper mapper)
    {
        _mapper = mapper;

        _floorMoveTimer = new Timer(InitiateElevatorMove, null, Timeout.Infinite, FloorMoveTime);
        _doorsOpenTimer = new Timer(CloseDoors, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// ElevatorModel constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="floorMoveTimer"></param>
    /// <param name="doorsOpenTimer"></param>
    public ElevatorModel(IMapper mapper, Timer floorMoveTimer, Timer doorsOpenTimer)
    {
        _mapper = mapper;

        _floorMoveTimer = floorMoveTimer;
        _doorsOpenTimer = doorsOpenTimer;
    }

    /// <summary>
    /// CanAcceptRequest
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public bool CanAcceptRequest(ElevatorRequest request)
    {
        return
            IsFloorInRange(request.OriginFloor) &&
            IsFloorInRange(request.DestinationFloor) &&
            IsFloorAndDirectionValid(request.OriginFloor, request.Direction);
    }

    /// <summary>
    /// IsFloorInRange function
    /// </summary>
    /// <param name="floor"></param>
    /// <returns></returns>
    protected virtual bool IsFloorInRange(int floor) =>
        floor.IsInRange(LowestFloor, HighestFloor);

    /// <summary>
    /// IsSameDirectionOnRoute function
    /// </summary>
    /// <param name="requestOriginFloor"></param>
    /// <param name="requestDirection"></param>
    /// <returns></returns>
    protected virtual bool IsSameDirectionOnRoute(int requestOriginFloor, ElevatorDirection requestDirection)
    {
        if (requestDirection == ElevatorDirection.Up)
            return requestOriginFloor > CurrentFloor;

        if (requestDirection == ElevatorDirection.Down)
            return requestOriginFloor < CurrentFloor;

        return false;
    }

    /// <summary>
    /// IsFloorAndDirectionValid function
    /// </summary>
    /// <param name="originFloor"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual bool IsFloorAndDirectionValid(int originFloor, ElevatorDirection direction)
    {
        Debug.WriteLine($"currentFloor: {CurrentFloor}, originFloor: {originFloor}, direction: {direction}, " +
            "lastFloor: {LastFloor}");

        if (Capacity == 0)
            return false;

        if (CurrentStatus == ElevatorStatus.Idle)
            return true;

        if (_currentDirection != direction)
            return false;

        return IsSameDirectionOnRoute(originFloor, direction);
    }

    public bool AcceptRequest(ElevatorRequest request)
    {
        if (IsFloorAndDirectionValid(request.OriginFloor, request.Direction))
        {
            if (CurrentStatus == ElevatorStatus.Idle)
            {
                _primaryDirectionQueue.Add(request.OriginFloor);

                if (IsSameDirectionOnRoute(request.OriginFloor, request.Direction))
                    _primaryDirectionQueue.Add(request.DestinationFloor);
                else
                    _secondaryDirectionQueue.Add(request.DestinationFloor);
            }
            else
            {
                _currentDirectionQueue.Add(request.OriginFloor);
                _currentDirectionQueue.Add(request.DestinationFloor);
            }

            var acceptedRequest = _mapper.Map<ElevatorAcceptedRequest>(request);
            acceptedRequest.ElevatorName = Description;

            _acceptedRequests.Add(acceptedRequest);

            PrintRequestStatus!(acceptedRequest.ToAcceptedRequestString());

            if (!_isMoving)
                MoveElevator();

            return true;
        }

        return false;
    }

    protected virtual void InitiateElevatorMove(object? state)
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

            StopElevator();
            OpenDoors();
        }
    }

    protected virtual void OpenDoors()
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

            PrintRequestStatus!(statusMessage);
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

            PrintRequestStatus!(statusMessage);
            HandleCompletedRequest(originRequest);
        }
    }

    protected virtual void CloseDoors(object? state)
    {
        _doorsOpenTimer.Change(Timeout.Infinite, Timeout.Infinite);
        MoveToNextFloor();
    }

    protected virtual void MoveToNextFloor()
    {
        if (NextFloor == null)
        {
            _currentStatus = ElevatorStatus.Idle;
            StopElevator();
        }
        else
            MoveElevator();
    }

    protected virtual void MoveElevator()
    {
        _floorMoveTimer.Change(0, FloorMoveTime);
        _isMoving = true;
    }

    protected virtual void StopElevator()
    {
        _floorMoveTimer.Change(Timeout.Infinite, Timeout.Infinite);
        _isMoving = false;
    }

    protected virtual void HandleCompletedRequest(ElevatorAcceptedRequest request)
    {
        if (request.Completed)
        {
            RemoveAcceptedRequest(request.Id);

            CompleteRequest!.Invoke(request.Id);
            PrintRequestStatus!(request.ToCompletedRequestString());
        }
    }

    protected virtual void HandleRequeueRequest(ElevatorAcceptedRequest request)
    {
        RemoveAcceptedRequest(request.Id);

        RequeueRequest!.Invoke(request.Id);
        PrintRequestStatus!(request.ToRequeuedRequestString());
    }

    protected virtual void RemoveAcceptedRequest(Guid id)
    {
        _acceptedRequests = _acceptedRequests
           .Where(request => request.Id != id)
           .ToList();
    }

    public override string ToString()
    {
        return $"{nameof(Description)}: {Description}, " +
            $"{nameof(LowestFloor)}: {LowestFloor}, " +
            $"{nameof(HighestFloor)}: {HighestFloor}, " +
            $"{nameof(MaximumLoad)}: {MaximumLoad}";
    }
}
