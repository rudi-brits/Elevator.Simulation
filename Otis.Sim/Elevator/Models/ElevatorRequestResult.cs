using FluentValidation.Results;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Elevator.Models;

/// <summary>
/// ElevatorRequestResult class
/// </summary>
public class ElevatorRequestResult
{
    /// <summary>
    /// Success 
    /// </summary>
    public bool Success { get; set; } = false;
    /// <summary>
    /// Message 
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// ElevatorRequestResult constructor
    /// </summary>
    /// <param name="success"></param>
    public ElevatorRequestResult(bool success = true)
    {
        Success = success;
    }

    /// <summary>
    /// ElevatorRequestResult constructor
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    public ElevatorRequestResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    /// <summary>
    /// ElevatorRequestResult constructor
    /// </summary>
    /// <param name="validationErrors"></param>
    public ElevatorRequestResult(List<ValidationFailure> validationErrors)
    {
        Message = validationErrors.ToNewLineString();
    }
}
