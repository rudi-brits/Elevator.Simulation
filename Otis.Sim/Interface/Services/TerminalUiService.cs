using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Interface.Interfaces;
using Otis.Sim.Utilities.Constants;
using System.Data;
using Terminal.Gui;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;
using UiConstants = Otis.Sim.Interface.Constants.TerminalUiConstants;

namespace Otis.Sim.Interface.Services;

/// <summary>
/// TerminalUiService class extends the <see cref="ConsoleFullScreenService" /> class.
/// </summary>
public class TerminalUiService : ConsoleFullScreenService
{
    /// <summary>
    /// _globalColorScheme
    /// </summary>
    private ColorScheme? _globalColorScheme { get; set; }
    /// <summary>
    /// _movingUpColorSchema
    /// </summary>
    private ColorScheme? _movingUpColorSchema { get; set; }
    /// <summary>
    /// _movingDownColorSchema
    /// </summary>
    private ColorScheme? _movingDownColorSchema { get; set; }
    /// <summary>
    /// _doorsOpenColorScheme
    /// </summary>
    private ColorScheme? _doorsOpenColorScheme { get; set; }
    /// <summary>
    /// _requestStatusView
    /// </summary>
    private TextView? _requestStatusView { get; set; }
    /// <summary>
    /// _elevatorsTableView
    /// </summary>
    private TableView? _elevatorsTableView { get; set; }
    /// <summary>
    /// _originFloorInput
    /// </summary>
    private TextField? _originFloorInput { get; set; }
    /// <summary>
    /// _destinationFloorInput
    /// </summary>
    private TextField? _destinationFloorInput { get; set; }
    /// <summary>
    /// _capacityInput
    /// </summary>
    private TextField? _capacityInput { get; set; }

    /// <summary>
    /// _refreshDataThread
    /// </summary>
    private Thread? _refreshDataThread;
    /// <summary>
    /// _cancellationTokenSource
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// _terminalGuiApplication
    /// </summary>
    private readonly ISimTerminalGuiApplication _terminalGuiApplication;
    /// <summary>
    /// _elevatorControllerService
    /// </summary>
    private readonly ElevatorControllerService _elevatorControllerService;

    /// <summary>
    /// TerminalUiService constructor
    /// </summary>
    /// <param name="terminalGuiApplication"></param>
    /// <param name="elevatorControllerService"></param>
    public TerminalUiService(ISimTerminalGuiApplication terminalGuiApplication,
        ElevatorControllerService elevatorControllerService)
    {
        _terminalGuiApplication    = terminalGuiApplication;
        _elevatorControllerService = elevatorControllerService;
        _elevatorControllerService.UpdateRequestStatus = UpdateRequestStatus;
    }

    /// <summary>
    /// InitialiseUi function
    /// </summary>
    public virtual void InitialiseUi()
    {
        InitialiseColorSchemes();
        InitialiseFullScreen();
        InitialiseApplication();
    }

    /// <summary>
    /// InitialiseColorSchemes function
    /// </summary>
    private void InitialiseColorSchemes()
    {
        _globalColorScheme = new ColorScheme()
        {
            Normal = UiConstants.GlobalColorAttribute
        };

        _movingUpColorSchema = new ColorScheme()
        {
            Normal    = UiConstants.MovingUpColorAttribute,
            Focus     = UiConstants.MovingUpColorAttribute,
            HotNormal = UiConstants.MovingUpColorAttribute,
            HotFocus  = UiConstants.MovingUpColorAttribute,
        };

        _movingDownColorSchema = new ColorScheme()
        {
            Normal    = UiConstants.MovingDownColorAttribute,
            Focus     = UiConstants.MovingDownColorAttribute,
            HotNormal = UiConstants.MovingDownColorAttribute,
            HotFocus  = UiConstants.MovingDownColorAttribute,
        };

        _doorsOpenColorScheme = new ColorScheme()
        {
            Normal    = UiConstants.DoorsOpenColorAttribute,
            Focus     = UiConstants.DoorsOpenColorAttribute,
            HotNormal = UiConstants.DoorsOpenColorAttribute,
            HotFocus  = UiConstants.DoorsOpenColorAttribute,
        };
    }

    /// <summary>
    /// InitialiseApplication function
    /// </summary>
    protected virtual void InitialiseApplication()
    {
        _terminalGuiApplication.Init();

        var window = new Window("Elevator Simulation")
        {
            X           = 0,
            Y           = 1,
            Width       = Dim.Fill(),
            Height      = Dim.Fill(),
            ColorScheme = _globalColorScheme
        };
        _terminalGuiApplication.Top.Add(window);

        var elevatorStatusFrameView = new FrameView("Elevator Status")
        {
            X           = 0,
            Y           = 1,
            Width       = Dim.Percent(50),
            Height      = Dim.Percent(70),
            CanFocus    = false,
            ColorScheme = _globalColorScheme
        };
        window.Add(elevatorStatusFrameView);

        var requestFrameView = new FrameView("Request")
        {
            X           = Pos.Percent(50),
            Y           = 1,
            Width       = Dim.Fill(),
            Height      = Dim.Percent(70),
            ColorScheme = _globalColorScheme
        };
        window.Add(requestFrameView);

        var requestStatusFrameView = new FrameView("Request status")
        {
            X           = 0,
            Y           = Pos.Percent(70) + 1,
            Width       = Dim.Fill(),
            Height      = Dim.Fill(),
            CanFocus    = false,
            ColorScheme = _globalColorScheme
        };
        window.Add(requestStatusFrameView);

        _requestStatusView = new TextView()
        {
            X        = 0,
            Y        = 1,
            Width    = Dim.Fill(),
            Height   = Dim.Fill(),
            ReadOnly = true,
            CanFocus = true,
            WordWrap = true,
        };
        requestStatusFrameView.Add(_requestStatusView);

        _elevatorsTableView = new TableView()
        {
            X             = 0,
            Y             = 1,
            Width         = Dim.Fill(),
            Height        = Dim.Fill(),
            CanFocus      = false,
            FullRowSelect = true,
            ColorScheme   = _globalColorScheme
        };
        elevatorStatusFrameView.Add(_elevatorsTableView);

        var originFloorLabel = new Label()
        {
            Y    = 1,
            Text = OtisSimConstants.OriginFloorName
        };

        _originFloorInput = new TextField("")
        {
            Y     = Pos.Top(originFloorLabel),
            X     = Pos.Right(originFloorLabel) + 1,
            Width = Dim.Fill()
        };

        var destinationFloorLabel = new Label()
        {
            Y    = Pos.Top(originFloorLabel) + 2,
            Text = OtisSimConstants.DestinationFloorName
        };

        _destinationFloorInput = new TextField("")
        {
            Y     = Pos.Top(destinationFloorLabel),
            X     = Pos.Right(destinationFloorLabel) + 1,
            Width = Dim.Fill()
        };

        var capacityLabel = new Label()
        {
            Y    = Pos.Top(destinationFloorLabel) + 2,
            X    = Pos.Left(destinationFloorLabel),
            Text = OtisSimConstants.NumberOfPeopleName
        };

        _capacityInput = new TextField("")
        {
            Y        = Pos.Top(capacityLabel),
            X        = Pos.Right(capacityLabel) + 1,
            Width    = Dim.Fill(),
            CanFocus = true,
        };

        var requestButton = new Button()
        {
            Text      = "Request elevator",
            Y         = Pos.Top(capacityLabel) + 2,
            X         = Pos.Center(),
            IsDefault = true
        };

        requestButton.Clicked += () =>
        {
            ProcessRequest();
        };

        requestFrameView.Add(
            originFloorLabel,
            _originFloorInput,
            destinationFloorLabel,
            _destinationFloorInput,
            capacityLabel,
            _capacityInput,
            requestButton);

        CreateElevatorTable();
        InitialiseTableDataRefresh();
        SetOriginFloorInputFocus();

        _terminalGuiApplication.Run();
    }

    /// <summary>
    /// SetOriginFloorInputFocus function
    /// </summary>
    protected virtual void SetOriginFloorInputFocus()
    {
        _originFloorInput?.SetFocus();
    }

    /// <summary>
    /// ProcessRequest function
    /// </summary>
    protected virtual void ProcessRequest()
    { 
        var request = new UserInputRequest
        {
            OriginFloorInput      = _originFloorInput!.Text,
            DestinationFloorInput = _destinationFloorInput!.Text,
            CapacityInput         = _capacityInput!.Text
        };

        var response = _elevatorControllerService.RequestElevator(request);

        if (response.Success)
        {
            _originFloorInput.Text      = string.Empty;
            _destinationFloorInput.Text = string.Empty;
            _capacityInput.Text         = string.Empty;

            ShowSuccessMessageBox("The request has been queued");
        }
        else
        {
            ShowErrorMessageBox(response.Message ?? "An unforseen error has occurred");
        }

        SetOriginFloorInputFocus();
    }

    /// <summary>
    /// ShowSuccessMessageBox function
    /// </summary>
    /// <param name="message"></param>
    protected virtual void ShowSuccessMessageBox(string message)
    {
        MessageBox.Query("Success", message, "Ok");
    }

    /// <summary>
    /// ShowErrorMessageBox function
    /// </summary>
    /// <param name="message"></param>
    protected virtual void ShowErrorMessageBox(string message)
    {
        MessageBox.ErrorQuery("Error", message, "Ok");
    }

    /// <summary>
    /// CreateElevatorTable function
    /// </summary>
    protected virtual void CreateElevatorTable()
    {
        _elevatorsTableView!.Table = new DataTable();
        ElevatorControllerService.ElevatorTableHeaders.ForEach(tableHeader =>
        {
            _elevatorsTableView.Table.Columns.Add(tableHeader);
        });

        AddElevatorRows();

        var statusFieldName = _elevatorControllerService.StatusFieldName;

        _elevatorsTableView.Style.RowColorGetter = (args) => {
            var direction = _elevatorsTableView.Table.Rows[args.RowIndex][statusFieldName].ToString();
            if (direction == ElevatorStatus.MovingUp.ToString())
                return _movingUpColorSchema;

            if (direction == ElevatorStatus.MovingDown.ToString())
                return _movingDownColorSchema;

            if (direction == ElevatorStatus.DoorsOpen.ToString())
                return _doorsOpenColorScheme;

            return _globalColorScheme;
        };

        _elevatorsTableView.Style.AlwaysShowHeaders = true;
        _elevatorsTableView.Style.ShowHorizontalScrollIndicators = true;
        _elevatorsTableView.Style.SmoothHorizontalScrolling = true;

        _elevatorsTableView.SetNeedsDisplay(); 
    }

    /// <summary>
    /// UpdateDataTable function
    /// </summary>
    protected virtual void UpdateDataTable()
    {
        try
        {
            _elevatorsTableView!.Table.Rows.Clear();
            AddElevatorRows();
            _elevatorsTableView.SetNeedsDisplay();
        }
        catch(Exception)
        {
        }
    }

    /// <summary>
    /// AddElevatorRows function
    /// </summary>
    protected virtual void AddElevatorRows()
    {
        _elevatorControllerService.ElevatorDataRows.ForEach(dataRow =>
        {
            _elevatorsTableView!.Table.Rows.Add(
                dataRow.Id,
                dataRow.Name,
                dataRow.CurrentFloor,
                dataRow.NextFloor,
                dataRow.CurrentLoad,
                dataRow.Capacity,
                dataRow.Status);
        });
    }

    /// <summary>
    /// InitialiseTableDataRefresh function
    /// </summary>
    protected virtual void InitialiseTableDataRefresh()
    {
        _cancellationTokenSource            = new CancellationTokenSource();
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        _refreshDataThread = new Thread(() =>
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(750);
                    _terminalGuiApplication.Invoke(() =>
                    {
                        UpdateDataTable();
                    });
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Data refresh thread was cancelled.");
            }
        });

        _refreshDataThread.Start();
    }

    /// <summary>
    /// UpdateRequestStatus function
    /// </summary>
    /// <param name="message"></param>
    protected virtual void UpdateRequestStatus(string message)
    {
        _terminalGuiApplication.Invoke(() =>
        {
            if (_requestStatusView != null)
            {
                _requestStatusView.Text = $"{message}{UtilityConstants.NewLineCharacter}{_requestStatusView.Text}";
            }
        });
    }
}
