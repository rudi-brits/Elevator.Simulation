using Moq;
using Moq.Protected;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Constants;
using Otis.Sim.Unit.Tests.Configuration;
using System.Text.Json;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Configuration.Services;

/// <summary>
/// Class OtisConfigurationServiceTests extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class OtisConfigurationServiceTests : ConfigurationTests
{
    /// <summary>
    /// The Mock<OtisConfigurationService> instance.
    /// </summary>
    private Mock<OtisConfigurationService> _mockOtisConfigurationService;

    /// <summary>
    /// Sets up a new Validator instance for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _mockOtisConfigurationService = new Mock<OtisConfigurationService>();
    }

    /// <summary>
    /// Test json in appsettings.json success.
    /// </summary>
    [Test]
    public void LoadConfiguration_Valid()
    {
        var validJson = @"
        {
            ""BuildingConfiguration"": {
                ""LowestFloor"": -2,
                ""HighestFloor"": 10,
                ""MaximumElevatorLoad"": 100
            },
            ""ElevatorsConfiguration"": [
                { ""Description"": ""Elevator 1"", ""LowestFloor"": 0 },
                { ""Description"": ""Elevator 2"", ""HighestFloor"": 9 },
                { ""Description"": ""Elevator 3"", ""LowestFloor"": -1, ""HighestFloor"": 6 }
            ]
        }";

        SetupMockReadAppSettings(validJson);

        var _mockObject = _mockOtisConfigurationService.Object;
        _mockObject.LoadConfiguration();

        Assert.NotNull(_mockObject.BuildingConfiguration);
        Assert.NotNull(_mockObject.ElevatorsConfiguration);
        Assert.That(_mockObject.ElevatorsConfiguration.Count, Is.EqualTo(3));
    }

    /// <summary>
    /// Test invalid json in appsettings.json error.
    /// </summary>
    [Test]
    public void LoadConfiguration_InvalidJson_Error()
    {
        // Missing closing bracket after: ""HighestFloor"": 6 
        var invalidJson = @"
        {
            ""BuildingConfiguration"": {
                ""LowestFloor"": -2,
                ""HighestFloor"": 10,
                ""MaximumElevatorLoad"": 100
            },
            ""ElevatorsConfiguration"": [
                { ""Description"": ""Elevator 1"", ""LowestFloor"": 0 },
                { ""Description"": ""Elevator 2"", ""HighestFloor"": 9 },
                { ""Description"": ""Elevator 3"", ""LowestFloor"": -1, ""HighestFloor"": 6 
            ]
        }";

        SetupMockReadAppSettings(invalidJson);

        Assert.Throws<JsonException>(() => _mockOtisConfigurationService.Object.LoadConfiguration());
    }

    /// <summary>
    /// Test invalid BuildingConfiguration error.
    /// </summary>
    [Test]
    public void LoadConfiguration_InvalidBuildingConfiguration_Error()
    {
        var json = @"
        {
            ""BuildingConfiguration"": {
                ""LowestFloor"": -2,
                ""HighestFloor"": 10,
                ""MaximumElevatorLoad"": 0
            },
            ""ElevatorsConfiguration"": [
                { ""Description"": ""Elevator 1"", ""LowestFloor"": 0 },
                { ""Description"": ""Elevator 2"", ""HighestFloor"": 9 },
                { ""Description"": ""Elevator 3"", ""LowestFloor"": -1, ""HighestFloor"": 6 }
            ]
        }";

        SetupMockReadAppSettings(json);

        var buildingConfiguration = new BuildingConfiguration();
        var errorMessage = MessageService.FormatMessage(
           MessageService.MustBeLargerThanOtherValue,
           nameof(buildingConfiguration.MaximumElevatorLoad),
           OtisSimConstants.Zero);

        Assert.Throws<ArgumentException>(() => 
            _mockOtisConfigurationService.Object.LoadConfiguration(), errorMessage);
    }

    /// <summary>
    /// Test invalid ElevatorsConfiguration error.
    /// </summary>
    [Test]
    public void LoadConfiguration_InvalidElevatorsConfiguration_Error()
    {
        var json = @"
        {
            ""BuildingConfiguration"": {
                ""LowestFloor"": -2,
                ""HighestFloor"": 10,
                ""MaximumElevatorLoad"": 2
            },
            ""ElevatorsConfiguration"": [
                { ""Description"": null, ""LowestFloor"": 0 },
                { ""Description"": ""Elevator 2"", ""HighestFloor"": 9 },
                { ""Description"": ""Elevator 3"", ""LowestFloor"": -1, ""HighestFloor"": 6 }
            ]
        }";

        SetupMockReadAppSettings(json);

        var elevatorConfiguration = new ElevatorConfiguration();
        var errorMessage = MessageService.FormatMessage(
            MessageService.MayNotBeNullOrEmpty,
            nameof(elevatorConfiguration.Description));

        Assert.Throws<ArgumentException>(() => 
            _mockOtisConfigurationService.Object.LoadConfiguration(), errorMessage);
    }

    /// <summary>
    /// Helper method to setup mock ReadAppSettings with a json string result.
    /// </summary>
    /// <param name="jsonString"></param>
    private void SetupMockReadAppSettings(string jsonString)
    {
        _mockOtisConfigurationService
            .Protected()
            .Setup<string>("ReadAppSettings")
            .Returns(jsonString);
    }
}
