using System.Reflection;

namespace Otis.Sim.Utilities.Helpers;

/// <summary>
/// ReflectionHelper class
/// </summary>
public static class ReflectionHelper
{
    /// <summary>
    /// GetFormattedPropertyNames
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<string> GetFormattedPropertyNames<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => StringHelper.SplitCamelCase(p.Name))
            .ToList();
}
