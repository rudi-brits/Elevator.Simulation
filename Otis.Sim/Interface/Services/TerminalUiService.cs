﻿using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Services;
using System.Data;
using Terminal.Gui;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;
using UiConstants = Otis.Sim.Interface.Constants.TerminalUiConstants;

namespace Otis.Sim.Interface.Services
{
    public class TerminalUiService : ConsoleFullScreenService
    {
        private ColorScheme? _globalColorScheme { get; set; }
        private ColorScheme? _idleColorScheme { get; set; }
        private TextView? _requestStatusView { get; set; }
        private TableView? _elevatorsTableView { get; set; }
        private TextField? _originFloorInput { get; set; }
        private TextField? _destinationFloorInput { get; set; }
        private TextField? _capacityInput { get; set; }

        private Thread? _refreshDataThread;
        private CancellationTokenSource? _cancellationTokenSource;

        private ElevatorControllerService _elevatorControllerService;

        public TerminalUiService(ElevatorControllerService elevatorControllerService)
        {
            _elevatorControllerService = elevatorControllerService;
            _elevatorControllerService.UpdateRequestStatus = UpdateRequestStatus;
        }

        public void InitialiseUi()
        {
            InitialiseColorSchemes();
            InitialiseFullScreen();
            InitialiseLayout();
        }

        protected void InitialiseColorSchemes()
        {
            _globalColorScheme = new ColorScheme()
            {
                Normal = UiConstants.GlobalColorAttribute
            };

            _idleColorScheme = new ColorScheme()
            {
                Normal    = UiConstants.IdleColorAttribute,
                Focus     = UiConstants.IdleColorAttribute,
                HotNormal = UiConstants.IdleColorAttribute,
                HotFocus  = UiConstants.IdleColorAttribute,
            };
        }

        private void InitialiseLayout()
        {
            Application.Init();

            var window = new Window("Elevator Simulation")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = _globalColorScheme
            };
            Application.Top.Add(window);

            var elevatorStatusFrameView = new FrameView("Elevator Status")
            {
                X = 0,
                Y = 1,
                Width = Dim.Percent(50),
                Height = Dim.Percent(70),
                CanFocus = false,
                ColorScheme = _globalColorScheme
            };
            window.Add(elevatorStatusFrameView);

            var requestFrameView = new FrameView("Request")
            {
                X = Pos.Percent(50),
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Percent(70),
                ColorScheme = _globalColorScheme
            };
            window.Add(requestFrameView);

            var requestStatusFrameView = new FrameView("Request status")
            {
                X = 0,
                Y = Pos.Percent(70) + 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = false,
                ColorScheme = _globalColorScheme
            };
            window.Add(requestStatusFrameView);

            _requestStatusView = new TextView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
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

            _originFloorInput.SetFocus();

            Application.Run();
        }

        private void ProcessRequest()
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

                showSuccessMessageBox("The request has been queued");
            }
            else
            {
                showErrorMessageBox(response.Message);
            }

            _originFloorInput.SetFocus();
        }

        private void showSuccessMessageBox(string message)
        {
            MessageBox.Query("Success", message, "Ok");
        }

        private void showErrorMessageBox(string message)
        {
            MessageBox.ErrorQuery("Error", message, "Ok");
        }

        private void CreateElevatorTable()
        {
            _elevatorsTableView!.Table = new DataTable();
            _elevatorControllerService.ElevatorTableHeaders.ForEach(tableHeader =>
            {
                _elevatorsTableView.Table.Columns.Add(tableHeader);
            });

            AddElevatorRows();

            var statusFieldName = _elevatorControllerService.StatusFieldName;

            _elevatorsTableView.Style.RowColorGetter = (args) => {
                var direction = _elevatorsTableView.Table.Rows[args.RowIndex][statusFieldName].ToString();
                if (direction == ElevatorStatus.Idle.ToString())
                {
                    return _idleColorScheme;
                }
                return _globalColorScheme;
            };

            _elevatorsTableView.Style.AlwaysShowHeaders = true;
            _elevatorsTableView.Style.ShowHorizontalScrollIndicators = true;
            _elevatorsTableView.Style.SmoothHorizontalScrolling = true;

            _elevatorsTableView.SetNeedsDisplay(); 
        }

        private void UpdateDataTable()
        {
            _elevatorsTableView!.Table.Rows.Clear();
            AddElevatorRows();
            _elevatorsTableView.SetNeedsDisplay();
        }

        private void AddElevatorRows()
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

        private void InitialiseTableDataRefresh()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _refreshDataThread = new Thread(() =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(1000);
                        Application.MainLoop.Invoke(() =>
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

        public void UpdateRequestStatus(string message)
        {
            Application.MainLoop.Invoke(() =>
            {
                if (_requestStatusView != null)
                {
                    _requestStatusView.Text = $"{message}\n{_requestStatusView.Text}";
                }
            });
        }
    }
}
