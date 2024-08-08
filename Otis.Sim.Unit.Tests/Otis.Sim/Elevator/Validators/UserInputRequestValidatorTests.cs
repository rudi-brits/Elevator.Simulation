using FluentValidation.TestHelper;
using NStack;
using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Validators;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Validators;

/// <summary>
/// Class UserInputRequestValidatorTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class UserInputRequestValidatorTests : ElevatorTests
{
    /// <summary>
    /// Validator instance to be tested.
    /// </summary>
    private UserInputRequestValidator _validator;

    /// <summary>
    /// UserInputRequest instance to be tested.
    /// </summary>
    private UserInputRequest _userInputRequest;

    /// <summary>
    /// Sets up a new Validator instance for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _validator        = new UserInputRequestValidator();
        _userInputRequest = new UserInputRequest()
        {
            OriginFloorInput      = "1",
            DestinationFloorInput = "1",
            CapacityInput         = "10",
        };
    }

    /// <summary>
    /// Properties as valid integers.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="inputValue"></param>
    /// <param name="expectedValue"></param>
    /// <param name="floorName"></param>
    [Test]
    // OriginFloorInput
    [TestCase(nameof(UserInputRequest.OriginFloorInput), "-1", -1, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(UserInputRequest.OriginFloorInput), "0", 0, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(UserInputRequest.OriginFloorInput), "1", 1, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(UserInputRequest.OriginFloorInput), "abs", null, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(UserInputRequest.OriginFloorInput), "", null, OtisSimConstants.OriginFloorName)]
    [TestCase(nameof(UserInputRequest.OriginFloorInput), "-", null, OtisSimConstants.OriginFloorName)]
    // DestinationFloorInput
    [TestCase(nameof(UserInputRequest.DestinationFloorInput), "-1", -1, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(UserInputRequest.DestinationFloorInput), "0", 0, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(UserInputRequest.DestinationFloorInput), "1", 1, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(UserInputRequest.DestinationFloorInput), "abs", null, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(UserInputRequest.DestinationFloorInput), "", null, OtisSimConstants.DestinationFloorName)]
    [TestCase(nameof(UserInputRequest.DestinationFloorInput), "-", null, OtisSimConstants.DestinationFloorName)]
    // CapacityInput
    [TestCase(nameof(UserInputRequest.CapacityInput), "-1", -1, OtisSimConstants.NumberOfPeopleName)]
    [TestCase(nameof(UserInputRequest.CapacityInput), "0", 0, OtisSimConstants.NumberOfPeopleName)]
    [TestCase(nameof(UserInputRequest.CapacityInput), "1", 1, OtisSimConstants.NumberOfPeopleName)]
    [TestCase(nameof(UserInputRequest.CapacityInput), "abs", null, OtisSimConstants.NumberOfPeopleName)]
    [TestCase(nameof(UserInputRequest.CapacityInput), "", null, OtisSimConstants.NumberOfPeopleName)]
    [TestCase(nameof(UserInputRequest.CapacityInput), "-", null, OtisSimConstants.NumberOfPeopleName)]
    public void FloorInput_MustBeAnInteger(string propertyName, string inputValue, int? expectedValue, string floorName)
    {
        var property = typeof(UserInputRequest).GetProperty(propertyName);
        Assert.That(property, Is.Not.Null);

        property.SetValue(_userInputRequest, ustring.Make(inputValue));

        var errorMessage = MessageService.FormatMessage(
            MessageService.MustBeValidInteger,
            floorName);

        var result = _validator.TestValidate(_userInputRequest);

        if (expectedValue != null)
            result.ShouldNotHaveValidationErrorFor(propertyName);
        else
            result.ShouldHaveValidationErrorFor(propertyName)
                .Where(message => message.ErrorMessage.Contains(errorMessage));
    }
}
