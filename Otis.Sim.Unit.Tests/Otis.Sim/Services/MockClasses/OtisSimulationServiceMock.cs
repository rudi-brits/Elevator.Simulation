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
        /// CalledWriteLineToConsole field.
        /// </summary>
        public bool CalledWriteLineToConsole = false;

        /// <summary>
        /// CallBaseRunSimulation field.
        /// </summary>
        public bool CallBaseRunSimulation = false;

        /// <summary>
        /// WriteLineToConsoleValue
        /// </summary>
        public string? WriteLineToConsoleValue;
        /// <summary>
        /// ThrowWriteLineException
        /// </summary>
        public bool ThrowWriteLineException = false;

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

        protected override void WriteLineToConsole(string message)
        {
            CalledWriteLineToConsole = true;
            if (ThrowWriteLineException)
                throw new Exception(nameof(ThrowWriteLineException));

            WriteLineToConsoleValue = message;
        }
    }
}
