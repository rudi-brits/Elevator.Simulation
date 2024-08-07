using FluentValidation.TestHelper;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Validators;
using Otis.Sim.Constants;
using Otis.Sim.Unit.Tests.Configuration;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Configuration.Validators;

/// <summary>
/// Class OtisConfigurationValidatorTests extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class OtisConfigurationValidatorTests : ConfigurationTests
{
    /// <summary>
    /// Validator instance to be tested.
    /// </summary>
    private OtisConfigurationValidator _validator;

    /// <summary>
    /// A valid <see cref="BuildingConfiguration" /> instance for testing.
    /// </summary>
    private BuildingConfiguration _validBuildingConfiguration = new BuildingConfiguration
    {
        LowestFloor = 0,
        HighestFloor = 10,
        MaximumElevatorLoad = 1000
    };

    /// <summary>
    /// A default instance of <see cref="ElevatorConfiguration" />.
    /// </summary>
    private ElevatorConfiguration _defaultElevatorConfiguration = new ElevatorConfiguration();

    /// <summary>
    /// A valid <see cref="ElevatorConfiguration" /> list instance for testing.
    /// </summary>
    private List<ElevatorConfiguration> _validElevatorsConfiguration = new List<ElevatorConfiguration>
    {
        new ElevatorConfiguration
        {
            Description  = "Elevator 1",
            LowestFloor  = 1,
            HighestFloor = 10
        }
    };

    /// <summary>
    /// Sets up a new Validator instance for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _validator = new OtisConfigurationValidator();
    }

    /// <summary>
    /// Test with correct error message when BuildingConfiguration is null.
    /// </summary>
    [Test]
    public void BuildingConfigurationIsNull_Error()
    {
        var otisConfiguration = new OtisConfiguration
        {
            BuildingConfiguration  = null,
            ElevatorsConfiguration = _validElevatorsConfiguration
        };

        var errorMessage = MessageService.FormatMessage(MessageService.MayNotBeNull,
            nameof(otisConfiguration.BuildingConfiguration));

        var result = _validator.TestValidate(otisConfiguration);
        result.ShouldHaveValidationErrorFor(config => config.BuildingConfiguration)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with correct error message when ElevatorConfiguration is null.
    /// </summary>
    [Test]
    public void ElevatorsConfigurationIsNull_Error()
    {
        var otisConfiguration = new OtisConfiguration
        {
            BuildingConfiguration  = _validBuildingConfiguration,
            ElevatorsConfiguration = null
        };

        var errorMessage = MessageService.FormatMessage(MessageService.MayNotBeNull,
            nameof(otisConfiguration.ElevatorsConfiguration));

        var result = _validator.TestValidate(otisConfiguration);
        result.ShouldHaveValidationErrorFor(config => config.ElevatorsConfiguration)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with correct error message when ElevatorConfiguration list is empty.
    /// </summary>
    [Test]
    public void ElevatorsConfigurationIsEmpty_Error()
    {
        var otisConfiguration = new OtisConfiguration
        {
            BuildingConfiguration  = _validBuildingConfiguration,
            ElevatorsConfiguration = new List<ElevatorConfiguration>()
        };

        var errorMessage = MessageService.FormatMessage(MessageService.MayNotBeEmpty,
            nameof(otisConfiguration.ElevatorsConfiguration));

        var result = _validator.TestValidate(otisConfiguration);
        result.ShouldHaveValidationErrorFor(config => config.ElevatorsConfiguration)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with correct error message when ElevatorConfiguration has duplicates descriptions.
    /// </summary>
    [Test]
    public void ElevatorsConfigurationDuplicateDescription_Error()
    {
        var otisConfiguration = new OtisConfiguration
        {
            BuildingConfiguration = _validBuildingConfiguration,
            ElevatorsConfiguration = new List<ElevatorConfiguration>()
            {
                new ElevatorConfiguration
                {
                    Description = "Elevator 1",
                    LowestFloor = 1,
                    HighestFloor = 10
                },
                new ElevatorConfiguration
                {
                    Description = "Elevator 1",
                    LowestFloor = 1,
                    HighestFloor = 10
                }
            }            
        };

        var errorMessage = MessageService.FormatMessage(
            MessageService.MayNotContainDuplicateValues,
            $"{OtisSimConstants.Elevator} {nameof(ElevatorConfiguration.Description)}");

        var result = _validator.TestValidate(otisConfiguration);
        result.ShouldHaveValidationErrorFor(config => config.ElevatorsConfiguration)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with no error message when all values are valid.
    /// </summary>
    [Test]
    public void ValidConfigurationValues_NoError()
    {
        var otisConfiguration = new OtisConfiguration
        {
            BuildingConfiguration  = _validBuildingConfiguration,
            ElevatorsConfiguration = _validElevatorsConfiguration
        };

        var result = _validator.TestValidate(otisConfiguration);
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Test with correct error message when BuildingConfiguration HighestFloor <= LowestFloor.
    /// </summary>
    [Test]
    public void ValidateBuildingConfiguration_Error()
    {
        var otisConfiguration = new OtisConfiguration
        {
            BuildingConfiguration  = new BuildingConfiguration 
            { 
                LowestFloor         = 0, 
                HighestFloor        = -1, 
                MaximumElevatorLoad = 1000 
            },
            ElevatorsConfiguration = _validElevatorsConfiguration
        };

        var errorMessage = MessageService.FormatMessage(
            MessageService.MustBeLargerThanOtherValue,
            nameof(otisConfiguration.BuildingConfiguration.HighestFloor),
            nameof(otisConfiguration.BuildingConfiguration.LowestFloor));


        var result = _validator.TestValidate(otisConfiguration);
        result.ShouldHaveValidationErrorFor(config => config.BuildingConfiguration!.HighestFloor)
            .Where(message => message.ErrorMessage.Contains(errorMessage));
    }

    /// <summary>
    /// Test with correct error message when 
    /// ElevatorsConfiguration[0] description is null
    /// ElevatorsConfiguration[1] description is empty
    /// ElevatorsConfiguration[2] HighestFloor <= LowestFloor
    /// ElevatorsConfiguration[3] is valid
    /// </summary>
    public void ValidateElevatorsConfiguration_Errors()
    {
        var otisConfiguration = new OtisConfiguration
        {
            BuildingConfiguration  = _validBuildingConfiguration,
            ElevatorsConfiguration = new List<ElevatorConfiguration>
            {
                new ElevatorConfiguration
                {
                    LowestFloor  = 1,
                    HighestFloor = 10
                },
                new ElevatorConfiguration
                {
                    Description  = "       ",
                    LowestFloor  = 1,
                    HighestFloor = 10
                },
                new ElevatorConfiguration
                {
                    Description  = "Elevator 1",
                    LowestFloor  = 1,
                    HighestFloor = 0
                },
                new ElevatorConfiguration
                {
                    Description  = "Elevator 2",
                    LowestFloor  = 0,
                    HighestFloor = 1
                }
            }
        };

        var highestFloorErrorMessage = MessageService.FormatMessage(
            MessageService.MustBeLargerThanOtherValue,
            nameof(_defaultElevatorConfiguration.HighestFloor),
            nameof(_defaultElevatorConfiguration.LowestFloor));

        var notNullOrEmptyErrorMessage = MessageService.FormatMessage(MessageService.MayNotBeNullOrEmpty,
            nameof(_defaultElevatorConfiguration.Description));

        var result = _validator.TestValidate(otisConfiguration);
        result.ShouldHaveValidationErrorFor(config => config.ElevatorsConfiguration![0].Description)
            .Where(message => message.ErrorMessage.Contains(notNullOrEmptyErrorMessage));
        result.ShouldHaveValidationErrorFor(config => config.ElevatorsConfiguration![1].Description)
            .Where(message => message.ErrorMessage.Contains(notNullOrEmptyErrorMessage));
        result.ShouldHaveValidationErrorFor(config => config.ElevatorsConfiguration![2].HighestFloor)
            .Where(message => message.ErrorMessage.Contains(highestFloorErrorMessage));
        result.ShouldNotHaveValidationErrorFor(config => config.ElevatorsConfiguration![3]);
    }
}
