using AutoMapper;
using Moq;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Interfaces;
using Otis.Sim.Interface.Services;
using Otis.Sim.Unit.Tests.Otis.Sim.Interface.MockClasses;
using Otis.Sim.Utilities.Constants;
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
    private IMapper _mapper;
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
        _mapper = SetupIMapper();

        _mockConfigurationService = new Mock<OtisConfigurationService>();
        _mockElevatorControllerService = new Mock<ElevatorControllerService>(
            _mockConfigurationService.Object, _mapper);

        _mockTerminalGuiApplication = new Mock<ISimTerminalGuiApplication>();
        _mockTerminalGuiApplication.Setup(x => x.Init()).Verifiable();
        _mockTerminalGuiApplication.Setup(x => x.Top).Returns(new Toplevel());
        _mockTerminalGuiApplication.Setup(x => x.Run()).Verifiable();
        _mockTerminalGuiApplication.Setup(x => x.Invoke(It.IsAny<Action>())).Callback<Action>(a => a.Invoke());
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
        var elevatorControllerServiceField = 
            GetNonPublicInstanceFieldNotNull<TerminalUiService>("_elevatorControllerService");

        var elevatorControllerServiceValue = 
            elevatorControllerServiceField.GetValue(_mockTerminalUiService);

        Assert.That(elevatorControllerServiceValue, Is.Not.Null);
        Assert.That(elevatorControllerServiceValue, Is.EqualTo(_mockElevatorControllerService.Object));

        // _terminalGuiApplication
        var terminalGuiApplicationField 
            = GetNonPublicInstanceFieldNotNull<TerminalUiService>("_terminalGuiApplication");
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
        var originFloorInput      = GetNonPublicInstancePropertyNotNull<TerminalUiService>("_originFloorInput");
        var destinationFloorInput = GetNonPublicInstancePropertyNotNull<TerminalUiService>("_destinationFloorInput");
        var capacityInput         = GetNonPublicInstancePropertyNotNull<TerminalUiService>("_capacityInput");

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
    /// Test the CreateElevatorTable function.
    /// </summary>
    [Test]
    public void CreateElevatorTable_Success()
    {
        SetupTableColumns();
    }

    /// <summary>
    /// Test the UpdateDataTable function.
    /// </summary>
    [Test]
    public void UpdateDataTable_Success()
    {
        var elevatorsTableView = SetupTableRows();

        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("UpdateDataTable");
        Assert.That(methodInfo, Is.Not.Null);

        _mockTerminalUiService.CallBaseElevatorRows = false;
        _mockTerminalUiService.CallBaseUpdateDataTable = true;
        methodInfo.Invoke(_mockTerminalUiService, null);

        var tableRowCount = GetElevatorsTableViewRowCount(elevatorsTableView);

        Assert.That(tableRowCount, Is.EqualTo(0));
    }

    /// <summary>
    /// Test the AddElevatorRows function.
    /// </summary>
    [Test]
    public void AddElevatorRows_Success()
    {
        SetupTableRows();
    }

    /// <summary>
    /// UpdateRequestStatus with message text.
    /// </summary>
    /// <param name="numberOfMessages"></param>
    /// <param name="statusViewIsNotNull"></param>
    [Test]
    [TestCase(1, true)]
    [TestCase(15, true)]
    [TestCase(1, false)]
    [TestCase(15, false)]
    public void UpdateRequestStatus_Success(int numberOfMessages, bool statusViewIsNotNull)
    {
        var requestStatusView = GetNonPublicInstancePropertyNotNull<TerminalUiService>("_requestStatusView");

        if (statusViewIsNotNull)
            requestStatusView!.SetValue(_mockTerminalUiService, new TextView());

        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("UpdateRequestStatus");
        Assert.That(methodInfo, Is.Not.Null);

        var expectedText = string.Empty;
        Assert.DoesNotThrow(() =>
        {
            for (var i = 0; i < numberOfMessages; i++)
            {
                var message = $"Message {i + 1}";
                methodInfo.Invoke(_mockTerminalUiService, new object[] { message });
                expectedText = $"{message}\r{UtilityConstants.NewLineCharacter}{expectedText}";
            }
        });

        if (!statusViewIsNotNull)
            return;

        var textValue = ((requestStatusView?.GetValue(_mockTerminalUiService)) as TextView)?.Text;
        Assert.That(textValue, Is.Not.Null);
        Assert.That(textValue.ToString(), Is.EqualTo(expectedText));
    }

    /// <summary>
    /// Ensure that the thread is started.
    /// </summary>
    [Test]
    public void InitialiseTableDataRefresh_StartThread()
    {
        SetupInitialiseTableDataRefresh();

        var threadValue = GetRefreshDataThreadValue();
        Assert.That(threadValue.IsAlive, Is.True);
    }

    /// <summary>
    /// Ensure that CalledUpdateDataTable was called.
    /// </summary>
    [Test]
    public void InitialiseTableDataRefresh_UpdateDataTable_WasInvoked()
    {
        SetupInitialiseTableDataRefresh();

        Thread.Sleep(1200);
        Assert.That(_mockTerminalUiService.CalledUpdateDataTable, Is.True);
    }

    /// <summary>
    /// Ensure that the thread is cancelled.
    /// </summary>
    [Test]
    public void InitialiseTableDataRefresh_CancelThread()
    {
        SetupInitialiseTableDataRefresh();

        var cancellationTokenSource = GetNonPublicInstanceFieldNotNull<TerminalUiService>("_cancellationTokenSource");
        (cancellationTokenSource.GetValue(_mockTerminalUiService) as CancellationTokenSource)?.Cancel();

        var threadValue = GetRefreshDataThreadValue();
        threadValue.Join();

        Assert.That(threadValue.IsAlive, Is.False);
    }

    /// <summary>
    /// Setup InitialiseTableDataRefresh
    /// </summary>
    private void SetupInitialiseTableDataRefresh()
    {
        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("InitialiseTableDataRefresh");
        Assert.That(methodInfo, Is.Not.Null);

        _mockTerminalUiService.CallBaseUpdateDataTable = true;
        _mockTerminalUiService.CallBaseInitialiseTableDataRefresh = true;

        methodInfo.Invoke(_mockTerminalUiService, null);
    }

    private Thread GetRefreshDataThreadValue()
    {
        var refreshDataThreadField = GetNonPublicInstanceFieldNotNull<TerminalUiService>("_refreshDataThread");

        var threadValue = (refreshDataThreadField?.GetValue(_mockTerminalUiService)) as Thread;
        Assert.That(threadValue, Is.Not.Null);

        return threadValue;
    }

    /// <summary>
    /// Method to test nonpublic property value not null.
    /// </summary>
    /// <param name="propertyName"></param>
    private void TestNonPublicPropertyValueNotNull(string propertyName)
    {
        var property = GetNonPublicInstancePropertyNotNull<TerminalUiService>(propertyName);

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

    /// <summary>
    /// Setup the elevatorsTableView,
    /// </summary>
    /// <returns></returns>
    private PropertyInfo? SetupElevatorsTableView()
    {
        var elevatorsTableView = GetNonPublicInstancePropertyNotNull<TerminalUiService>("_elevatorsTableView");
        elevatorsTableView.SetValue(_mockTerminalUiService, new TableView());
        return elevatorsTableView;
    }

    /// <summary>
    /// Setup the elevatorsTableView columns.
    /// </summary>
    private PropertyInfo? SetupTableColumns()
    {
        var elevatorsTableView = SetupElevatorsTableView();
        Assert.That(elevatorsTableView, Is.Not.Null);

        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("CreateElevatorTable");
        Assert.That(methodInfo, Is.Not.Null);

        _mockTerminalUiService.CallBaseCreateElevatorTable = true;
        methodInfo.Invoke(_mockTerminalUiService, null);

        var tableColumns = (elevatorsTableView?.GetValue(_mockTerminalUiService) as TableView)?.Table?.Columns;
        Assert.That(tableColumns, Is.Not.Null);
        Assert.That(tableColumns?.Count, Is.EqualTo(_mockElevatorControllerService.Object.ElevatorTableHeaders.Count));

        return elevatorsTableView;
    }

    /// <summary>
    /// Setup the elevatorsTableView rows.
    /// </summary>
    private PropertyInfo? SetupTableRows()
    {
        var elevatorsTableView = SetupTableColumns();

        var elevatorsField = GetNonPublicInstanceFieldNotNull<ElevatorControllerService>("_elevators");
        var elevators = new List<ElevatorModel>()
        {
            new ElevatorModel(_mapper),
            new ElevatorModel(_mapper),
            new ElevatorModel(_mapper)
        };

        elevatorsField?.SetValue(_mockElevatorControllerService.Object, elevators);

        var methodInfo = GetNonPublicInstanceMethod<TerminalUiService>("AddElevatorRows");
        Assert.That(methodInfo, Is.Not.Null);

        _mockTerminalUiService.CallBaseElevatorRows = true;
        methodInfo.Invoke(_mockTerminalUiService, null);

        var tableRowCount = GetElevatorsTableViewRowCount(elevatorsTableView);
        var dataRowsCount = _mockElevatorControllerService.Object.ElevatorDataRows.Count;

        Assert.That(tableRowCount, Is.EqualTo(dataRowsCount));

        return elevatorsTableView;
    }

    private int GetElevatorsTableViewRowCount(PropertyInfo? elevatorsTableView)
        => (elevatorsTableView?.GetValue(_mockTerminalUiService) as TableView)?.Table?.Rows?.Count ?? 0;
}
