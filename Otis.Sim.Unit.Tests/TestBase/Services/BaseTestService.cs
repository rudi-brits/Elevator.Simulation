using System.Reflection;

namespace Otis.Sim.Unit.Tests.TestBase.Services;

public abstract class BaseTestService
{
    protected PropertyInfo[] GetProperties<T>()
        => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

    protected IEnumerable<string> GetPropertyNames<T>()
        => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(prop => prop.Name);
}
