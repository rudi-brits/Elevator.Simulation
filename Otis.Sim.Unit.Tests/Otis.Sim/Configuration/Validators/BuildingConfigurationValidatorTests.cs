using FluentValidation.TestHelper;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Validators;
using Otis.Sim.Constants;
using Otis.Sim.Unit.Tests.Configuration;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Configuration.Validators;

/// <summary>
/// Class BuildingConfigurationValidatorTests extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class BuildingConfigurationValidatorTests : ConfigurationTests
{
    /// <summary>
    /// Validator instance to be tested.
    /// </summary>
    private BuildingConfigurationValidator _validator;

    /// <summary>
    /// Sets up a new Validator instance for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _validator = new BuildingConfigurationValidator();
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
        var buildingConfiguration = new BuildingConfiguration
        {
            LowestFloor         = lowestFloor,
            HighestFloor        = highestFloor,
            MaximumElevatorLoad = 20
        };

        var errorMessage = MessageService.FormatMessage(
            MessageService.MustBeLargerThanOtherValue,
            nameof(buildingConfiguration.HighestFloor),
            nameof(buildingConfiguration.LowestFloor));

        var result = _validator.TestValidate(buildingConfiguration);
        result.ShouldHaveValidationErrorFor(m => m.HighestFloor)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with correct error message when MaximumElevatorLoad <= 0.
    /// </summary>
    /// <param name="lowestFloor"></param>
    /// <param name="highestFloor"></param>
    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public void MaximumElevatorLoad_IsNotLargerThan_Zero_Error(int maximumElevatorLoad)
    {
        var buildingConfiguration = new BuildingConfiguration
        {
            LowestFloor         = 5,
            HighestFloor        = 8,
            MaximumElevatorLoad = maximumElevatorLoad
        };

        var errorMessage = MessageService.FormatMessage(
            MessageService.MustBeLargerThanOtherValue,
            nameof(buildingConfiguration.MaximumElevatorLoad),
            OtisSimConstants.Zero);

        var result = _validator.TestValidate(buildingConfiguration);
        result.ShouldHaveValidationErrorFor(m => m.MaximumElevatorLoad)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with no error message when all values are valid.
    /// </summary>
    [Test]
    public void ValidConfigurationValues_NoError()
    {
        var buildingConfiguration = new BuildingConfiguration
        {
            LowestFloor = 5,
            HighestFloor = 8,
            MaximumElevatorLoad = 4
        };

        var result = _validator.TestValidate(buildingConfiguration);
        result.ShouldNotHaveAnyValidationErrors();
    }
}