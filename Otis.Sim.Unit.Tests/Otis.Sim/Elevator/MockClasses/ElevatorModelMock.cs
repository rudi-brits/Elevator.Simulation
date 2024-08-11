using AutoMapper;
using Otis.Sim.Elevator.Models;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.MockClasses;

public class ElevatorModelMock : ElevatorModel
{
    /// <summary>
    /// CalledIsFloorInRange field.
    /// </summary>
    public bool CalledIsFloorInRange = false;
    /// <summary>
    /// CalledIsSecondFloorInRange field.
    /// </summary>
    public bool CalledIsSecondFloorInRange = false;
    /// <summary>
    /// CalledIsSameDirectionOnRoute field.
    /// </summary>
    public bool CalledIsSameDirectionOnRoute = false;
    /// <summary>
    /// CalledIsFloorAndDirectionValid field.
    /// </summary>
    public bool CalledIsFloorAndDirectionValid = false;
    /// <summary>
    /// CalledPrintRequestStatus
    /// </summary>
    public bool CalledPrintRequestStatus = false;
    /// <summary>
    /// CalledMoveElevator
    /// </summary>
    public bool CalledMoveElevator = false;
    /// <summary>
    /// CalledStopElevator
    /// </summary>
    public bool CalledStopElevator = false;
    /// <summary>
    /// CalledOpenDoors
    /// </summary>
    public bool CalledOpenDoors = false;
    /// <summary>
    /// CalledCloseDoors
    /// </summary>
    public bool CalledCloseDoors = false;
    /// <summary>
    /// CalledMoveToNextFloor
    /// </summary>
    public bool CalledMoveToNextFloor = false;
    /// <summary>
    /// CalledHandleCompletedRequest
    /// </summary>
    public bool CalledHandleCompletedRequest = false;
    /// <summary>
    /// CalledHandleRequeueRequest
    /// </summary>
    public bool CalledHandleRequeueRequest = false;
    /// <summary>
    /// CalledRemoveAcceptedRequest
    /// </summary>
    public bool CalledRemoveAcceptedRequest = false;
    /// <summary>
    /// CalledCompleteRequest
    /// </summary>
    public bool CalledCompleteRequest = false;
    /// <summary>
    /// CalledRequeueRequest
    /// </summary>
    public bool CalledRequeueRequest = false;

    /// <summary>
    /// CallBaseIsFloorInRange field.
    /// </summary>
    public bool CallBaseIsFloorInRange = false;
    /// <summary>
    /// CallBaseIsSameDirectionOnRoute field.
    /// </summary>
    public bool CallBaseIsSameDirectionOnRoute = false;
    /// <summary>
    /// CallBaseIsFloorAndDirectionValid
    /// </summary>
    public bool CallBaseIsFloorAndDirectionValid = false;
    /// <summary>
    /// CallBaseOpenDoors
    /// </summary>
    public bool CallBaseOpenDoors = false;
    /// <summary>
    /// CallBaseCloseDoors
    /// </summary>
    public bool CallBaseCloseDoors = false;
    /// <summary>
    /// CallBaseMoveToNextFloor
    /// </summary>
    public bool CallBaseMoveToNextFloor = false;
    /// <summary>
    /// CallBaseRemoveAcceptedRequest
    /// </summary>
    public bool CallBaseRemoveAcceptedRequest = false;
    /// <summary>
    /// CallBaseHandleRequeueRequest
    /// </summary>
    public bool CallBaseHandleCompletedRequest = false;
    /// <summary>
    /// CallBaseHandleRequeueRequest
    /// </summary>
    public bool CallBaseHandleRequeueRequest = false;

    /// <summary>
    /// IsFirstFloorInRangeMockReturnValue
    /// </summary>
    public bool IsFirstFloorInRangeMockReturnValue = true;
    /// <summary>
    /// IsFirstFloorInRangeMockReturnValue
    /// </summary>
    public bool IsSecondFloorInRangeMockReturnValue = true;
    /// <summary>
    /// IsSameDirectionOnRouteMockReturnValue field.
    /// </summary>
    public bool IsSameDirectionOnRouteMockReturnValue = true;
    /// <summary>
    /// IsFloorAndDirectionValidMockReturnValue field.
    /// </summary>
    public bool IsFloorAndDirectionValidMockReturnValue = true;

    /// <summary>
    /// secondFloorInRangeFloor
    /// </summary>
    public int secondFloorInRangeFloor = int.MinValue + 1;
    /// <summary>
    /// PrintRequestStatusMessage
    /// </summary>
    public string? PrintRequestStatusMessage;

    public ElevatorModelMock(IMapper mapper)
        : base(mapper)
    {
    }

    /// <summary>
    /// ElevatorModelMock constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="floorMoveTimer"></param>
    /// <param name="doorsOpenTimer"></param>
    public ElevatorModelMock(IMapper mapper, Timer floorMoveTimer, Timer doorsOpenTimer)
        : base(mapper, floorMoveTimer, doorsOpenTimer)
    {
        PrintRequestStatus = (string message) =>
        {
            CalledPrintRequestStatus = true;
            PrintRequestStatusMessage = message;
        };

        CompleteRequest = (Guid requestId) =>
        {
            CalledCompleteRequest = true;
        };

        RequeueRequest = (Guid requestId) =>
        {
            CalledRequeueRequest = true;
        };
    }

    /// <summary>
    /// IsFloorInRange
    /// </summary>
    /// <param name="floor"></param>
    /// <returns></returns>
    protected override bool IsFloorInRange(int floor)
    {
        CalledIsFloorInRange = true;
        if (CallBaseIsFloorInRange)
            return base.IsFloorInRange(floor);

        if (floor == secondFloorInRangeFloor)
        {
            CalledIsSecondFloorInRange = true;
            return IsSecondFloorInRangeMockReturnValue;
        }

        return IsFirstFloorInRangeMockReturnValue;
    }

    /// <summary>
    /// IsSameDirectionOnRoute
    /// </summary>
    /// <param name="requestOriginFloor"></param>
    /// <param name="requestDirection"></param>
    /// <returns></returns>
    protected override bool IsSameDirectionOnRoute(int requestOriginFloor, ElevatorDirection requestDirection)
    {
        CalledIsSameDirectionOnRoute = true;
        if (CallBaseIsSameDirectionOnRoute)
            return base.IsSameDirectionOnRoute(requestOriginFloor, requestDirection);

        return IsSameDirectionOnRouteMockReturnValue;
    }

    /// <summary>
    /// IsFloorAndDirectionValid
    /// </summary>
    /// <param name="originFloor"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected override bool IsFloorAndDirectionValid(int originFloor, ElevatorDirection direction)
    {
        CalledIsFloorAndDirectionValid = true;
        if (CallBaseIsFloorAndDirectionValid)
            return base.IsFloorAndDirectionValid(originFloor, direction);

        return IsFloorAndDirectionValidMockReturnValue;
    }

    /// <summary>
    /// OpenDoors
    /// </summary>
    protected override void OpenDoors()
    {
        CalledOpenDoors = true;
        if (CallBaseOpenDoors)
            base.OpenDoors();
    }

    /// <summary>
    /// CloseDoors
    /// </summary>
    /// <param name="state"></param>
    protected override void CloseDoors(object? state)
    {
        CalledCloseDoors = true;
        if (CallBaseCloseDoors)
            base.CloseDoors(state);
    }

    /// <summary>
    /// MoveToNextFloor
    /// </summary>
    protected override void MoveToNextFloor()
    {
        CalledMoveToNextFloor = true;
        if (CallBaseMoveToNextFloor)
            base.MoveToNextFloor();
    }

    /// <summary>
    /// MoveElevator
    /// </summary>
    protected override void MoveElevator()
    {
        CalledMoveElevator = true;
        _isMoving = true;
    }

    /// <summary>
    /// StopElevator
    /// </summary>
    protected override void StopElevator()
    {
        CalledStopElevator = true;
        _isMoving = false;
    }

    /// <summary>
    /// HandleCompletedRequest
    /// </summary>
    /// <param name="request"></param>
    protected override void HandleCompletedRequest(ElevatorAcceptedRequest request)
    {
        CalledHandleCompletedRequest = true;
        if (CallBaseHandleCompletedRequest)
            base.HandleCompletedRequest(request);
    }

    /// <summary>
    /// HandleRequeueRequest
    /// </summary>
    /// <param name="request"></param>
    protected override void HandleRequeueRequest(ElevatorAcceptedRequest request)
    {
        CalledHandleRequeueRequest = true;
        if (CallBaseHandleRequeueRequest)
            base.HandleRequeueRequest(request);
    }

    /// <summary>
    /// RemoveAcceptedRequest
    /// </summary>
    /// <param name="id"></param>
    protected override void RemoveAcceptedRequest(Guid id)
    {
        CalledRemoveAcceptedRequest = true;
        if (CallBaseRemoveAcceptedRequest)
            base.RemoveAcceptedRequest(id);
    }
}
