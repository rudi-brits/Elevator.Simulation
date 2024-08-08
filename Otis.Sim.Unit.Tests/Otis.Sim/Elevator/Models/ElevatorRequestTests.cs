using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorRequestTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorRequestTests : ElevatorTests
{
    /// <summary>
    /// _userInputRequest
    /// </summary>
    UserInputRequest _userInputRequest;

    /// <summary>
    /// Sets up a new Validator instance for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _userInputRequest = new UserInputRequest()
        {
            OriginFloorInput      = "-2",
            DestinationFloorInput = "5",
            CapacityInput         = "6"
        };
    }

    /// <summary>
    /// Ensure exception on empty constructor.
    /// </summary>
    [Test]
    public void EmptyConstructor_Exception()
    {
        Assert.Throws<MissingMethodException>(() => 
            Activator.CreateInstance(typeof(ElevatorRequest), true));
    }

    /// <summary>
    /// Validate value assignments.
    /// </summary>
    [Test]
    public void Constructor_InitialiseProperties()
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);

        Assert.That(elevatorRequest.ElevatorId, Is.Null);
        Assert.That(elevatorRequest.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(elevatorRequest.OriginFloor, Is.EqualTo(-2));
        Assert.That(elevatorRequest.DestinationFloor, Is.EqualTo(5));
        Assert.That(elevatorRequest.NumberOfPeople, Is.EqualTo(6));
    }

    /// <summary>
    /// Validate request status assignments.
    /// </summary>
    [Test]
    [TestCase(null, RequestStatus.Pending)]
    [TestCase(0, RequestStatus.Complete)]
    [TestCase(1, RequestStatus.Assigned)]
    public void Constructor_InitialiseProperties(int? elevatorId, RequestStatus expectedRequestStatus)
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        elevatorRequest.ElevatorId = elevatorId;

        Assert.That(elevatorRequest.RequestStatus, Is.EqualTo(expectedRequestStatus));
    }

    /// <summary>
    /// Validate ElevatorDirection.
    /// </summary>
    [Test]
    [TestCase(1, 4, ElevatorDirection.Up)]
    [TestCase(4, 1, ElevatorDirection.Down)]
    [TestCase(-1, -4, ElevatorDirection.Down)]
    [TestCase(-4, -1, ElevatorDirection.Up)]
    [TestCase(-1, 4, ElevatorDirection.Up)]
    [TestCase(4, -1, ElevatorDirection.Down)]
    public void Constructor_InitialiseProperties(int originFloor, int destinationFloor,
        ElevatorDirection expectedDirection)
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        elevatorRequest.OriginFloor      = originFloor;
        elevatorRequest.DestinationFloor = destinationFloor;

        Assert.That(elevatorRequest.Direction, Is.EqualTo(expectedDirection));
    }

    /// <summary>
    /// Valiadate the ToDuplicateRequestString function.
    /// </summary>
    [Test]
    public void ToDuplicateRequestString_Valid()
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        ValidateToString(elevatorRequest.ToDuplicateRequestString(), elevatorRequest);
    }

    /// <summary>
    /// Valiadate the ToQueuedRequestString function.
    /// </summary>
    [Test]
    public void ToQueuedRequestString_Valid()
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        var requestString   = elevatorRequest.ToQueuedRequestString();

        ValidateToString(elevatorRequest.ToQueuedRequestString(), elevatorRequest);

        StringAssert.Contains(
            $"{nameof(ElevatorRequest.Id)}: {elevatorRequest.Id}, ", requestString);
        StringAssert.Contains(
            $"{OtisSimConstants.PeopleName}: {elevatorRequest.NumberOfPeople}, ", requestString);
        StringAssert.Contains(
            $"{OtisSimConstants.Status}: {elevatorRequest.RequestStatus}, ", requestString);
    }

    /// <summary>
    /// Valiadate generic results.
    /// </summary>
    private void ValidateToString(string value, ElevatorRequest elevatorRequest)
    {
        StringAssert.Contains(
            $"{OtisSimConstants.OriginFloorName}: {elevatorRequest.OriginFloor}, ", value);
        StringAssert.Contains(
            $"{OtisSimConstants.DestinationFloorName}: {elevatorRequest.DestinationFloor}, ", value);
        StringAssert.Contains(
            $"{nameof(ElevatorRequest.Direction)}: {elevatorRequest.Direction}", value);
    }
}
