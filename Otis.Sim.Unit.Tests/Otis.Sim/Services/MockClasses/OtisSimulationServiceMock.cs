using Otis.Sim.Services;

namespace Otis.Sim.Unit.Tests.Otis.Sim.MappingProfiles.MockClasses
{
    /// <summary>
    /// OtisSimulationServiceMock extends the <see cref="OtisSimulationService" /> class.
    /// </summary>
    public class OtisSimulationServiceMock : OtisSimulationService
    {
        /// <summary>
        /// RunSimulation field.
        /// </summary>
        public bool CalledRunSimulation = false;
        /// <summary>
        /// CalledSetupServiceCollection
        /// </summary>
        public bool CalledSetupServiceCollection = false;
        /// <summary>
        /// RunSimulation field.
        /// </summary>
        public bool CalledLoadAppConfiguration = false;
        /// <summary>
        /// CalledLoadElevatorControllerConfiguration
        /// </summary>
        public bool CalledLoadElevatorControllerConfiguration = false;
        /// <summary>
        /// CalledLoadElevatorControllerConfiguration
        /// </summary>
        public bool CalledInitialiseTerminalUi = false;
        /// <summary>
        /// CalledWriteLineToConsole field.
        /// </summary>
        public bool CalledWriteLineToConsole = false;
        /// <summary>
        /// CalledDisplayUserInputMessage
        /// </summary>
        public bool CalledDisplayUserInputMessage = false;

        /// <summary>
        /// CallBaseRunSimulation field.
        /// </summary>
        public bool CallBaseRunSimulation = false;
        /// <summary>
        /// CallBaseSetupServiceCollection
        /// </summary>
        public bool CallBaseSetupServiceCollection = false;
        /// <summary>
        /// CallBaseLoadAppConfiguration
        /// </summary>
        public bool CallBaseLoadAppConfiguration = false;
        /// <summary>
        /// CallBaseLoadAppConfiguration
        /// </summary>
        public bool CallBaseLoadElevatorControllerConfiguration = false;
        /// <summary>
        /// CallBaseInitialiseTerminalUi
        /// </summary>
        public bool CallBaseInitialiseTerminalUi = false;

        /// <summary>
        /// WriteLineToConsoleValue
        /// </summary>
        public string? WriteLineToConsoleValue;
        /// <summary>
        /// ThrowWriteLineException
        /// </summary>
        public bool ThrowWriteLineException = false;
        /// <summary>
        /// ThrowWriteLineException
        /// </summary>
        public bool ThrowLoadAppConfigurationException = false;
        /// <summary>
        /// ThrowSetupServiceCollectionException
        /// </summary>
        public bool ThrowSetupServiceCollectionException = false;
        /// <summary>
        /// ThrowLoadElevatorControllerConfiguration
        /// </summary>
        public bool ThrowLoadElevatorControllerConfiguration = false;
        /// <summary>
        /// CalledDisplayUserInputMessageValue
        /// </summary>
        public string? DisplayUserInputMessageValue;

        /// <summary>
        /// BaseMessageServiceMock constructor
        /// </summary>
        public OtisSimulationServiceMock()
            : base()
        {
        }

        /// <summary>
        /// RunSimulation
        /// </summary>
        public override void RunSimulation()
        {
            CalledRunSimulation = true;
            if (CallBaseRunSimulation)
                base.RunSimulation();
        }

        /// <summary>
        /// SetupServiceCollection
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void SetupServiceCollection()
        {
            CalledSetupServiceCollection = true;
            if (CallBaseSetupServiceCollection)
                base.SetupServiceCollection();

            if (ThrowSetupServiceCollectionException)
                throw new Exception(nameof(ThrowSetupServiceCollectionException));
        }

        /// <summary>
        /// LoadAppConfiguration
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void LoadAppConfiguration()
        {
            CalledLoadAppConfiguration = true;
            if (CallBaseLoadAppConfiguration)
                base.LoadAppConfiguration();

            if (ThrowLoadAppConfigurationException)
                throw new Exception(nameof(ThrowLoadAppConfigurationException));
        }

        /// <summary>
        /// LoadElevatorControllerConfiguration
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void LoadElevatorControllerConfiguration()
        {
            CalledLoadElevatorControllerConfiguration = true;
            if (CallBaseLoadElevatorControllerConfiguration)
                base.LoadElevatorControllerConfiguration();

            if (ThrowLoadElevatorControllerConfiguration)
                throw new Exception(nameof(ThrowLoadElevatorControllerConfiguration));
        }

        /// <summary>
        /// InitialiseTerminalUi
        /// </summary>
        protected override void InitialiseTerminalUi()
        {
            CalledInitialiseTerminalUi = true;
            if (CallBaseInitialiseTerminalUi)
                base.InitialiseTerminalUi();
        }

        /// <summary>
        /// WriteLineToConsole
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="Exception"></exception>
        protected override void WriteLineToConsole(string message)
        {
            CalledWriteLineToConsole = true;
            if (ThrowWriteLineException)
                throw new Exception(nameof(ThrowWriteLineException));

            WriteLineToConsoleValue = message;
        }

        /// <summary>
        /// DisplayUserInputMessage
        /// </summary>
        /// <param name="message"></param>
        protected override void DisplayUserInputMessage(string message)
        {
            CalledDisplayUserInputMessage = true;
            DisplayUserInputMessageValue = message;
        }
    }
}
