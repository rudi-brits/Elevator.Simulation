using FluentValidation.TestHelper;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Validators;
using Otis.Sim.Unit.Tests.Configuration;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Configuration.Validators;

/// <summary>
/// Class BuildingConfigurationValidatorTests extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class ElevatorConfigurationValidatorTests : ConfigurationTests
{
    /// <summary>
    /// Validator instance to be tested.
    /// </summary>
    private ElevatorConfigurationValidator _validator;

    /// <summary>
    /// Sets up a new Validator instance for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _validator = new ElevatorConfigurationValidator();
    }

    /// <summary>
    /// Test with correct error message when Description is null or empty.
    /// </summary>
    /// <param name="description"></param>
    [Test]
    [TestCase(null)]
    [TestCase("")]
    public void EmptyOrNull_Description_Error(string description)
    {
        var elevatorConfiguration = new ElevatorConfiguration
        {
            Description = description,
            LowestFloor = 5,
            HighestFloor = 6
        };

        var errorMessage = MessageService.FormatMessage(
            MessageService.MayNotBeNullOrEmpty,
            nameof(elevatorConfiguration.Description));

        var result = _validator.TestValidate(elevatorConfiguration);
        result.ShouldHaveValidationErrorFor(m => m.Description)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with correct error message when HighestFloor <= LowestFloor.
    /// </summary>
    /// <param name="lowestFloor"></param>
    /// <param name="highestFloor"></param>
    [Test]
    [TestCase(5, 5)]
    [TestCase(6, 5)]
    [TestCase(-1, -2)]
    public void HighestFloor_IsNotLargerThan_LowestFloor_Error(int lowestFloor, int highestFloor)
    {
        var elevatorConfiguration = new ElevatorConfiguration
        {
            Description = "Elevator 1",
            LowestFloor = lowestFloor,
            HighestFloor = highestFloor
        };

        var errorMessage = MessageService.FormatMessage(
            MessageService.MustBeLargerThanOtherValue,
            nameof(elevatorConfiguration.HighestFloor),
            nameof(elevatorConfiguration.LowestFloor));

        var result = _validator.TestValidate(elevatorConfiguration);
        result.ShouldHaveValidationErrorFor(m => m.HighestFloor)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with no error message when all values are valid.
    /// </summary>
    /// <param name="lowestFloor"></param>
    /// <param name="highestFloor"></param>
    [Test]
    [TestCase(5, 8)]
    [TestCase(-2, -1)]
    [TestCase(null, -1)]
    [TestCase(2, null)]
    public void ValidConfigurationValues_NoError(int? lowestFloor, int? highestFloor)
    {
        var elevatorConfiguration = new ElevatorConfiguration
        {
            Description  = "Elevator 1",
            LowestFloor  = lowestFloor,
            HighestFloor = highestFloor
        };

        var result = _validator.TestValidate(elevatorConfiguration);
            result.ShouldNotHaveAnyValidationErrors();
    }
}
