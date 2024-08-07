using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Interfaces;
using Otis.Sim.Interface.Services;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Interface.MockClasses;

/// <summary>
/// The TerminalUiServiceMock class extends the <see cref="TerminalUiService" /> class.
/// </summary>
public class TerminalUiServiceMock : TerminalUiService
{
    /// <summary>
    /// CalledSetOriginFloorInputFocus field.
    /// </summary>
    public bool CalledSetOriginFloorInputFocus = false;
    /// <summary>
    /// CalledProcessRequest field.
    /// </summary>
    public bool CalledProcessRequest = false;
    /// <summary>
    /// CalledShowSuccessMessageBox field.
    /// </summary>
    public bool CalledShowSuccessMessageBox = false;
    /// <summary>
    /// CalledShowErrorMessageBox field.
    /// </summary>
    public bool CalledShowErrorMessageBox = false;
    /// <summary>
    /// CalledCreateElevatorTable field.
    /// </summary>
    public bool CalledCreateElevatorTable = false;
    /// <summary>
    /// CalledInitialiseTableDataRefresh field.
    /// </summary>
    public bool CalledInitialiseTableDataRefresh = false;

    /// <summary>
    /// CallBaseProcessRequest field.
    /// </summary>
    public bool CallBaseProcessRequest = false;

    /// <summary>
    /// TerminalUiServiceMock constructor
    /// </summary>
    /// <param name="terminalGuiApplication"></param>
    /// <param name="elevatorControllerService"></param>
    public TerminalUiServiceMock(ISimTerminalGuiApplication terminalGuiApplication,
        ElevatorControllerService elevatorControllerService)
            : base(terminalGuiApplication, elevatorControllerService)
    {
    }

    /// <summary>
    /// SetOriginFloorInputFocus function.
    /// </summary>
    protected override void SetOriginFloorInputFocus()
    {
        CalledSetOriginFloorInputFocus = true;
    }

    /// <summary>
    /// ProcessRequest function.
    /// </summary>
    protected override void ProcessRequest()
    {
        CalledProcessRequest = true;
        if (CallBaseProcessRequest)
            base.ProcessRequest();
    }

    /// <summary>
    /// ShowSuccessMessageBox function.
    /// </summary>
    /// <param name="message"></param>
    protected override void ShowSuccessMessageBox(string message)
    {
        CalledShowSuccessMessageBox = true;
    }

    /// <summary>
    /// ShowErrorMessageBox function.
    /// </summary>
    /// <param name="message"></param>
    protected override void ShowErrorMessageBox(string message)
    {
        CalledShowErrorMessageBox = true;
    }

    /// <summary>
    /// CreateElevatorTable function.
    /// </summary>
    protected override void CreateElevatorTable()
    {
        CalledCreateElevatorTable = true;
    }

    /// <summary>
    /// InitialiseTableDataRefresh function.
    /// </summary>
    protected override void InitialiseTableDataRefresh()
    {
        CalledInitialiseTableDataRefresh = true;
    }
}
