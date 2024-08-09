using AutoMapper;
using Moq;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Unit.Tests.Otis.Sim.Elevator.MockClasses;
using System.Reflection;
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
    /// Mock<ElevatorModel> field
    /// </summary>
    private Mock<ElevatorModel> _mockElevatorModel;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper            = SetupIMapper();
        _mockElevatorModel = new Mock<ElevatorModel>(_mapper);
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

        var currentStatusProperty = GetCurrentStatusProperty();
        SetCurrentStatusProperty(currentStatusProperty, currentStatus);

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
    public void CanAcceptRequest_ExpectedResult(bool originFloorInRangeValid, 
        bool destinationFloorInRangeValid, 
        bool floorAndDirectionValid,
        bool expectedResult)
    {
        _elevatorModelMock.IsFirstFloorInRangeMockReturnValue      = originFloorInRangeValid;
        _elevatorModelMock.IsSecondFloorInRangeMockReturnValue     = destinationFloorInRangeValid;
        _elevatorModelMock.IsFloorAndDirectionValidMockReturnValue = floorAndDirectionValid;        

        var userInputRequest = new UserInputRequest()
        {
            OriginFloorInput = "1",
            DestinationFloorInput = $"{_elevatorModelMock.secondFloorInRangeFloor}",
            CapacityInput = "3"
        };

        var request = new ElevatorRequest(userInputRequest);

        var methodInfo = GetPublicInstanceMethodNotNull<ElevatorModelMock>("CanAcceptRequest");
        var result     = methodInfo.Invoke(_elevatorModelMock, new object[] { request });

        Assert.That(result, Is.EqualTo(expectedResult));
        Assert.That(_elevatorModelMock.CalledIsFloorInRange, Is.True);
        if (originFloorInRangeValid)
            Assert.That(_elevatorModelMock.CalledIsSecondFloorInRange, Is.True);
        if (originFloorInRangeValid && destinationFloorInRangeValid)
            Assert.That(_elevatorModelMock.CalledIsFloorAndDirectionValid, Is.True);
    }

    /// <summary>
    /// CanAcceptRequest with expected result
    /// </summary>
    [Test]
    [TestCase(ElevatorStatus.Idle, true, true)]
    [TestCase(ElevatorStatus.Idle, false, true)]
    [TestCase(ElevatorStatus.MovingUp, false, true)]
    [TestCase(ElevatorStatus.Idle, true, true, false)]
    [TestCase(ElevatorStatus.Idle, false, true, false)]
    [TestCase(ElevatorStatus.MovingUp, false, true, false)]
    [TestCase(ElevatorStatus.MovingUp, false, false, false, false)]
    public void AcceptRequest_ExpectedResult(ElevatorStatus currentStatus, 
        bool isSameDirectionOnRoute,
        bool expectedResult, 
        bool isMoving = true,
        bool isFloorAndDirectionValid = true)
    {
        var originFloor      = 1;
        var destinationFloor = 2;

        var userInputRequest = new UserInputRequest()
        {
            OriginFloorInput      = originFloor.ToString(),
            DestinationFloorInput = destinationFloor.ToString(),
            CapacityInput         = "3"
        };

        var request = new ElevatorRequest(userInputRequest);

        var currentStatusProperty = GetCurrentStatusProperty();
        SetCurrentStatusProperty(currentStatusProperty, currentStatus);

        _elevatorModelMock.IsSameDirectionOnRouteMockReturnValue = isSameDirectionOnRoute;

        var isMovingField = GetIsMovingField();
        isMovingField.SetValue(_elevatorModelMock, isMoving);

        _elevatorModelMock.IsFloorAndDirectionValidMockReturnValue = isFloorAndDirectionValid;

        var methodInfo = GetPublicInstanceMethodNotNull<ElevatorModelMock>("AcceptRequest");
        var result     = methodInfo.Invoke(_elevatorModelMock, new object[] { request });

        Assert.That(result, Is.EqualTo(expectedResult));

        if (!isFloorAndDirectionValid)
            return;

        if (currentStatus == ElevatorStatus.Idle)
        {
            var primaryDirectionQueueValue = GetPrimaryDirectionQueueValue(_elevatorModelMock) as SortedSet<int>;
            Assert.That(primaryDirectionQueueValue!.Contains(originFloor), Is.True);

            if (isSameDirectionOnRoute)
                Assert.That(primaryDirectionQueueValue.Contains(destinationFloor), Is.True);
            else
            {
                var secondaryDirectionQueueValue = GetSecondaryDirectionQueueValue(_elevatorModelMock) as SortedSet<int>;
                Assert.That(secondaryDirectionQueueValue!.Contains(destinationFloor), Is.True);
            }
        }
        else
        {
            var currentDirectionQueueValue = GetCurrentDirectionQueueValue(_elevatorModelMock) as SortedSet<int>;
            Assert.That(currentDirectionQueueValue!.Contains(originFloor), Is.True);
            Assert.That(currentDirectionQueueValue!.Contains(destinationFloor), Is.True);
        }        

        Assert.That(_elevatorModelMock.CalledPrintRequestStatus, Is.True);
        Assert.That(_elevatorModelMock.CalledMoveElevator, Is.EqualTo(!isMoving));
    }

    /// <summary>
    /// GetIsMovingField
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private FieldInfo GetIsMovingField()
        => GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_isMoving");

    /// <summary>
    /// GetIsMovingValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetIsMovingValue(ElevatorModelMock elevatorModelMock)
    {
        var isMovingField = GetIsMovingField();
        return ValidateFieldValue(isMovingField, elevatorModelMock);
    }

    /// <summary>
    /// GetCurrentStatusProperty
    /// </summary>
    /// <returns></returns>
    private PropertyInfo GetCurrentStatusProperty()
        => GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_currentStatus");

    /// <summary>
    /// SetCurrentStatusProperty
    /// </summary>
    /// <param name="currentStatusProperty"></param>
    /// <param name="elevatorStatus"></param>
    private void SetCurrentStatusProperty(PropertyInfo currentStatusProperty, ElevatorStatus elevatorStatus)
        => currentStatusProperty.SetValue(_elevatorModelMock, elevatorStatus);

    /// <summary>
    /// GetPrimaryDirectionQueueValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetPrimaryDirectionQueueValue(ElevatorModelMock elevatorModelMock)
    {
        var primaryDirectionQueueField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_primaryDirectionQueue");
        return ValidateFieldValue(primaryDirectionQueueField, elevatorModelMock);
    }

    /// <summary>
    /// GetSecondaryDirectionQueueValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetSecondaryDirectionQueueValue(ElevatorModelMock elevatorModelMock)
    {
        var secondaryDirectionQueueField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_secondaryDirectionQueue");
        return ValidateFieldValue(secondaryDirectionQueueField, elevatorModelMock);
    }

    /// <summary>
    /// GetCurrentDirectionQueueValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetCurrentDirectionQueueValue(ElevatorModelMock elevatorModelMock)
    {
        var currentDirectionProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_currentDirectionQueue");
        return ValidatePropertyValue(currentDirectionProperty, elevatorModelMock);
    }

    /// <summary>
    /// ValidateConstructorInitialisationsAndDefaults
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    private void ValidateConstructorInitialisationsAndDefaults(ElevatorModelMock? elevatorModelMock = null)
    {
        elevatorModelMock = elevatorModelMock ?? _elevatorModelMock;

        var descriptionProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("Description");
        var descriptionValue = ValidatePropertyValue(descriptionProperty, elevatorModelMock);
        Assert.That(descriptionValue, Is.EqualTo(string.Empty));

        var lowestFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("LowestFloor");
        var lowestFloorValue = ValidatePropertyValue(lowestFloorProperty, elevatorModelMock);
        Assert.That(lowestFloorValue, Is.EqualTo(0));

        var highestFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("HighestFloor");
        var highestFloorValue = ValidatePropertyValue(highestFloorProperty, elevatorModelMock);
        Assert.That(highestFloorValue, Is.EqualTo(0));

        var currentFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentFloor");
        var currentFloorValue = ValidatePropertyValue(currentFloorProperty, elevatorModelMock);
        Assert.That(currentFloorValue, Is.EqualTo(0));

        var nextFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("NextFloor");
        ValidatePropertyValue(nextFloorProperty, elevatorModelMock, false);

        var lastFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("LastFloor");
        var lastFloorValue = ValidatePropertyValue(lastFloorProperty, elevatorModelMock);
        Assert.That(lastFloorValue, Is.EqualTo(0));

        var currentLoadProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentLoad");
        var currentLoadValue = ValidatePropertyValue(currentLoadProperty, elevatorModelMock);
        Assert.That(currentLoadValue, Is.EqualTo(0));

        var maximumLoadProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("MaximumLoad");
        var maximumLoadValue = ValidatePropertyValue(maximumLoadProperty, elevatorModelMock);
        Assert.That(maximumLoadValue, Is.EqualTo(0));

        var capacityProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("Capacity");
        var capacityValue = ValidatePropertyValue(capacityProperty, elevatorModelMock);
        Assert.That(capacityValue, Is.EqualTo(0));

        var floorMoveTimeProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("FloorMoveTime");
        var floorMoveTimeValue = ValidatePropertyValue(floorMoveTimeProperty, elevatorModelMock);
        Assert.That(floorMoveTimeValue, Is.Not.EqualTo(0));

        var doorsOpenTimeProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("DoorsOpenTime");
        var doorsOpenTimeValue = ValidatePropertyValue(doorsOpenTimeProperty, elevatorModelMock);
        Assert.That(doorsOpenTimeValue, Is.Not.EqualTo(0));

        GetNestedTypeNotNull<ElevatorModel>("CompleteRequestDelegate");

        var completeRequestField = GetPublicInstanceFieldNotNull<ElevatorModel>("CompleteRequest");
        ValidateFieldValue(completeRequestField, _mockElevatorModel.Object, false);

        GetNestedTypeNotNull<ElevatorModel>("RequeueRequestDelegate");

        var requeueRequestField = GetPublicInstanceFieldNotNull<ElevatorModel>("RequeueRequest");
        ValidateFieldValue(requeueRequestField, _mockElevatorModel.Object, false);

        GetNestedTypeNotNull<ElevatorModel>("PrintRequestStatusDelegate");

        var printRequestStatusField = GetPublicInstanceFieldNotNull<ElevatorModel>("PrintRequestStatus");
        ValidateFieldValue(printRequestStatusField, _mockElevatorModel.Object, false);

        var isMovingValue = GetIsMovingValue(elevatorModelMock);
        Assert.That(isMovingValue, Is.EqualTo(false));

        var _currentStatusProperty = GetCurrentStatusProperty();
        var _currentStatusValue = ValidatePropertyValue(_currentStatusProperty, elevatorModelMock);
        Assert.That(_currentStatusValue, Is.EqualTo(ElevatorStatus.Idle));

        var currentStatusProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentStatus");
        var currentStatusValue = ValidatePropertyValue(currentStatusProperty, elevatorModelMock);
        Assert.That(currentStatusValue, Is.EqualTo(_currentStatusValue));

        var elevatorDirectionField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_elevatorDirection");
        var elevatorDirectionValue = ValidateFieldValue(elevatorDirectionField, elevatorModelMock, false);

        var currentDirectionProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_currentDirection");
        var currentDirectionValue = ValidatePropertyValue(currentDirectionProperty, elevatorModelMock, false);
        Assert.That(currentDirectionValue, Is.EqualTo(elevatorDirectionValue));

        var isPrimaryDirectionProperty = GetNonPublicInstancePropertyNotNull<ElevatorModelMock>("_isPrimaryDirection");
        var isPrimaryDirectionValue = ValidatePropertyValue(isPrimaryDirectionProperty, elevatorModelMock);
        Assert.That(isPrimaryDirectionValue, Is.EqualTo(false));

        var primaryDirectionQueueValue = GetPrimaryDirectionQueueValue(elevatorModelMock);
        Assert.That(primaryDirectionQueueValue, Is.EqualTo(new SortedSet<int>()));

        var secondaryDirectionQueueValue = GetSecondaryDirectionQueueValue(elevatorModelMock);
        Assert.That(secondaryDirectionQueueValue, Is.EqualTo(new SortedSet<int>()));

        var currentDirectionQueueValue = GetCurrentDirectionQueueValue(elevatorModelMock);
        Assert.That(currentDirectionQueueValue, Is.EqualTo(secondaryDirectionQueueValue));

        var acceptedRequestsField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_acceptedRequests");
        var acceptedRequestsValue = ValidateFieldValue(acceptedRequestsField, elevatorModelMock);
        Assert.That(acceptedRequestsValue, Is.EqualTo(new List<ElevatorAcceptedRequest>()));


        var mapperField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_mapper");
        ValidateFieldValue(mapperField, elevatorModelMock);

        var mockFloorMoveTimer = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_floorMoveTimer");
        ValidateFieldValue(mockFloorMoveTimer, elevatorModelMock);

        var mockDoorsOpenTimer = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_doorsOpenTimer");
        ValidateFieldValue(mockDoorsOpenTimer, elevatorModelMock);
    }
}
