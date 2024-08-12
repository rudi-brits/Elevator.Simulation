using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Constants;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Services;
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
    /// Mock<TerminalUiService> field
    /// </summary>
    private Mock<TerminalUiService> _mockTerminalUiService;
    /// <summary>
    /// SetupServiceCollectionMessageFunctionName
    /// </summary>
    private const string SetupServiceCollectionMessageFunction 
        = "SetupServiceCollectionMessage";
    /// <summary>
    /// LoadAppConfigurationMessageFunction
    /// </summary>
    private const string LoadAppConfigurationMessageFunction 
        = "LoadAppConfigurationMessage";
    /// <summary>
    /// LoadElevatorControllerConfigurationMessageFunction
    /// </summary>
    private const string LoadElevatorControllerConfigurationMessageFunction 
        = "LoadElevatorControllerConfigurationMessage";
    /// <summary>
    /// LoadAppConfigurationFunction
    /// </summary>
    private const string LoadAppConfigurationFunction
        = "LoadAppConfiguration";
    /// <summary>
    /// LoadElevatorControllerConfigurationFunction
    /// </summary>
    private const string LoadElevatorControllerConfigurationFunction
        = "LoadElevatorControllerConfiguration";
    /// <summary>
    /// SetupServiceCollectionFunction
    /// </summary>
    private const string SetupServiceCollectionFunction
        = "SetupServiceCollection";
    /// <summary>
    /// InitialiseTerminalUiFunction
    /// </summary>
    private const string InitialiseTerminalUiFunction
        = "InitialiseTerminalUi";

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper = SetupIMapper();
       
        _mockOtisConfigurationService  = new Mock<OtisConfigurationService>();
        _mockElevatorControllerService = new Mock<ElevatorControllerService>(
            _mockOtisConfigurationService.Object, _mapper);

        var mockTerminalGuiApplication = BuildMockTerminalGuiApplication();

        _mockTerminalUiService         = new Mock<TerminalUiService>(
            mockTerminalGuiApplication.Object,
            _mockElevatorControllerService.Object);
    }

    /// <summary>
    /// SetUp before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _otisSimulationServiceMock = new OtisSimulationServiceMock();
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
        _otisSimulationServiceMock.CallBaseSetupServiceCollection = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(SetupServiceCollectionFunction);
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        var message = GetMessage(SetupServiceCollectionMessageFunction, !throwException);

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.True);
        if (!throwException)
            Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue, Is.EqualTo(message));
        else
            Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue?.Contains(message), Is.True);
    }

    /// <summary>
    /// RunSimulation success
    /// </summary>
    [Test]
    public void RunSimulation_Success()
    {
        _otisSimulationServiceMock.CallBaseRunSimulation = true;
        _otisSimulationServiceMock.RunSimulation();

        Assert.That(_otisSimulationServiceMock.CalledSetupServiceCollection, Is.True);
        Assert.That(_otisSimulationServiceMock.CalledLoadAppConfiguration, Is.True);
        Assert.That(_otisSimulationServiceMock.CalledLoadElevatorControllerConfiguration, Is.True);
        Assert.That(_otisSimulationServiceMock.CalledInitialiseTerminalUi, Is.True);

        Assert.That(_otisSimulationServiceMock.CalledDisplayUserInputMessage, Is.True);
        Assert.That(
            _otisSimulationServiceMock.DisplayUserInputMessageValue, Is.EqualTo(OtisSimConstants.PressAnyKeyToContinueMessage));
    }

    /// <summary>
    /// RunSimulation fail result
    /// </summary>
    /// <param name="failSetupServiceCollection"></param>
    /// <param name="failLoadAppConfiguration"></param>
    [Test]
    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    public void RunSimulation_Fail(bool failSetupServiceCollection,
        bool failLoadAppConfiguration,
        bool failLoadElevatorControllerConfiguration)
    {
        _otisSimulationServiceMock.ThrowSetupServiceCollectionException = failSetupServiceCollection;
        _otisSimulationServiceMock.ThrowLoadAppConfigurationException = failLoadAppConfiguration;
        _otisSimulationServiceMock.ThrowLoadElevatorControllerConfiguration = failLoadElevatorControllerConfiguration;

        var errorMessage = failSetupServiceCollection
            ? nameof(_otisSimulationServiceMock.ThrowSetupServiceCollectionException)
            : failLoadAppConfiguration
                ? nameof(_otisSimulationServiceMock.ThrowLoadAppConfigurationException)
                : nameof(_otisSimulationServiceMock.ThrowLoadElevatorControllerConfiguration);

        _otisSimulationServiceMock.CallBaseRunSimulation = true;
        _otisSimulationServiceMock.RunSimulation();

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.True);
        Assert.That(
            _otisSimulationServiceMock.WriteLineToConsoleValue?.Contains(OtisSimConstants.ApplicationStartupFailedMessage), Is.True);
        Assert.That(
            _otisSimulationServiceMock.WriteLineToConsoleValue?.Contains(errorMessage), Is.True);
        Assert.That(_otisSimulationServiceMock.CalledDisplayUserInputMessage, Is.True);
        Assert.That(
            _otisSimulationServiceMock.DisplayUserInputMessageValue, Is.EqualTo(OtisSimConstants.PressAnyKeyToExitMessage));
    }

    /// <summary>
    /// LoadAppConfiguration success
    /// </summary>
    [Test]
    public void LoadAppConfiguration_Success()
    {
        _mockOtisConfigurationService.Setup(service => service.LoadConfiguration());

        SetupServiceProvider();

        _otisSimulationServiceMock.CallBaseLoadAppConfiguration = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(LoadAppConfigurationFunction);
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        _mockOtisConfigurationService.Verify(service => service.LoadConfiguration(), Times.AtLeastOnce);

        var message = GetMessage(LoadAppConfigurationMessageFunction, true);

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

        _otisSimulationServiceMock.CallBaseLoadAppConfiguration = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(LoadAppConfigurationFunction);
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(_otisSimulationServiceMock, null));

        var message = GetMessage(LoadAppConfigurationMessageFunction, false);

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

        _otisSimulationServiceMock.CallBaseLoadElevatorControllerConfiguration = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(LoadElevatorControllerConfigurationFunction);
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        _mockElevatorControllerService.Verify(service => service.LoadConfiguration(), Times.AtLeastOnce);

        var message = GetMessage(LoadElevatorControllerConfigurationMessageFunction, true);

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

        _otisSimulationServiceMock.CallBaseLoadElevatorControllerConfiguration = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(LoadElevatorControllerConfigurationFunction);
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(_otisSimulationServiceMock, null));

        var message = GetMessage(LoadElevatorControllerConfigurationMessageFunction, false);

        Assert.That(exception.InnerException, Is.Not.Null);
        Assert.That(exception.InnerException.Message.Contains(message), Is.True);
    }

    /// <summary>
    /// InitialiseTerminalUi success
    /// </summary>
    [Test]
    public void InitialiseTerminalUi_Success()
    {
        _mockTerminalUiService.Setup(service => service.InitialiseUi());

        SetupServiceProvider();

        _otisSimulationServiceMock.CallBaseInitialiseTerminalUi = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(InitialiseTerminalUiFunction);
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        _mockTerminalUiService.Verify(service => service.InitialiseUi(), Times.AtLeastOnce);

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.False);
        Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue, Is.EqualTo(null));
    }

    /// <summary>
    /// InitialiseTerminalUi exception
    /// </summary>
    [Test]
    public void InitialiseTerminalUi_Exception()
    {
        _mockTerminalUiService.Setup(service => service.InitialiseUi())
            .Throws(new InvalidOperationException());

        SetupServiceProvider();

        _otisSimulationServiceMock.CallBaseInitialiseTerminalUi = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>(InitialiseTerminalUiFunction);
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        Assert.That(_otisSimulationServiceMock.CalledDisplayUserInputMessage, Is.True);
        Assert.That(
            _otisSimulationServiceMock.DisplayUserInputMessageValue?.Contains(OtisSimConstants.ErrorInitialiseTerminalUi), Is.True);
    }

    /// <summary>
    /// LoadingMessages expected values
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="expectedMessage"></param>
    /// <param name="isSuccess"></param>
    [TestCase(SetupServiceCollectionMessageFunction, OtisSimConstants.SetupServiceCollectionResultMessage, true)]
    [TestCase(SetupServiceCollectionMessageFunction, OtisSimConstants.SetupServiceCollectionResultMessage, false)]
    [TestCase(LoadAppConfigurationMessageFunction, OtisSimConstants.LoadAppConfigurationResultMessage, true)]
    [TestCase(LoadAppConfigurationMessageFunction, OtisSimConstants.LoadAppConfigurationResultMessage, false)]
    [TestCase(LoadElevatorControllerConfigurationMessageFunction, OtisSimConstants.LoadElevatorConfigurationResultMessage, true)]
    [TestCase(LoadElevatorControllerConfigurationMessageFunction, OtisSimConstants.LoadElevatorConfigurationResultMessage, false)]
    [Test]
    public void LoadingMessages_ExpectedValues(string methodName, string expectedMessage, bool isSuccess)
    {
        var prefix  = isSuccess ? GetSuccessPrefixValue()!.ToString() : "";
        var message = GetMessage(methodName, isSuccess);

        Assert.That(message, Is.Not.Null.Or.Empty);
        Assert.That(message.Contains(expectedMessage), Is.True);
        if (isSuccess)
            Assert.That(message.Contains(prefix!), Is.True);
    }

    /// <summary>
    /// GetPrefix with expected values
    /// </summary>
    /// <param name="isSuccess"></param>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void GetPrefix_ExpectedValues(bool isSuccess)
    {
        var successPrefix = GetSuccessPrefixValue();

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("GetPrefix");
        var prefix = methodInfo.Invoke(_otisSimulationServiceMock, new object[] { isSuccess });

        Assert.That(prefix, Is.Not.Null.Or.Empty);

        if (isSuccess)
            Assert.That(prefix, Is.EqualTo(successPrefix));
        else
            Assert.That(prefix, Is.EqualTo(string.Empty));
    }

    /// <summary>
    /// WriteLineToConsole with expected values
    /// </summary>
    [Test]
    public void WriteLineToConsole_ExpectedValues()
    {
        var message = nameof(WriteLineToConsole_ExpectedValues);

        var methodInfo     = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("WriteLineToConsole");
        methodInfo.Invoke(_otisSimulationServiceMock, new object[] { message });

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.True);
        Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue, Is.EqualTo(message));
    }

    /// <summary>
    /// DisplayUserInputMessage with expected values
    /// </summary>
    [Test]
    public void DisplayUserInputMessage_ExpectedValues()
    {
        var message = nameof(DisplayUserInputMessage_ExpectedValues);

        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("DisplayUserInputMessage");
        methodInfo.Invoke(_otisSimulationServiceMock, new object[] { message });

        Assert.That(_otisSimulationServiceMock.CalledDisplayUserInputMessage, Is.True);
        Assert.That(_otisSimulationServiceMock.DisplayUserInputMessageValue, Is.EqualTo(message));
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
        serviceCollection.AddSingleton(_mockTerminalUiService.Object);

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