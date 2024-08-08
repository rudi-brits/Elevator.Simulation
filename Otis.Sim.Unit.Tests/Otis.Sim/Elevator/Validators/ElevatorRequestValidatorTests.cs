using FluentValidation.TestHelper;
using NStack;
using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Validators;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Validators;

/// <summary>
/// Class ElevatorRequestValidatorTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorRequestValidatorTests : ElevatorTests
{
    /// <summary>
    /// Validator instance to be tested.
    /// </summary>
    private ElevatorRequestValidator _validator;

    /// <summary>
    /// ElevatorRequestValidationValues
    /// </summary>
    private ElevatorRequestValidationValues _validationValues;

    /// <summary>
    /// UserInputRequest instance to be tested.
    /// </summary>
    private UserInputRequest _userInputRequest;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _validationValues = new ElevatorRequestValidationValues()
        {
            LowestFloor  = -2,
            HighestFloor = 10,
            MaximumLoad  = 12
        };
    }

    /// <summary>
    /// Sets up a new Validator instance for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _validator = new ElevatorRequestValidator(_validationValues);
        _userInputRequest = new UserInputRequest()
        {
            OriginFloorInput      = "5",
            DestinationFloorInput = "6",
            CapacityInput         = "10",
        };
    }

    /// <summary>
    /// Test with originFloor == destinationFloor.
    /// </summary>
    [Test]
    public void OriginFloor_EqualTo_Destination_Error()
    {
        _userInputRequest.OriginFloorInput      = "2";
        _userInputRequest.DestinationFloorInput = _userInputRequest.OriginFloorInput;

        var errorMessage = MessageService.FormatMessage(
            MessageService.MayNotBeEqualTo,
            OtisSimConstants.OriginFloorName,
            OtisSimConstants.DestinationFloorName);

        var result = _validator.TestValidate(new ElevatorRequest(_userInputRequest));
        result.ShouldHaveValidationErrorFor(m => m.OriginFloor)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// OriginFloor & DestinationFloor in range.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="floorNumber"></param>
    /// <param name="expectedValid"></param>
    /// <param name="floorName"></param>
    [Test]
    // OriginFloor
    [TestCase(nameof(ElevatorRequest.OriginFloor), "-2", true, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(ElevatorRequest.OriginFloor), "-1", true, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(ElevatorRequest.OriginFloor), "10", true, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(ElevatorRequest.OriginFloor), "9", true, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(ElevatorRequest.OriginFloor), "-3", false, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(ElevatorRequest.OriginFloor), "11", false, OtisSimConstants.OriginFloorName)]
    // DestinationFloor
    [TestCase(nameof(ElevatorRequest.DestinationFloor), "-2", true, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(ElevatorRequest.DestinationFloor), "-1", true, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(ElevatorRequest.DestinationFloor), "10", true, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(ElevatorRequest.DestinationFloor), "9", true, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(ElevatorRequest.DestinationFloor), "-3", false, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(ElevatorRequest.DestinationFloor), "11", false, OtisSimConstants.DestinationFloorName)]
    public void Floor_InBuildingConfigurationRange_Validation(string propertyName, string floorNumber, bool expectedValid, string floorName)
    {
        var property = typeof(UserInputRequest).GetProperty($"{propertyName}Input");
        Assert.That(property, Is.Not.Null);

        property.SetValue(_userInputRequest, ustring.Make(floorNumber));

        var errorMessage = MessageService.FormatMessage(
            MessageService.MustBeWithinRange,
            floorName,
            $"{_validationValues.LowestFloor}",
            $"{_validationValues.HighestFloor}");

        var result = _validator.TestValidate(new ElevatorRequest(_userInputRequest));

        if (expectedValid)
            result.ShouldNotHaveValidationErrorFor(propertyName);
        else
            result.ShouldHaveValidationErrorFor(propertyName)
                .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// NumberOfPeople in building configuration range.
    /// </summary>
    /// <param name="numberOfPeople"></param>
    /// <param name="expectedValid"></param>
    [Test]
    [TestCase(12, true)]
    [TestCase(11, true)]
    [TestCase(1, true)]
    [TestCase(2, true)]
    [TestCase(0, false)]
    [TestCase(13, false)]
    public void NumberOfPeople_InBuildingConfigurationRange_Validation(int numberOfPeople, bool expectedValid)
    {
        UserInputRequest userInputRequest = new UserInputRequest()
        {
            OriginFloorInput      = "1",
            DestinationFloorInput = "6",
            CapacityInput         = numberOfPeople.ToString(),
        };

        var errorMessage = MessageService.FormatMessage(
            MessageService.MustBeWithinRange,
            OtisSimConstants.NumberOfPeopleName,
            $"{1}",
            $"{_validationValues.MaximumLoad}");

        var result = _validator.TestValidate(new ElevatorRequest(userInputRequest));

        if (expectedValid)
            result.ShouldNotHaveAnyValidationErrors();
        else
            result.ShouldHaveValidationErrorFor(m => m.NumberOfPeople)
                .Where(message => message.ErrorMessage.Contains(errorMessage));
    }
}
