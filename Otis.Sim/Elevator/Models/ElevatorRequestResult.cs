using FluentValidation.Results;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Elevator.Models
{
    public class ElevatorRequestResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }

        public ElevatorRequestResult(bool success = true)
        {
            Success = success;
        }

        public ElevatorRequestResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ElevatorRequestResult(List<ValidationFailure> validationErrors)
        {
            Message = validationErrors.ToNewLineString();
        }
    }
}
