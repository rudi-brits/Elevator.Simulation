using FluentValidation.Results;
using Otis.Sim.Utilities.Constants;

namespace Otis.Sim.Utilities.Extensions;

/// <summary>
/// ValidationFailureExtensions class
/// </summary>
public static class ValidationFailureExtensions
{
    /// <summary>
    /// ToNewLineString
    /// </summary>
    /// <param name="failures"></param>
    /// <returns>The string result</returns>
    public static string ToNewLineString(this List<ValidationFailure> failures)
    {
        if (!failures.Any())
        {
            return string.Empty;
        }

        return string.Join(UtilityConstants.NewLineCharacter, failures.Select(e => e.ErrorMessage));
    }
}