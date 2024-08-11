using System.Text.RegularExpressions;

namespace Otis.Sim.Utilities.Helpers;

/// <summary>
/// StringHelper
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// _splitCamelCaseRegex regex
    /// </summary>
    private static readonly Regex _splitCamelCaseRegex = new("([a-z])([A-Z])", RegexOptions.Compiled);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns>The string result</returns>
    public static string SplitCamelCase(string input) 
        => _splitCamelCaseRegex.Replace(input, "$1 $2");
}
