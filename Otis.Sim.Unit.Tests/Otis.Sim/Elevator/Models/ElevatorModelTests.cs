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
        _mapper = SetupIMapper();
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
        var elevatorModelMock = new ElevatorModelMock(_mapper);
        ValidateConstructorInitialisationsAndDefaults(elevatorModelMock);
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
        _elevatorModelMock.LowestFloor = 2;
        _elevatorModelMock.HighestFloor = 10;
        _elevatorModelMock.CallBaseIsFloorInRange = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("IsFloorInRange");
        var result = methodInfo.Invoke(_elevatorModelMock, new object[] { floor });

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
        var result = methodInfo.Invoke(_elevatorModelMock, new object[] { requestOriginFloor, requestDirection });

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
        _elevatorModelMock.CallBaseIsSameDirectionOnRoute = false;

        var currentStatusProperty = GetCurrentStatusProperty();
        SetCurrentStatusValue(currentStatusProperty, currentStatus);

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("IsFloorAndDirectionValid");
        var result = methodInfo.Invoke(_elevatorModelMock, new object[] { 1, direction });

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
        _elevatorModelMock.IsFirstFloorInRangeMockReturnValue = originFloorInRangeValid;
        _elevatorModelMock.IsSecondFloorInRangeMockReturnValue = destinationFloorInRangeValid;
        _elevatorModelMock.IsFloorAndDirectionValidMockReturnValue = floorAndDirectionValid;

        var userInputRequest = new UserInputRequest()
        {
            OriginFloorInput = "1",
            DestinationFloorInput = $"{_elevatorModelMock.secondFloorInRangeFloor}",
            CapacityInput = "3"
        };

        var request = new ElevatorRequest(userInputRequest);

        var methodInfo = GetPublicInstanceMethodNotNull<ElevatorModelMock>("CanAcceptRequest");
        var result = methodInfo.Invoke(_elevatorModelMock, new object[] { request });

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
        var originFloor = 1;
        var destinationFloor = 2;

        var userInputRequest = new UserInputRequest()
        {
            OriginFloorInput = originFloor.ToString(),
            DestinationFloorInput = destinationFloor.ToString(),
            CapacityInput = "3"
        };

        var request = new ElevatorRequest(userInputRequest);

        var currentStatusProperty = GetCurrentStatusProperty();
        SetCurrentStatusValue(currentStatusProperty, currentStatus);

        _elevatorModelMock.IsSameDirectionOnRouteMockReturnValue = isSameDirectionOnRoute;

        var isMovingField = GetIsMovingField();
        isMovingField.SetValue(_elevatorModelMock, isMoving);

        _elevatorModelMock.IsFloorAndDirectionValidMockReturnValue = isFloorAndDirectionValid;

        var methodInfo = GetPublicInstanceMethodNotNull<ElevatorModelMock>("AcceptRequest");
        var result = methodInfo.Invoke(_elevatorModelMock, new object[] { request });

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
    /// InitiateElevatorMove with expected result
    /// </summary>
    /// <param name="currentFloor"></param>
    /// <param name="nextFloor"></param>
    /// <param name="isPrimaryDirection"></param>
    /// <param name="expectedStatus"></param>
    [Test]
    [TestCase(1, null, true)]
    [TestCase(1, null, false)]
    [TestCase(1, 2, true, ElevatorStatus.MovingUp)]
    [TestCase(-1, 14, false, ElevatorStatus.MovingUp)]
    [TestCase(2, 1, true, ElevatorStatus.MovingDown)]
    [TestCase(14, -1, false, ElevatorStatus.MovingDown)]
    [TestCase(-2, -2, false)]
    [TestCase(1, 1, false)]
    public void InitiateElevatorMove_ExpectedResult(int currentFloor, int? nextFloor, bool isPrimaryDirection,
        ElevatorStatus expectedStatus = ElevatorStatus.Idle)
    {
        var currentFloorProperty = GetCurrentFloorProperty();
        var primaryDirectionQueueField = GetPrimaryDirectionQueueField();
        var secondaryDirectionQueueField = GetSecondaryDirectionQueueField();

        var queueField = isPrimaryDirection
            ? primaryDirectionQueueField
            : secondaryDirectionQueueField;

        var queue = new SortedSet<int>();
        if (nextFloor != null)
            queue.Add((int)nextFloor);

        queueField.SetValue(_elevatorModelMock, queue);

        currentFloorProperty.SetValue(_elevatorModelMock, currentFloor);

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("InitiateElevatorMove");
        methodInfo.Invoke(_elevatorModelMock, new object[] { null! });

        var currentFloorValue = GetCurrentFloorValue(_elevatorModelMock);
        var primaryDirectionQueueValue = GetPrimaryDirectionQueueValue(_elevatorModelMock) as SortedSet<int>;
        var secondaryDirectionQueueValue = GetSecondaryDirectionQueueValue(_elevatorModelMock) as SortedSet<int>;
        var currentStatusValue = GetCurrentStatusValue(_elevatorModelMock);

        var areCurrentAndNextFloorEqual = nextFloor == currentFloor;

        if (nextFloor == null || areCurrentAndNextFloorEqual)
        {
            Assert.That(currentFloorValue, Is.EqualTo(currentFloor));
            Assert.That(primaryDirectionQueueValue!.Any(), Is.False);
            Assert.That(secondaryDirectionQueueValue!.Any(), Is.False);
            Assert.That(_elevatorModelMock.CalledStopElevator, Is.EqualTo(areCurrentAndNextFloorEqual));
            Assert.That(_elevatorModelMock.CalledOpenDoors, Is.EqualTo(areCurrentAndNextFloorEqual));
        }
        else
        {
            var newCurrentFloor = nextFloor > currentFloor
                ? ++currentFloor
                : --currentFloor;

            Assert.That(currentFloorValue, Is.EqualTo(newCurrentFloor));
            Assert.That(primaryDirectionQueueValue!.Any(), Is.EqualTo(isPrimaryDirection));
            Assert.That(secondaryDirectionQueueValue!.Any(), Is.EqualTo(!isPrimaryDirection));
            Assert.That(_elevatorModelMock.CalledStopElevator, Is.False);
            Assert.That(_elevatorModelMock.CalledOpenDoors, Is.False);
        }

        Assert.That(currentStatusValue, Is.EqualTo(expectedStatus));
    }

    /// <summary>
    /// OpenDoors with expected result
    /// </summary>
    /// <param name="currentFloor"></param>
    /// <param name="originFloor"></param>
    /// <param name="destinationFloor"></param>
    /// <param name="setCapacityToZero"></param>
    /// <param name="numberOfPeople"></param>
    [Test]
    [TestCase(1, 4, 1)]
    [TestCase(1, 1, 4, true)]
    [TestCase(1, 1, 4, false, 10)]
    [TestCase(1, 1, 4, false)]
    public void OpenDoors_ExpectedResult(int currentFloor, int originFloor, int destinationFloor,
        bool setCapacityToZero = false, int numberOfPeople = 2)
    {
        var initialCurrentLoadValue = setCapacityToZero
            ? 10
            : 6;
        var maximumLoadValue = 10;

        var currentFloorProperty = GetCurrentFloorProperty();
        currentFloorProperty.SetValue(_elevatorModelMock, currentFloor);

        var currentLoadProperty = GetCurrentLoadProperty();
        currentLoadProperty.SetValue(_elevatorModelMock, initialCurrentLoadValue);

        var maximumLoadProperty = GetMaximumLoadProperty();
        maximumLoadProperty.SetValue(_elevatorModelMock, maximumLoadValue);

        var initialCapacityValue = (int)GetCapacityValue(_elevatorModelMock)!;

        var acceptedRequestsField = GetAcceptedRequestsField();
        var acceptedRequest = new ElevatorAcceptedRequest()
        {
            OriginFloor = originFloor,
            DestinationFloor = destinationFloor,
            NumberOfPeople = numberOfPeople
        };
        var acceptedRequestsList = new List<ElevatorAcceptedRequest>()
        {
            acceptedRequest
        };
        acceptedRequestsField.SetValue(_elevatorModelMock, acceptedRequestsList);

        _elevatorModelMock.CallBaseOpenDoors = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("OpenDoors");
        methodInfo.Invoke(_elevatorModelMock, null);

        var currentLoadValue = (int)GetCurrentLoadValue(_elevatorModelMock)!;
        var capacityValue = (int)GetCapacityValue(_elevatorModelMock)!;

        if (currentFloor == destinationFloor)
        {
            var droppedOffMessage = acceptedRequest.ToDroppedOffRequestString(acceptedRequest.NumberOfPeople, capacityValue);

            Assert.That(acceptedRequest.DestinationFloorServiced, Is.True);
            Assert.That(currentLoadValue, Is.EqualTo(initialCurrentLoadValue - acceptedRequest.NumberOfPeople));
            Assert.That(_elevatorModelMock.CalledPrintRequestStatus, Is.True);
            Assert.That(_elevatorModelMock.PrintRequestStatusMessage, Is.EqualTo(droppedOffMessage));
            Assert.That(_elevatorModelMock.CalledHandleCompletedRequest, Is.True);
        }
        else if (currentFloor == originFloor)
        {
            var pickedUpMessage = acceptedRequest.ToPickedUpRequestString(acceptedRequest.NumberOfPeople, capacityValue);

            Assert.That(acceptedRequest.OriginFloorServiced, Is.True);

            if (setCapacityToZero)
            {
                Assert.That(_elevatorModelMock.CalledHandleRequeueRequest, Is.True);
                return;
            }

            if (initialCapacityValue < numberOfPeople)
                Assert.That(acceptedRequest.NumberOfPeople, Is.EqualTo(initialCapacityValue));

            Assert.That(currentLoadValue, Is.EqualTo(initialCurrentLoadValue + acceptedRequest.NumberOfPeople));
            Assert.That(_elevatorModelMock.PrintRequestStatusMessage, Is.EqualTo(pickedUpMessage));
            Assert.That(_elevatorModelMock.CalledHandleCompletedRequest, Is.True);
        }
    }

    /// <summary>
    /// CloseDoors with expected result
    /// </summary>
    [Test]
    public void CloseDoors_ExpectedResult()
    {
        _elevatorModelMock.CallBaseCloseDoors = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("CloseDoors");
        methodInfo.Invoke(_elevatorModelMock, new object[] { null! });

        Assert.That(_elevatorModelMock.CalledMoveToNextFloor, Is.True);
    }

    /// <summary>
    /// MoveToNextFloor with expected result
    /// </summary>
    /// <param name="nextFloor"></param>
    [Test]
    [TestCase(null)]
    [TestCase(1)]
    public void MoveToNextFloor_ExpectedResult(int? nextFloor)
    {
        var currenstStatusProperty = GetCurrentStatusProperty();
        currenstStatusProperty.SetValue(_elevatorModelMock, ElevatorStatus.MovingUp);

        if (nextFloor != null)
        {
            var primaryDirectionQueueField = GetPrimaryDirectionQueueField();
            primaryDirectionQueueField.SetValue(_elevatorModelMock, new SortedSet<int>() { (int)nextFloor });
        }

        _elevatorModelMock.CallBaseMoveToNextFloor = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("MoveToNextFloor");
        methodInfo.Invoke(_elevatorModelMock, null);

        if (nextFloor == null)
        {
            var currenstStatusValue = GetCurrentStatusValue(_elevatorModelMock);

            Assert.That(currenstStatusValue, Is.EqualTo(ElevatorStatus.Idle));
            Assert.That(_elevatorModelMock.CalledStopElevator, Is.True);
        }
        else
            Assert.That(_elevatorModelMock.CalledMoveElevator, Is.True);
    }

    /// <summary>
    /// MoveElevator with expected result
    /// </summary>
    [Test]
    public void MoveElevator_ExpectedResult()
    {
        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("MoveElevator");
        methodInfo.Invoke(_elevatorModelMock, null);

        var isMovingValue = GetIsMovingValue(_elevatorModelMock);

        Assert.That(isMovingValue, Is.True);
    }

    /// <summary>
    /// StopElevator with expected result
    /// </summary>
    [Test]
    public void StopElevator_ExpectedResult()
    {
        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("StopElevator");
        methodInfo.Invoke(_elevatorModelMock, null);

        var isMovingValue = GetIsMovingValue(_elevatorModelMock);

        Assert.That(isMovingValue, Is.False);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void HandleCompletedRequest_ExpectedResult(bool isRequestCompleted)
    {
        ElevatorAcceptedRequest elevatorAcceptedRequest = new ElevatorAcceptedRequest()
        {
            OriginFloorServiced = true,
            DestinationFloorServiced = isRequestCompleted
        };

        _elevatorModelMock.CallBaseHandleCompletedRequest = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("HandleCompletedRequest");
        methodInfo.Invoke(_elevatorModelMock, new object[] { elevatorAcceptedRequest });

        if (isRequestCompleted)
        {
            var completedRequestMessage = elevatorAcceptedRequest.ToCompletedRequestString();

            Assert.That(_elevatorModelMock.CalledRemoveAcceptedRequest, Is.True);
            Assert.That(_elevatorModelMock.CalledCompleteRequest, Is.True);
            Assert.That(_elevatorModelMock.CalledPrintRequestStatus, Is.True);
            Assert.That(_elevatorModelMock.PrintRequestStatusMessage, Is.EqualTo(completedRequestMessage));
        }
        else
        {
            Assert.That(_elevatorModelMock.CalledRemoveAcceptedRequest, Is.False);
            Assert.That(_elevatorModelMock.CalledCompleteRequest, Is.False);
            Assert.That(_elevatorModelMock.CalledPrintRequestStatus, Is.False);
        }
    }

    /// <summary>
    /// HandleRequeueRequest with expected result
    /// </summary>
    [Test]
    public void HandleRequeueRequest_ExpectedResult()
    {
        ElevatorAcceptedRequest elevatorAcceptedRequest = new ElevatorAcceptedRequest();

        _elevatorModelMock.CallBaseHandleRequeueRequest = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("HandleRequeueRequest");
        methodInfo.Invoke(_elevatorModelMock, new object[] { elevatorAcceptedRequest });

        var requeueRequestMessage = elevatorAcceptedRequest.ToRequeuedRequestString();

        Assert.That(_elevatorModelMock.CalledRemoveAcceptedRequest, Is.True);
        Assert.That(_elevatorModelMock.CalledRequeueRequest, Is.True);
        Assert.That(_elevatorModelMock.CalledPrintRequestStatus, Is.True);
        Assert.That(_elevatorModelMock.PrintRequestStatusMessage, Is.EqualTo(requeueRequestMessage));
    }

    /// <summary>
    /// RemoveAcceptedRequest with expected result
    /// </summary>
    [Test]
    public void RemoveAcceptedRequest_ExpectedResult()
    {
        ElevatorAcceptedRequest elevatorAcceptedRequest = new ElevatorAcceptedRequest();
        var acceptedRequestsField = GetAcceptedRequestsField();
        acceptedRequestsField.SetValue(_elevatorModelMock, new List<ElevatorAcceptedRequest>() { elevatorAcceptedRequest });

        _elevatorModelMock.CallBaseRemoveAcceptedRequest = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorModelMock>("RemoveAcceptedRequest");
        methodInfo.Invoke(_elevatorModelMock, new object[] { elevatorAcceptedRequest.Id });

        var acceptedRequestsValue = GetAcceptedRequestsValue(_elevatorModelMock) as List<ElevatorAcceptedRequest>;

        Assert.That(acceptedRequestsValue!.Any(), Is.False);
    }

    /// <summary>
    /// ToString with expected result
    /// </summary>
    [Test]
    public void ToString_ExpectedResult()
    {
        var elevatorToString = _elevatorModelMock.ToString();
        Assert.That(
            elevatorToString.Contains($"{nameof(_elevatorModelMock.Description)}: {_elevatorModelMock.Description}, "), Is.True);
        Assert.That(
            elevatorToString.Contains($"{nameof(_elevatorModelMock.LowestFloor)}: {_elevatorModelMock.LowestFloor}, "), Is.True);
        Assert.That(
            elevatorToString.Contains($"{nameof(_elevatorModelMock.HighestFloor)}: {_elevatorModelMock.HighestFloor}, "), Is.True);
        Assert.That(
            elevatorToString.Contains($"{nameof(_elevatorModelMock.MaximumLoad)}: {_elevatorModelMock.MaximumLoad}"), Is.True);
    }

    /// <summary>
    /// GetCurrentFloorValue
    /// </summary>
    /// <returns></returns>
    private PropertyInfo GetCurrentFloorProperty()
        => GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentFloor");

    /// <summary>
    /// GetCurrentFloorValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetCurrentFloorValue(ElevatorModelMock elevatorModelMock)
    {
        var currentFloorProperty = GetCurrentFloorProperty();
        return ValidatePropertyValue(currentFloorProperty, elevatorModelMock);
    }

    /// <summary>
    /// GetIsMovingField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetIsMovingField()
        => GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_isMoving");

    /// <summary>
    /// GetCurrentLoadProperty
    /// </summary>
    /// <returns></returns>
    private PropertyInfo GetCurrentLoadProperty()
        => GetPublicInstancePropertyNotNull<ElevatorModelMock>("CurrentLoad");

    /// <summary>
    /// GetCurrentLoadValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetCurrentLoadValue(ElevatorModelMock elevatorModelMock)
    {
        var currentLoadProperty = GetCurrentLoadProperty();
        return ValidatePropertyValue(currentLoadProperty, elevatorModelMock);
    }

    /// <summary>
    /// GetMaximumLoadProperty
    /// </summary>
    /// <returns></returns>
    private PropertyInfo GetMaximumLoadProperty()
        => GetPublicInstancePropertyNotNull<ElevatorModelMock>("MaximumLoad");

    /// <summary>
    /// GetMaximumLoadValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetMaximumLoadValue(ElevatorModelMock elevatorModelMock)
    {
        var currentLoadProperty = GetMaximumLoadProperty();
        return ValidatePropertyValue(currentLoadProperty, elevatorModelMock);
    }

    /// <summary>
    /// GetCapacityValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetCapacityValue(ElevatorModelMock elevatorModelMock)
    {
        var capacityProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("Capacity");
        return ValidatePropertyValue(capacityProperty, elevatorModelMock);
    }

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
    /// GetCurrentStatusValue
    /// </summary>
    private object? GetCurrentStatusValue(ElevatorModelMock elevatorModelMock)
    {
        var currentStatusProperty = GetCurrentStatusProperty();
        return ValidatePropertyValue(currentStatusProperty, elevatorModelMock);
    }

    /// <summary>
    /// SetCurrentStatusProperty
    /// </summary>
    /// <param name="currentStatusProperty"></param>
    /// <param name="elevatorStatus"></param>
    private void SetCurrentStatusValue(PropertyInfo currentStatusProperty, ElevatorStatus elevatorStatus)
        => currentStatusProperty.SetValue(_elevatorModelMock, elevatorStatus);

    /// <summary>
    /// GetPrimaryDirectionQueueField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetPrimaryDirectionQueueField()
        => GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_primaryDirectionQueue");

    /// <summary>
    /// GetPrimaryDirectionQueueValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetPrimaryDirectionQueueValue(ElevatorModelMock elevatorModelMock)
    {
        var primaryDirectionQueueField = GetPrimaryDirectionQueueField();
        return ValidateFieldValue(primaryDirectionQueueField, elevatorModelMock);
    }

    /// <summary>
    /// GetSecondaryDirectionQueueField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetSecondaryDirectionQueueField()
        => GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_secondaryDirectionQueue");

    /// <summary>
    /// GetSecondaryDirectionQueueValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetSecondaryDirectionQueueValue(ElevatorModelMock elevatorModelMock)
    {
        var secondaryDirectionQueueField = GetSecondaryDirectionQueueField();
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
    /// GetAcceptedRequestsField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetAcceptedRequestsField()
        => GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_acceptedRequests");

    /// <summary>
    /// GetAcceptedRequestsValue
    /// </summary>
    /// <param name="elevatorModelMock"></param>
    /// <returns></returns>
    private object? GetAcceptedRequestsValue(ElevatorModelMock elevatorModelMock)
    {
        var acceptedRequestsField = GetAcceptedRequestsField();
        return ValidateFieldValue(acceptedRequestsField, elevatorModelMock);
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

        var currentFloorValue = GetCurrentFloorValue(elevatorModelMock);
        Assert.That(currentFloorValue, Is.EqualTo(0));

        var nextFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("NextFloor");
        ValidatePropertyValue(nextFloorProperty, elevatorModelMock, false);

        var lastFloorProperty = GetPublicInstancePropertyNotNull<ElevatorModelMock>("LastFloor");
        var lastFloorValue = ValidatePropertyValue(lastFloorProperty, elevatorModelMock);
        Assert.That(lastFloorValue, Is.EqualTo(0));

        var currentLoadValue = GetCurrentLoadValue(elevatorModelMock);
        Assert.That(currentLoadValue, Is.EqualTo(0));

        var maximumLoadValue = GetMaximumLoadValue(elevatorModelMock);
        Assert.That(maximumLoadValue, Is.EqualTo(0));

        var capacityValue = GetCapacityValue(elevatorModelMock);
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

        var acceptedRequestsValue = GetAcceptedRequestsValue(elevatorModelMock);
        Assert.That(acceptedRequestsValue, Is.EqualTo(new List<ElevatorAcceptedRequest>()));

        var mapperField = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_mapper");
        ValidateFieldValue(mapperField, elevatorModelMock);

        var mockFloorMoveTimer = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_floorMoveTimer");
        ValidateFieldValue(mockFloorMoveTimer, elevatorModelMock);

        var mockDoorsOpenTimer = GetNonPublicInstanceFieldNotNull<ElevatorModelMock>("_doorsOpenTimer");
        ValidateFieldValue(mockDoorsOpenTimer, elevatorModelMock);
    }
}
