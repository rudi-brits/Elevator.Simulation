using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Unit.Tests.Constants;
using Otis.Sim.Unit.Tests.Otis.Sim.MappingProfiles.MockClasses;
using Otis.Sim.Unit.Tests.TestBase.Services;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Services;

/// <summary>
/// Class OtisMappingProfileTests extends the <see cref="BaseTestService" /> class.
/// </summary>
/// <category><see cref="TestConstants.SimulationServiceCategory" /></category>
public class OtisSimulationServiceTests : BaseTestService
{
    /// <summary>
    /// Mock<IMapper> field.
    /// </summary>
    private IMapper _mapper;
    /// <summary>
    /// OtisSimulationServiceMock field
    /// </summary>
    private OtisSimulationServiceMock _otisSimulationServiceMock;
    /// <summary>
    /// Mock<OtisConfigurationService> field
    /// </summary>
    private Mock<OtisConfigurationService> _mockOtisConfigurationService;
    /// <summary>
    /// Mock<ElevatorControllerService> field
    /// </summary>
    private Mock<ElevatorControllerService> _mockElevatorControllerService;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper = SetupIMapper();
        _otisSimulationServiceMock    = new OtisSimulationServiceMock();
        _mockOtisConfigurationService = new Mock<OtisConfigurationService>();
        _mockElevatorControllerService = new Mock<ElevatorControllerService>(
            _mockOtisConfigurationService.Object, _mapper);
    }

    /// <summary>
    /// Test constructor
    /// </summary>
    [Test]
    public void Constructor_WithExpected_InitialisationsAndDefaults()
    {
        var serviceProviderField = GetServiceProviderField();
        ValidateFieldValue(serviceProviderField, _otisSimulationServiceMock, false);

        var successPrefixValue = GetSuccessPrefixValue();
        Assert.That(successPrefixValue, Is.Not.Null.Or.Empty);
    }

    /// <summary>
    /// SetupServiceCollection expected result
    /// </summary>
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void SetupServiceCollection_ExpectedResult(bool throwException)
    {
        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("SetupServiceCollection");
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        var message = GetMessage("SetupServiceCollectionMessage", !throwException);

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.True);
        if (!throwException)
            Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue, Is.EqualTo(message));
        else
            Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue?.Contains(message), Is.True);
    }

    /// <summary>
    /// LoadAppConfiguration success
    /// </summary>
    [Test]
    public void LoadAppConfiguration_Success()
    {
        _mockOtisConfigurationService.Setup(service => service.LoadConfiguration());

        SetupServiceProvider();

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("LoadAppConfiguration");
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        _mockOtisConfigurationService.Verify(service => service.LoadConfiguration(), Times.AtLeastOnce);

        var message = GetMessage("LoadAppConfigurationMessage", true);

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.True);
        Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue?.Contains(message), Is.True);
    }

    /// <summary>
    /// LoadAppConfiguration exception
    /// </summary>
    [Test]
    public void LoadAppConfiguration_Exception()
    {
        _mockOtisConfigurationService.Setup(service => service.LoadConfiguration())
            .Throws(new InvalidOperationException());

        SetupServiceProvider();

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("LoadAppConfiguration");
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(_otisSimulationServiceMock, null));

        var message = GetMessage("LoadAppConfigurationMessage", false);

        Assert.That(exception.InnerException, Is.Not.Null);
        Assert.That(exception.InnerException.Message.Contains(message), Is.True);
    }

    /// <summary>
    /// LoadElevatorControllerConfiguration success
    /// </summary>
    [Test]
    public void LoadElevatorControllerConfiguration_Success()
    {
        _mockElevatorControllerService.Setup(service => service.LoadConfiguration());

        SetupServiceProvider();

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("LoadElevatorControllerConfiguration");
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        _mockElevatorControllerService.Verify(service => service.LoadConfiguration(), Times.AtLeastOnce);

        var message = GetMessage("LoadElevatorControllerConfigurationMessage", true);

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.True);
        Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue?.Contains(message), Is.True);
    }

    /// <summary>
    /// LoadElevatorControllerConfiguration exception
    /// </summary>
    [Test]
    public void LoadElevatorControllerConfiguration_Exception()
    {
        _mockElevatorControllerService.Setup(service => service.LoadConfiguration())
            .Throws(new InvalidOperationException());

        SetupServiceProvider();

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("LoadElevatorControllerConfiguration");
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(_otisSimulationServiceMock, null));

        var message = GetMessage("LoadElevatorControllerConfigurationMessage", false);

        Assert.That(exception.InnerException, Is.Not.Null);
        Assert.That(exception.InnerException.Message.Contains(message), Is.True);
    }

    /// <summary>
    /// GetMessage
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="isSuccess"></param>
    /// <returns></returns>
    private string GetMessage(string methodName, bool isSuccess = true)
    {
        var messageMethodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(methodName);
        var message = messageMethodInfo.Invoke(_otisSimulationServiceMock, new object[] { isSuccess }) as string ?? "";
        return message;
    }

    /// <summary>
    /// SetupServiceProvider
    /// </summary>
    private void SetupServiceProvider()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(_mockOtisConfigurationService.Object);
        serviceCollection.AddSingleton(_mockElevatorControllerService.Object);

        var serviceProviderField = GetServiceProviderField();
        serviceProviderField.SetValue(_otisSimulationServiceMock, serviceCollection.BuildServiceProvider());

    }

    /// <summary>
    /// GetSuccessPrefixValue
    /// </summary>
    /// <returns></returns>
    private object? GetSuccessPrefixValue()
    {
        var successPrefixField 
            = GetNonPublicInstanceFieldNotNull<OtisSimulationServiceMock>("SuccessPrefix");
        return ValidateFieldValue(successPrefixField, _otisSimulationServiceMock);
    }

    /// <summary>
    /// GetServiceProviderField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetServiceProviderField()
        => GetNonPublicInstanceFieldNotNull<OtisSimulationServiceMock>("_serviceProvider");

    /// <summary>
    /// GetServiceProviderValue
    /// </summary>
    /// <returns></returns>
    private object? GetServiceProviderValue()
    {
        var serviceProviderField = GetServiceProviderField();
        return ValidateFieldValue(serviceProviderField, _otisSimulationServiceMock);
    }
}