using System.Text.RegularExpressions;

namespace Otis.Sim.Utilities.Helpers
{
    public static class StringHelper
    {
        private static readonly Regex _splitCamelCaseRegex = new Regex("([a-z])([A-Z])", RegexOptions.Compiled);

        public static string SplitCamelCase(string input) 
            => _splitCamelCaseRegex.Replace(input, "$1 $2");
    }
}
