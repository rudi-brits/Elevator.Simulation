using FluentValidation.Results;

namespace Otis.Sim.Utilities.Extensions
{
    public static class ValidationFailureExtensions
    {
        public static string ToNewLineString(this List<ValidationFailure> failures)
        {
            if (!failures.Any())
            {
                return string.Empty;
            }

            return string.Join("\n", failures.Select(e => e.ErrorMessage));
        }
    }
}