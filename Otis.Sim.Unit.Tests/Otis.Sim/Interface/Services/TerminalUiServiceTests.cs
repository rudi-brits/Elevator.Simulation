using AutoMapper;
using Moq;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Interfaces;
using Otis.Sim.Interface.Services;
using Otis.Sim.Unit.Tests.Otis.Sim.Interface.MockClasses;
using System.Reflection;
using Terminal.Gui;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Interface.Services;

/// <summary>
/// Class TerminalUiServiceTests extends the <see cref="InterfaceTests" /> class.
/// </summary>
public class TerminalUiServiceTests : InterfaceTests
{
    /// <summary>
    /// Mock<IMapper> field.
    /// </summary>
    private Mock<IMapper> _mapper;
    /// <summary>
    /// Mock<OtisConfigurationService> field.
    /// </summary>
    private Mock<OtisConfigurationService> _mockConfigurationService;
    /// <summary>
    /// Mock<ISimTerminalGuiApplication>
    /// </summary>
    private Mock<ISimTerminalGuiApplication> _mockTerminalGuiApplication;
    /// <summary>
    /// Mock<ElevatorControllerService> field.
    /// </summary>
    private Mock<ElevatorControllerService> _mockElevatorControllerService;
    /// <summary>
    /// TerminalUiServiceMock to test private methods.
    /// </summary>
    private TerminalUiServiceMock _mockTerminalUiService;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper = new Mock<IMapper>();

        _mockConfigurationService = new Mock<OtisConfigurationService>();
        _mockElevatorControllerService = new Mock<ElevatorControllerService>(
            _mockConfigurationService.Object, _mapper.Object);

        _mockTerminalGuiApplication = new Mock<ISimTerminalGuiApplication>();
        _mockTerminalGuiApplication.Setup(x => x.Init()).Verifiable();
        _mockTerminalGuiApplication.Setup(x => x.Top).Returns(new Toplevel());
        _mockTerminalGuiApplication.Setup(x => x.Run()).Verifiable();
    }

    /// <summary>
    /// SetUp before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockTerminalUiService = new TerminalUiServiceMock(_mockTerminalGuiApplication.Object,
            _mockElevatorControllerService.Object);
    }

    /// <summary>
    /// Test constructor assignments.
    /// </summary>
    [Test]
    public void Constructor_ObjectAssignments_Success()
    {
        // elevatorControllerService
        var elevatorControllerServiceField = GetNonPublicInstanceField<TerminalUiService>("_elevatorControllerService");
        Assert.That(elevatorControllerServiceField, Is.Not.Null);

        var elevatorControllerServiceValue = 
            elevatorControllerServiceField.GetValue(_mockTerminalUiService);

        Assert.That(elevatorControllerServiceValue, Is.Not.Null);
        Assert.That(elevatorControllerServiceValue, Is.EqualTo(_mockElevatorControllerService.Object));

        // _terminalGuiApplication
        var terminalGuiApplicationField = GetNonPublicInstanceField<TerminalUiService>("_terminalGuiApplication");
        Assert.That(terminalGuiApplicationField, Is.Not.Null);

        var terminalGuiApplicationValue =
            terminalGuiApplicationField.GetValue(_mockTerminalUiService);

        Assert.That(terminalGuiApplicationValue, Is.Not.Null);
        Assert.That(terminalGuiApplicationValue, Is.EqualTo(_mockTerminalGuiApplication.Object));

        // UpdateRequestStatus
        Assert.That(_mockElevatorControllerService?.Object?.UpdateRequestStatus, Is.Not.Null);
    }

    /// <summary>
    /// Test that the InitialiseUi method exists.
    /// </summary>
    [Test]
    public void InitialiseUiMethod_Exists()
    {
        var methodInfo = GetPublicInstanceMethod<TerminalUiService>("InitialiseUi");
        Assert.That(methodInfo, Is.Not.Null);
    }

    /// <summary>
    /// Test that InitialiseColorSchemes assigns property values.
    /// </summary>
    [Test]
    public void InitialiseColorSchemes_HasValues()
    {
        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("InitialiseColorSchemes");
        Assert.That(methodInfo, Is.Not.Null);

        methodInfo.Invoke(_mockTerminalUiService, null);

        TestNonPublicPropertyValueNotNull("_globalColorScheme");
        TestNonPublicPropertyValueNotNull("_idleColorScheme");
    }

    /// <summary>
    /// Test that the InitialiseApplication method exists.
    /// </summary>
    [Test]
    public void InitialiseApplicationMethod_AssignValues()
    {
        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("InitialiseApplication");
        Assert.That(methodInfo, Is.Not.Null);

        methodInfo.Invoke(_mockTerminalUiService, null);

        TestNonPublicPropertyValueNotNull("_requestStatusView");
        TestNonPublicPropertyValueNotNull("_elevatorsTableView");
        TestNonPublicPropertyValueNotNull("_originFloorInput");
        TestNonPublicPropertyValueNotNull("_destinationFloorInput");
        TestNonPublicPropertyValueNotNull("_capacityInput");

        Assert.That(_mockTerminalUiService.CalledSetOriginFloorInputFocus, Is.True);
        Assert.That(_mockTerminalUiService.CalledProcessRequest, Is.False);
        Assert.That(_mockTerminalUiService.CalledCreateElevatorTable, Is.True);
        Assert.That(_mockTerminalUiService.CalledInitialiseTableDataRefresh, Is.True);
    }

    /// <summary>
    /// Test ProcessRequest for success and fail.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ProcessRequest_ShowSuccessMessage(bool success)
    {
        var newTextField = new TextField("");
        var originFloorInput      = GetNonPublicInstanceProperty<TerminalUiService>("_originFloorInput");
        var destinationFloorInput = GetNonPublicInstanceProperty<TerminalUiService>("_destinationFloorInput");
        var capacityInput         = GetNonPublicInstanceProperty<TerminalUiService>("_capacityInput");

        originFloorInput?.SetValue(_mockTerminalUiService, newTextField);
        destinationFloorInput?.SetValue(_mockTerminalUiService, newTextField);
        capacityInput?.SetValue(_mockTerminalUiService, newTextField);

        var response = new ElevatorRequestResult { Success = success };
        _mockElevatorControllerService
            .Setup(x => x.RequestElevator(It.IsAny<UserInputRequest>()))
            .Returns(response);

        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("ProcessRequest");
        Assert.That(methodInfo, Is.Not.Null);

        _mockTerminalUiService.CallBaseProcessRequest = true;
        methodInfo.Invoke(_mockTerminalUiService, null);

        Assert.That(_mockTerminalUiService.CalledSetOriginFloorInputFocus, Is.True);

        if (success)
        {
            Assert.That(_mockTerminalUiService.CalledShowSuccessMessageBox, Is.True);

            Assert.That(GetTextFieldStringValue(originFloorInput), Is.EqualTo(string.Empty));
            Assert.That(GetTextFieldStringValue(destinationFloorInput), Is.EqualTo(string.Empty));
            Assert.That(GetTextFieldStringValue(capacityInput), Is.EqualTo(string.Empty));
        } 
        else
        {
            Assert.That(_mockTerminalUiService.CalledShowErrorMessageBox, Is.True);
        }
    }

    /// <summary>
    /// Method to test nonpublic property value not null.
    /// </summary>
    /// <param name="propertyName"></param>
    private void TestNonPublicPropertyValueNotNull(string propertyName)
    {
        var property = GetNonPublicInstanceProperty<TerminalUiService>(propertyName);
        Assert.That(property, Is.Not.Null);

        var value = property.GetValue(_mockTerminalUiService);
        Assert.That(value, Is.Not.Null);
    }

    /// <summary>
    /// Get the string value from PropertyInfo of a TextField object.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    private string? GetTextFieldStringValue(PropertyInfo? propertyInfo)
        => (propertyInfo?.GetValue(_mockTerminalUiService) as TextField)?.Text?.ToString();
}
