using System.Reflection;

namespace Otis.Sim.Unit.Tests.TestBase.Services;

/// <summary>
/// An abstract base class for testing.
/// </summary>
public abstract class BaseTestService
{
    /// <summary>
    /// Gets a PropertyInfo[] of public instance properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="PropertyInfo"/></returns>
    protected PropertyInfo[] GetPublicInstanceProperties<T>()
        => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

    /// <summary>
    /// Gets PropertyInfo? of a nonpublic instance property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns>An array of <see cref="PropertyInfo"/></returns>
    protected PropertyInfo? GetNonPublicInstanceProperty<T>(string propertyName)
        => typeof(T)
            .GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Gets a PropertyInfo[] of public static properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="PropertyInfo"/></returns>
    protected PropertyInfo[] GetPublicStaticProperties<T>()
        => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Static);

    /// <summary>
    /// Gets a PropertyInfo[] of non public static fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected FieldInfo[] GetNonPublicStaticFields<T>()
        => typeof(T)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static);

    /// <summary>
    /// Gets an IEnumerable<string> of public instance property names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="string"/></returns>
    protected IEnumerable<string> GetPublicPropertyNames<T>()
        => GetPublicInstanceProperties<T>()
            .Select(prop => prop.Name);

    /// <summary>
    /// Gets an IEnumerable<FieldInfo> of public, static, literal fields.
    /// </summary>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected IEnumerable<FieldInfo> GetPublicStaticLiteralFieldsInfo<T>()
        => typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.IsLiteral && !field.IsInitOnly);

    /// <summary>
    /// Gets MethodInfo of a nonpublic instance method.
    /// </summary>
    /// <returns>An array of <see cref="MethodInfo"/></returns>
    protected MethodInfo? GetNonPublicInstanceMethod<T>(string methodName)
        => typeof(T)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
}
