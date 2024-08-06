using AutoMapper;
using Moq;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Services;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Interface.Services;

/// <summary>
/// Class TerminalUiServiceTests extends the <see cref="InterfaceTests" /> class.
/// </summary>
public class TerminalUiServiceTests : InterfaceTests
{
    /// <summary>
    /// Mock<IMapper> field.
    /// </summary>
    Mock<IMapper> _mapper;
    /// <summary>
    /// Mock<OtisConfigurationService> field.
    /// </summary>
    Mock<OtisConfigurationService> _mockConfigurationService;
    /// <summary>
    /// Mock<ElevatorControllerService> field.
    /// </summary>
    Mock<ElevatorControllerService> _mockElevatorControllerService;
    /// <summary>
    /// TerminalUiService field.
    /// </summary>
    TerminalUiService _terminalUiService;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper                        = new Mock<IMapper>();
        _mockConfigurationService      = new Mock<OtisConfigurationService>();
        _mockElevatorControllerService = new Mock<ElevatorControllerService>(
            _mockConfigurationService.Object, _mapper.Object);
        _terminalUiService             = new TerminalUiService(_mockElevatorControllerService.Object);
    }

    /// <summary>
    /// Thest that InitialiseColorSchemes assigns property values.
    /// </summary>
    [Test]
    public void InitialiseColorSchemes_HasValues()
    {
        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("InitialiseColorSchemes");
        Assert.That(methodInfo, Is.Not.Null);
        
        methodInfo.Invoke(_terminalUiService, null);

        TestNonPublicPropertyValueNotNull("_globalColorScheme");
        TestNonPublicPropertyValueNotNull("_idleColorScheme");
    }

    /// <summary>
    /// Method to test nonpublic property value not null.
    /// </summary>
    /// <param name="propertyName"></param>
    private void TestNonPublicPropertyValueNotNull(string propertyName)
    {
        var property = GetNonPublicInstanceProperty<TerminalUiService>(propertyName);
        Assert.That(property, Is.Not.Null);

        var value = property.GetValue(_terminalUiService);
        Assert.That(value, Is.Not.Null);
    }
}
