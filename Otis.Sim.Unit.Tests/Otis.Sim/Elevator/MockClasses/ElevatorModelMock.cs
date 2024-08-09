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
    /// 
    /// </summary>
    public int secondFloorInRangeFloor = int.MinValue + 1;

    public ElevatorModelMock(IMapper mapper) 
        : base(mapper)
    {
    }

    public ElevatorModelMock(IMapper mapper, Timer floorMoveTimer, Timer doorsOpenTimer) 
        : base(mapper, floorMoveTimer, doorsOpenTimer)
    {
        PrintRequestStatus = (string message) => { CalledPrintRequestStatus = true; };
    }

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

    protected override bool IsSameDirectionOnRoute(int requestOriginFloor, ElevatorDirection requestDirection)
    {
        CalledIsSameDirectionOnRoute = true;
        if (CallBaseIsSameDirectionOnRoute)
            return base.IsSameDirectionOnRoute(requestOriginFloor, requestDirection);

        return IsSameDirectionOnRouteMockReturnValue;
    }

    protected override bool IsFloorAndDirectionValid(int originFloor, ElevatorDirection direction)
    {
        CalledIsFloorAndDirectionValid = true;
        if (CallBaseIsFloorAndDirectionValid)
            return base.IsFloorAndDirectionValid(originFloor, direction);

        return IsFloorAndDirectionValidMockReturnValue;
    }

    protected override void MoveElevator()
    {
        CalledMoveElevator = true;
    }
}
