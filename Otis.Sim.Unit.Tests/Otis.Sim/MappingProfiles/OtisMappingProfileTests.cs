using AutoMapper;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Unit.Tests.Constants;
using Otis.Sim.Unit.Tests.TestBase.Services;

namespace Otis.Sim.Unit.Tests.Otis.Sim.MappingProfiles;

/// <summary>
/// Class OtisMappingProfileTests extends the <see cref="BaseTestService" /> class.
/// </summary>
/// <category><see cref="TestConstants.MappingProfilesCategory" /></category>
[TestFixture(Category = TestConstants.MappingProfilesCategory)]
public class OtisMappingProfileTests : BaseTestService
{
    /// <summary>
    /// Mock<IMapper> field.
    /// </summary>
    private IMapper _mapper;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper = SetupIMapper();
    }

    /// <summary>
    /// OtisMapperConfiguration is valid
    /// </summary>
    [Test]
    public void OtisMapperConfiguration_IsValid()
    {
        Assert.That(OtisMapperConfiguration, Is.Not.Null);
        OtisMapperConfiguration.AssertConfigurationIsValid();
    }

    /// <summary>
    /// Map ElevatorModel to ElevatorDataRow success
    /// </summary>
    [Test]
    public void Map_ElevatorModel_To_ElevatorDataRow_Success()
    {
        var elevatorModel = new ElevatorModel(_mapper)
        {
            Id            = 1,
            Description   = "Elevator A",
            CurrentFloor  = 8,
            CurrentLoad   = 7
        };

        var elevatorDataRow = _mapper.Map<ElevatorDataRow>(elevatorModel);

        Assert.That(elevatorDataRow.Id, Is.EqualTo(elevatorModel.Id));
        Assert.That(elevatorDataRow.Name, Is.EqualTo(elevatorModel.Description));
        Assert.That(elevatorDataRow.CurrentFloor, Is.EqualTo(elevatorModel.CurrentFloor));
        Assert.That(elevatorDataRow.NextFloor, Is.EqualTo(elevatorModel.NextFloor ?? 0));
        Assert.That(elevatorDataRow.CurrentLoad, Is.EqualTo(elevatorModel.CurrentLoad));
        Assert.That(elevatorDataRow.Capacity, Is.EqualTo(elevatorModel.Capacity));
        Assert.That(elevatorDataRow.Status, Is.EqualTo(elevatorModel.CurrentStatus));
    }

    /// <summary>
    /// Map ElevatorModel to ElevatorDataRow success
    /// </summary>
    [Test]
    public void Map_ElevatorRequest_To_ElevatorAcceptedRequest_Success()
    {
        var userInputRequest = new UserInputRequest()
        {
            OriginFloorInput = "1",
            DestinationFloorInput = "2",
            CapacityInput = "10"
        };
        var elevatorRequest = new ElevatorRequest(userInputRequest);

        var elevatorAcceptedRequest = _mapper.Map<ElevatorAcceptedRequest>(elevatorRequest);

        Assert.That(elevatorAcceptedRequest.Id, Is.EqualTo(elevatorRequest.Id));
        Assert.That(elevatorAcceptedRequest.OriginFloor, Is.EqualTo(elevatorRequest.OriginFloor));
        Assert.That(elevatorAcceptedRequest.DestinationFloor, Is.EqualTo(elevatorRequest.DestinationFloor));
        Assert.That(elevatorAcceptedRequest.NumberOfPeople, Is.EqualTo(elevatorRequest.NumberOfPeople));
        Assert.That(elevatorAcceptedRequest.ElevatorName, Is.EqualTo(string.Empty));
        Assert.That(elevatorAcceptedRequest.RequestDirection, Is.EqualTo(elevatorRequest.Direction));
        Assert.That(elevatorAcceptedRequest.OriginFloorServiced, Is.False);
        Assert.That(elevatorAcceptedRequest.DestinationFloorServiced, Is.False);
    }
}
