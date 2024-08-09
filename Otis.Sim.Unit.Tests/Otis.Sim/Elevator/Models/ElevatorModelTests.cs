using AutoMapper;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Unit.Tests.Otis.Sim.Elevator.MockClasses;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorModelMockTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorModelMockTests : ElevatorTests
{
    /// <summary>
    /// Mock<IMapper> field.
    /// </summary>
    private IMapper _mapper;
    /// <summary>
    /// Mock _mockFloorMoveTimer field.
    /// </summary>
    private Timer _floorMoveTimer;
    /// <summary>
    /// Mock _mockDoorsOpenTimer field.
    /// </summary>
    private Timer _doorsOpenTimer;
    /// <summary>
    /// ElevatorModelMock field
    /// </summary>
    private ElevatorModelMock _elevatorModelMock;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper = SetupIMapper();
    }

    /// <summary>
    /// SetUp before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _floorMoveTimer = 
            new Timer(new TimerCallback((o) => { }), null, Timeout.Infinite, Timeout.Infinite);
        _doorsOpenTimer = 
            new Timer(new TimerCallback((o) => { }), null, Timeout.Infinite, Timeout.Infinite);

        _elevatorModelMock = 
            new ElevatorModelMock(_mapper, _floorMoveTimer, _doorsOpenTimer);
    }

    /// <summary>
    /// Test constructor with mapper
    /// </summary>
    [Test]
    public void Constructor_WithMapper()
    {
        var ElevatorModelMock = new ElevatorModelMock(_mapper);
        ValidateConstructorInitialisationsAndDefaults(ElevatorModelMock);
    }

    /// <summary>
    /// Test constructor with mapper, floorMoveTimer and doorsOpenTimer
    /// </summary>
    [Test]
    public void Constructor_WithMapperAndTimers()
    {
        ValidateConstructorInitialisationsAndDefaults();
    }

    /// <summary>
    /// IsFloorInRange with expected result
    /// </summary>
    /// <param name="floorNumber"></param>
    /// <returns></returns>
    [Test]
    [TestCase(2, true)]
    [TestCase(10, true)]
    [TestCase(3, true)]
    [TestCase(9, true)]
    [TestCase(1, false)]
    [TestCase(11, false)]
    public void IsFloorInRange_ExpectedResult(int floor, bool expectedResult)
    {
        _elevatorModelMock.LowestFloor  = 2;
        _elevatorModelMock.HighestFloor = 10;
        _elevatorModelMock.CallBaseIsFloorInRange = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("IsFloorInRange");
        var result     = methodInfo.Invoke(_elevatorModelMock, new object[] { floor });

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    /// <summary>
    /// IsSameDirectionOnRoute with expected result
    /// </summary>
    /// <param name="requestOriginFloor"></param>
    /// <param name="requestDirection"></param>
    /// <param name="expectedResult"></param>
    [Test]
    [TestCase(3, ElevatorDirection.Up, true)]
    [TestCase(1, ElevatorDirection.Down, true)]
    [TestCase(2, ElevatorDirection.Up, false)]
    [TestCase(2, ElevatorDirection.Down, false)]
    [TestCase(1, ElevatorDirection.Up, false)]
    [TestCase(3, ElevatorDirection.Down, false)]
    public void IsSameDirectionOnRoute_ExpectedResult(int requestOriginFloor, ElevatorDirection requestDirection,
        bool expectedResult)
    {
        _elevatorModelMock.CurrentFloor = 2;
        _elevatorModelMock.CallBaseIsSameDirectionOnRoute = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("IsSameDirectionOnRoute");
        var result     = methodInfo.Invoke(_elevatorModelMock, new object[] { requestOriginFloor, requestDirection });

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    /// <summary>
    /// IsFloorAndDirectionValid with expected result
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="currentStatus"></param>
    /// <param name="expectedResult"></param>
    [Test]
    [TestCase(false, ElevatorDirection.Up, ElevatorStatus.MovingDown, false)]
    [TestCase(true, ElevatorDirection.Up, ElevatorStatus.Idle, true)]
    [TestCase(true, ElevatorDirection.Up, ElevatorStatus.MovingDown, false)]
    [TestCase(true, ElevatorDirection.Down, ElevatorStatus.MovingUp, false)]
    [TestCase(true, ElevatorDirection.Up, ElevatorStatus.MovingUp, true, true)]
    [TestCase(true, ElevatorDirection.Down, ElevatorStatus.MovingDown, true, true)]
    public void IsFloorAndDirectionValid_ExpectedResult(bool capacity,
        ElevatorDirection direction,
        ElevatorStatus currentStatus, 
        bool expectedResult,
        bool checkWasIsSameDirectionOnRouteCalled = false)
    {
        _elevatorModelMock.MaximumLoad = 2;
        _elevatorModelMock.CurrentLoad = capacity
            ? 1
            : _elevatorModelMock.MaximumLoad;

        _elevatorModelMock.CallBaseIsFloorAndDirectionValid = true;
        _elevatorModelMock.CallBaseIsSameDirectionOnRoute   = false;

        var currentStatusProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_currentStatus");
        currentStatusProperty.SetValue(_elevatorModelMock, currentStatus);

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("IsFloorAndDirectionValid");
        var result     = methodInfo.Invoke(_elevatorModelMock, new object[] { 1, direction });

        Assert.That(result, Is.EqualTo(expectedResult));
        if (checkWasIsSameDirectionOnRouteCalled)
            Assert.That(_elevatorModelMock.CalledIsSameDirectionOnRoute, Is.True);
    }

    /// <summary>
    /// CanAcceptRequest with expected result
    /// </summary>
    /// <param name="originFloorInRangeValid"></param>
    /// <param name="destinationFloorInRangeValid"></param>
    /// <param name="floorAndDirectionValid"></param>
    /// <param name="expectedResult"></param>
    [Test]
    [TestCase(false, true, true, false)]
    [TestCase(false, false, true, false)]
    [TestCase(false, false, false, false)]
    [TestCase(true, false, false, false)]
    [TestCase(true, true, false, false)]
    [TestCase(true, true, true, true)]
    public void CanAcceptRequest(bool originFloorInRangeValid, 
        bool destinationFloorInRangeValid, 
        bool floorAndDirectionValid,
        bool expectedResult)
    {
        _elevatorModelMock.IsFirstFloorInRangeMockReturnValue      = originFloorInRangeValid;
        _elevatorModelMock.IsSecondFloorInRangeMockReturnValue     = destinationFloorInRangeValid;
        _elevatorModelMock.IsFloorAndDirectionValidMockReturnValue = floorAndDirectionValid;

        var methodInfo = GetPublicInstanceMethodNotNull<ElevatorModelMock>("CanAcceptRequest");
        var request    = new ElevatorRequest(
            new UserInputRequest()
            {
                OriginFloorInput      = "1",
                DestinationFloorInput = $"{_elevatorModelMock.secondFloorInRangeFloor}",
                CapacityInput         = "3"
            });

        var result     = methodInfo.Invoke(_elevatorModelMock, new object[] { request });

        Assert.That(result, Is.EqualTo(expectedResult));
        Assert.That(_elevatorModelMock.CalledIsFloorInRange, Is.True);
        if (originFloorInRangeValid)
            Assert.That(_elevatorModelMock.CalledIsSecondFloorInRange, Is.True);
        if (originFloorInRangeValid && destinationFloorInRangeValid)
            Assert.That(_elevatorModelMock.CalledIsFloorAndDirectionValid, Is.True);
    }

    private void ValidateConstructorInitialisationsAndDefaults(ElevatorModelMock? ElevatorModelMock = null)
    {
        ElevatorModelMock = ElevatorModelMock ?? _elevatorModelMock;

        var descriptionProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("Description");
        var descriptionValue = ValidatePropertyValue(descriptionProperty, ElevatorModelMock);
        Assert.That(descriptionValue, Is.EqualTo(string.Empty));

        var lowestFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("LowestFloor");
        var lowestFloorValue = ValidatePropertyValue(lowestFloorProperty, ElevatorModelMock);
        Assert.That(lowestFloorValue, Is.EqualTo(0));

        var highestFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("HighestFloor");
        var highestFloorValue = ValidatePropertyValue(highestFloorProperty, ElevatorModelMock);
        Assert.That(highestFloorValue, Is.EqualTo(0));

        var currentFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentFloor");
        var currentFloorValue = ValidatePropertyValue(currentFloorProperty, ElevatorModelMock);
        Assert.That(currentFloorValue, Is.EqualTo(0));

        var nextFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("NextFloor");
        ValidatePropertyValue(nextFloorProperty, ElevatorModelMock, false);

        var lastFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("LastFloor");
        var lastFloorValue = ValidatePropertyValue(lastFloorProperty, ElevatorModelMock);
        Assert.That(lastFloorValue, Is.EqualTo(0));

        var currentLoadProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentLoad");
        var currentLoadValue = ValidatePropertyValue(currentLoadProperty, ElevatorModelMock);
        Assert.That(currentLoadValue, Is.EqualTo(0));

        var maximumLoadProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("MaximumLoad");
        var maximumLoadValue = ValidatePropertyValue(maximumLoadProperty, ElevatorModelMock);
        Assert.That(maximumLoadValue, Is.EqualTo(0));

        var capacityProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("Capacity");
        var capacityValue = ValidatePropertyValue(capacityProperty, ElevatorModelMock);
        Assert.That(capacityValue, Is.EqualTo(0));

        var floorMoveTimeProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("FloorMoveTime");
        var floorMoveTimeValue = ValidatePropertyValue(floorMoveTimeProperty, ElevatorModelMock);
        Assert.That(floorMoveTimeValue, Is.Not.EqualTo(0));

        var doorsOpenTimeProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("DoorsOpenTime");
        var doorsOpenTimeValue = ValidatePropertyValue(doorsOpenTimeProperty, ElevatorModelMock);
        Assert.That(doorsOpenTimeValue, Is.Not.EqualTo(0));

        GetNestedTypeNotNull<ElevatorModel>("CompleteRequestDelegate");

        var completeRequestField = GetPublicInstanceFieldNotNull<ElevatorModelMock>("CompleteRequest");
        ValidateFieldValue(completeRequestField, ElevatorModelMock, false);

        GetNestedTypeNotNull<ElevatorModel>("RequeueRequestDelegate");

        var requeueRequestField = GetPublicInstanceFieldNotNull<ElevatorModelMock>("RequeueRequest");
        ValidateFieldValue(requeueRequestField, ElevatorModelMock, false);

        GetNestedTypeNotNull<ElevatorModel>("PrintRequestStatusDelegate");

        var printRequestStatusField = GetPublicInstanceFieldNotNull<ElevatorModelMock>("PrintRequestStatus");
        ValidateFieldValue(printRequestStatusField, ElevatorModelMock, false);

        var isMovingField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_isMoving");
        var isMovingValue = ValidateFieldValue(isMovingField, ElevatorModelMock);
        Assert.That(isMovingValue, Is.EqualTo(false));

        var _currentStatusProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_currentStatus");
        var _currentStatusValue = ValidatePropertyValue(_currentStatusProperty, ElevatorModelMock);
        Assert.That(_currentStatusValue, Is.EqualTo(ElevatorStatus.Idle));

        var currentStatusProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentStatus");
        var currentStatusValue = ValidatePropertyValue(currentStatusProperty, ElevatorModelMock);
        Assert.That(currentStatusValue, Is.EqualTo(_currentStatusValue));

        var elevatorDirectionField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_elevatorDirection");
        var elevatorDirectionValue = ValidateFieldValue(elevatorDirectionField, ElevatorModelMock, false);

        var currentDirectionProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_currentDirection");
        var currentDirectionValue = ValidatePropertyValue(currentDirectionProperty, ElevatorModelMock, false);
        Assert.That(currentDirectionValue, Is.EqualTo(elevatorDirectionValue));

        var isPrimaryDirectionProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_isPrimaryDirection");
        var isPrimaryDirectionValue = ValidatePropertyValue(isPrimaryDirectionProperty, ElevatorModelMock);
        Assert.That(isPrimaryDirectionValue, Is.EqualTo(false));

        var primaryDirectionQueueField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_primaryDirectionQueue");
        var primaryDirectionQueueValue = ValidateFieldValue(primaryDirectionQueueField, ElevatorModelMock);
        Assert.That(primaryDirectionQueueValue, Is.EqualTo(new SortedSet<int>()));

        var secondaryDirectionQueueField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_secondaryDirectionQueue");
        var secondaryDirectionQueueValue = ValidateFieldValue(secondaryDirectionQueueField, ElevatorModelMock);
        Assert.That(secondaryDirectionQueueValue, Is.EqualTo(new SortedSet<int>()));

        var currentDirectionQueueProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_currentDirectionQueue");
        var currentDirectionQueueValue = ValidatePropertyValue(currentDirectionQueueProperty, ElevatorModelMock);
        Assert.That(currentDirectionQueueValue, Is.EqualTo(secondaryDirectionQueueValue));

        var acceptedRequestsField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_acceptedRequests");
        var acceptedRequestsValue = ValidateFieldValue(acceptedRequestsField, ElevatorModelMock);
        Assert.That(acceptedRequestsValue, Is.EqualTo(new List<ElevatorAcceptedRequest>()));


        var mapperField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_mapper");
        ValidateFieldValue(mapperField, ElevatorModelMock);

        var mockFloorMoveTimer = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_floorMoveTimer");
        ValidateFieldValue(mockFloorMoveTimer, ElevatorModelMock);

        var mockDoorsOpenTimer = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_doorsOpenTimer");
        ValidateFieldValue(mockDoorsOpenTimer, ElevatorModelMock);
    }
}
