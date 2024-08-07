using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorEnum extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorAcceptedRequestTests : ElevatorTests
{
    private Guid _id;
    private ElevatorAcceptedRequest _elevatorAcceptedRequest;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _id = Guid.NewGuid();

        _elevatorAcceptedRequest = new ElevatorAcceptedRequest
        {
            Id                       = _id,
            OriginFloor              = 1,
            DestinationFloor         = 5,
            NumberOfPeople           = 3,
            ElevatorName             = "Elevator A",
            RequestDirection         = ElevatorDirection.Down,
            OriginFloorServiced      = false,
            DestinationFloorServiced = false
        };
    }

    [Test]
    public void Validate_DefaultValues()
    {
        var elevatorRequest = new ElevatorAcceptedRequest();

        Assert.That(elevatorRequest.Id, Is.EqualTo(Guid.Empty));
        Assert.That(elevatorRequest.OriginFloor, Is.EqualTo(0));
        Assert.That(elevatorRequest.DestinationFloor, Is.EqualTo(0));
        Assert.That(elevatorRequest.NumberOfPeople, Is.EqualTo(0));
        Assert.That(elevatorRequest.ElevatorName, Is.EqualTo(string.Empty));
        Assert.That(elevatorRequest.RequestDirection, Is.EqualTo(ElevatorDirection.Up));
        Assert.That(elevatorRequest.OriginFloorServiced, Is.EqualTo(false));
        Assert.That(elevatorRequest.DestinationFloorServiced, Is.EqualTo(false));
        Assert.That(elevatorRequest.Completed, Is.EqualTo(false));
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    public void Validate_Completed_FromServicedFloors(bool originFloorServiced, bool destinationFloorServiced)
    {
        var elevatorRequest = new ElevatorAcceptedRequest()
        {
            OriginFloorServiced      = originFloorServiced,
            DestinationFloorServiced = destinationFloorServiced
        };

        if (originFloorServiced && destinationFloorServiced)
            Assert.That(elevatorRequest.Completed, Is.EqualTo(true));
        else
            Assert.That(elevatorRequest.Completed, Is.EqualTo(false));
    }

    [Test]
    public void Validate_ToAcceptedRequestString()
    {
        var acceptedRequestString = _elevatorAcceptedRequest.ToAcceptedRequestString();
        ValidateToString(acceptedRequestString, OtisSimConstants.Accepted);
    }

    [Test]
    [TestCase(OtisSimConstants.PickUp)]
    [TestCase(OtisSimConstants.DropOff)]
    public void Validate_ToPickedUp_ToDroppedOff_RequestString(string status)
    {
        int numberOfPeople = 3;
        int capacity       = 8;

        var requestString = status == OtisSimConstants.PickUp
            ? _elevatorAcceptedRequest.ToPickedUpRequestString(numberOfPeople, capacity)
            : _elevatorAcceptedRequest.ToDroppedOffRequestString(numberOfPeople, capacity);

        ValidateToString(requestString, $"{ElevatorStatus.DoorsOpen} ({status})");

        StringAssert.Contains(
            $"{OtisSimConstants.PeopleName}: {numberOfPeople}, ", requestString);
        StringAssert.Contains(
            $"{OtisSimConstants.Capacity}: {capacity}, ", requestString);
    }

    [Test]
    public void Validate_ToCompletedRequestString()
    {
        var completedRequestString = _elevatorAcceptedRequest.ToCompletedRequestString();
        ValidateToString(completedRequestString, OtisSimConstants.Completed);
    }

    [Test]
    public void Validate_ToRequeuedRequestString()
    {
        var requeuedRequestString = _elevatorAcceptedRequest.ToRequeuedRequestString();
        ValidateToString(requeuedRequestString, OtisSimConstants.Requeued);
    }

    private void ValidateToString(string value, string status)
    {
        StringAssert.Contains(
            $"{nameof(_elevatorAcceptedRequest.Id)}: {_elevatorAcceptedRequest.Id}, ", value);
        StringAssert.Contains(
            $"{OtisSimConstants.OriginFloorName}: {_elevatorAcceptedRequest.OriginFloor}, ", value);
        StringAssert.Contains(
            $"{OtisSimConstants.DestinationFloorName}: {_elevatorAcceptedRequest.DestinationFloor}, ", value);
        StringAssert.Contains(
            $"{OtisSimConstants.PeopleName}: {_elevatorAcceptedRequest.NumberOfPeople}, ", value);
        StringAssert.Contains(
            $"{OtisSimConstants.Status}: {status}, ", value);
        StringAssert.Contains(
            $"{nameof(_elevatorAcceptedRequest.RequestDirection)}: {_elevatorAcceptedRequest.RequestDirection}, ", value);
        StringAssert.Contains(
            $"{OtisSimConstants.Elevator}: {_elevatorAcceptedRequest.ElevatorName}", value);
    }
}
