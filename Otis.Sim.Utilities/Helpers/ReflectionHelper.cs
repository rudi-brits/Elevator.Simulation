using System.Reflection;

namespace Otis.Sim.Utilities.Helpers
{
    public static class ReflectionHelper
    {
        public static List<string> GetFormattedPropertyNames<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => StringHelper.SplitCamelCase(p.Name))
                .ToList();
        }
    }
}
