using AutoMapper;
using Moq;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Unit.Tests.Otis.Sim.Elevator.MockClasses;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Services;

/// <summary>
/// Class ElevatorConfigurationServiceTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorConfigurationServiceTests : ElevatorTests
{
    /// <summary>
    /// Mock<IMapper> field.
    /// </summary>
    private IMapper _mapper;
    /// <summary>
    /// OtisConfigurationService field
    /// </summary>
    private Mock<OtisConfigurationService> _mockConfigurationService;
    /// <summary>
    /// ConcreteElevatorConfigurationServiceMock field
    /// </summary>
    private ConcreteElevatorConfigurationServiceMock _elevatorConfigurationService;
    /// <summary>
    /// BuildingConfiguration field
    /// </summary>
    private BuildingConfiguration _buildingConfiguration;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper                   = SetupIMapper();
        _mockConfigurationService = new Mock<OtisConfigurationService>();

        _buildingConfiguration = new BuildingConfiguration
        {
            LowestFloor         = 1,
            HighestFloor        = 10,
            MaximumElevatorLoad = 1000
        };
    }

    /// <summary>
    /// SetUp before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _elevatorConfigurationService = new ConcreteElevatorConfigurationServiceMock(
            _mockConfigurationService.Object,
            _mapper);
    }

    /// <summary>
    /// Validate instance fields.
    /// </summary>
    [Test]
    public void Field_DefaultValues()
    {
        var elevatorsField = GetNonPublicInstanceFieldNotNull<ConcreteElevatorConfigurationServiceMock>("_elevators");
        var elevatorsValue = ValidateFieldValue(elevatorsField, _elevatorConfigurationService);
        Assert.That(elevatorsValue, Is.EqualTo(new List<ElevatorModel>()));

        var elevatorRequestValidationValuesField =
            GetNonPublicInstanceFieldNotNull<ConcreteElevatorConfigurationServiceMock>("_elevatorRequestValidationValues");
        ValidateFieldValue(elevatorRequestValidationValuesField, _elevatorConfigurationService, false);
    }

    /// <summary>
    /// Test constructor assignments.
    /// </summary>
    [Test]
    public void Constructor_ObjectAssignments_Success()
    {
        var configurationServiceField 
            = GetNonPublicInstanceFieldNotNull<ElevatorConfigurationService>("_configurationService");
        var configurationServiceValue = ValidateFieldValue(configurationServiceField, _elevatorConfigurationService);
        Assert.That(configurationServiceValue, Is.EqualTo(_mockConfigurationService.Object));

        var mapperField = GetNonPublicInstanceFieldNotNull<ElevatorConfigurationService>("_mapper");
        var mapperValue = ValidateFieldValue(mapperField, _elevatorConfigurationService);

        Assert.That(mapperValue, Is.Not.Null);
        Assert.That(mapperValue, Is.EqualTo(_mapper));
    }

    /// <summary>
    /// LoadConfiguration with elevator parameters within that of the building.
    /// </summary>
    [Test]
    public void LoadConfiguration_ElevatorsWithValuesInBuildingRange()
    {
        var elevatorsConfiguration = new List<ElevatorConfiguration>
        {
            new ElevatorConfiguration
            {
                Description  = "Elevator 1",
                LowestFloor  = 1,
                HighestFloor = 5
            },
            new ElevatorConfiguration
            {
                Description  = "Elevator 2",
                LowestFloor  = 2,
                HighestFloor = 9
            }
        };

        var elevators = LoadConfiguration(elevatorsConfiguration, 2);

        var elevatorIds = elevators.Select(e => e.Id).ToList();
        Assert.That(elevatorIds.Distinct().Count(), Is.EqualTo(elevatorIds.Count));

        for (var i = 0; i < elevators.Count; i++)
        {
            var elevator       = elevators[i];
            var elevatorConfig = elevatorsConfiguration[i];

            Assert.That(elevator.Id > 0);
            Assert.That(elevator.Description, Is.EqualTo(elevatorConfig.Description.Trim()));
            Assert.That(elevator.LowestFloor, Is.EqualTo(elevatorConfig.LowestFloor));
            Assert.That(elevator.HighestFloor, Is.EqualTo(elevatorConfig.HighestFloor));
            Assert.That(elevator.MaximumLoad, Is.EqualTo(_buildingConfiguration.MaximumElevatorLoad));
            Assert.That(elevator.CompleteRequest, Is.Not.Null);
            Assert.That(elevator.RequeueRequest, Is.Not.Null);
            Assert.That(elevator.PrintRequestStatus, Is.Not.Null);
        }
    }

    /// <summary>
    /// LoadConfiguration with elevator parameters within that of the building.
    /// </summary>
    [Test]
    public void LoadConfiguration_ElevatorsWithMissingFloorParameters()
    {
        var elevatorsConfiguration = new List<ElevatorConfiguration>
        {
            new ElevatorConfiguration
            {
                Description  = "Elevator 1",
                HighestFloor = 7
            },
            new ElevatorConfiguration
            {
                Description  = "Elevator 2",
                LowestFloor  = 2,
            },
            new ElevatorConfiguration
            {
                Description  = "Elevator 3"
            }
        };

        var elevators = LoadConfiguration(elevatorsConfiguration, 3);

        Assert.That(elevators[0].LowestFloor, Is.EqualTo(_buildingConfiguration.LowestFloor));
        Assert.That(elevators[0].HighestFloor, Is.EqualTo(elevatorsConfiguration[0].HighestFloor));

        Assert.That(elevators[1].LowestFloor, Is.EqualTo(elevatorsConfiguration[1].LowestFloor));
        Assert.That(elevators[1].HighestFloor, Is.EqualTo(_buildingConfiguration.HighestFloor));

        Assert.That(elevators[2].LowestFloor, Is.EqualTo(_buildingConfiguration.LowestFloor));
        Assert.That(elevators[2].HighestFloor, Is.EqualTo(_buildingConfiguration.HighestFloor));
    }

    /// <summary>
    /// LoadConfiguration with elevator parameters outside that of the building.
    /// </summary>
    [Test]
    public void LoadConfiguration_ElevatorsWithOutOfBoundsFloorParameters()
    {
        var elevatorsConfiguration = new List<ElevatorConfiguration>
        {
            new ElevatorConfiguration
            {
                Description  = "Elevator 1",
                HighestFloor = 11
            },
            new ElevatorConfiguration
            {
                Description  = "Elevator 2",
                LowestFloor  = 2,
            },
            new ElevatorConfiguration
            {
                Description  = "Elevator 3",
                LowestFloor  = -1,
                HighestFloor = 6
            }
        };

        var elevators = LoadConfiguration(elevatorsConfiguration, 3);

        Assert.That(elevators[0].LowestFloor, Is.EqualTo(_buildingConfiguration.LowestFloor));
        Assert.That(elevators[0].HighestFloor, Is.EqualTo(_buildingConfiguration.HighestFloor));

        Assert.That(elevators[1].LowestFloor, Is.EqualTo(elevatorsConfiguration[1].LowestFloor));
        Assert.That(elevators[1].HighestFloor, Is.EqualTo(_buildingConfiguration.HighestFloor));

        Assert.That(elevators[2].LowestFloor, Is.EqualTo(_buildingConfiguration.LowestFloor));
        Assert.That(elevators[2].HighestFloor, Is.EqualTo(elevatorsConfiguration[2].HighestFloor));
    }

    /// <summary>
    /// LoadConfiguration with elevator parameters outside that of the building.
    /// </summary>
    [Test]
    public void LoadConfiguration_ElevatorRequestValidationValues()
    {
        var elevatorsConfiguration = new List<ElevatorConfiguration>
        {
            new ElevatorConfiguration
            {
                Description  = "Elevator 1",
                LowestFloor  = 3,
                HighestFloor = 9,
            },
            new ElevatorConfiguration
            {
                Description  = "Elevator 2",
                LowestFloor  = 2,
                HighestFloor = 8,
            },
            new ElevatorConfiguration
            {
                Description  = "Elevator 3",
                LowestFloor  = 4,
                HighestFloor = 7
            }
        };

        LoadConfiguration(elevatorsConfiguration, 3);

        var elevatorRequestValidationValuesField =
            GetNonPublicInstanceFieldNotNull<ElevatorConfigurationService>("_elevatorRequestValidationValues");

        var elevatorRequestValidationValues 
            = ValidateFieldValue(elevatorRequestValidationValuesField, _elevatorConfigurationService);

        var model = elevatorRequestValidationValues as ElevatorRequestValidationValues;

        Assert.That(model, Is.Not.Null);
        Assert.That(model.LowestFloor, Is.EqualTo(2));
        Assert.That(model.HighestFloor, Is.EqualTo(9));
        Assert.That(model.MaximumLoad, Is.EqualTo(_buildingConfiguration.MaximumElevatorLoad));
    }

    /// <summary>
    /// Confirm abstract methods are implemented
    /// </summary>
    [Test]
    public void Confirm_AbstractMethods_AreImplemented()
    {
        var completeRequestMethod =
            GetNonPublicInstanceMethod<ConcreteElevatorConfigurationServiceMock>("CompleteRequest");
        Assert.That(completeRequestMethod, Is.Not.Null);

        var requeueRequestMethod =
            GetNonPublicInstanceMethod<ConcreteElevatorConfigurationServiceMock>("RequeueRequest");
        Assert.That(requeueRequestMethod, Is.Not.Null);

        var printRequestStatus =
            GetNonPublicInstanceMethod<ConcreteElevatorConfigurationServiceMock>("PrintRequestStatus");
        Assert.That(printRequestStatus, Is.Not.Null);
    }

    /// <summary>
    /// LoadConfiguration with elevator parameters outside that of the building.
    /// </summary>
    [Test]
    public void PrintLoadedConfiguration_WithoutExceptions()
    {
        var elevatorsConfiguration = new List<ElevatorConfiguration>
        {
            new ElevatorConfiguration
            {
                Description  = "Elevator 1",
                LowestFloor  = 3,
                HighestFloor = 9,
            }
        };

        _elevatorConfigurationService.CallBasePrintLoadedConfiguration = true;
        Assert.DoesNotThrow(() => LoadConfiguration(elevatorsConfiguration, 1));
        Assert.That(_elevatorConfigurationService.CalledPrintLoadedConfiguration, Is.True);
    }

    /// <summary>
    /// Load the config and return a list of elevators
    /// </summary>
    /// <param name="elevatorConfigurations"></param>
    /// <param name="elevatorsCount"></param>
    /// <returns></returns>
    private List<ElevatorModel> LoadConfiguration(List<ElevatorConfiguration> elevatorConfigurations,
        int elevatorsCount)
    {
        var otisConfiguration = new OtisConfiguration()
        {
            BuildingConfiguration  = _buildingConfiguration,
            ElevatorsConfiguration = elevatorConfigurations
        };

        var configurationField = GetNonPublicInstancePropertyNotNull<OtisConfigurationService>("_configuration");
        configurationField.SetValue(_mockConfigurationService.Object, otisConfiguration);

        _elevatorConfigurationService.LoadConfiguration();

        var elevatorsField 
            = GetNonPublicInstanceFieldNotNull<ConcreteElevatorConfigurationServiceMock>("_elevators");
        var elevatorsValue = ValidateFieldValue(elevatorsField, _elevatorConfigurationService);

        var elevatorsModel = elevatorsValue as List<ElevatorModel>;
        Assert.That(elevatorsModel?.Count, Is.EqualTo(elevatorsCount));

        return elevatorsModel;
    }
}
