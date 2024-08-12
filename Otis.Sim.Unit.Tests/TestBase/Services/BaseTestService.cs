using AutoMapper;
using FluentValidation.Results;
using Moq;
using Otis.Sim.Interface.Interfaces;
using Otis.Sim.MappingProfiles;
using Otis.Sim.Utilities.Constants;
using System.Reflection;
using Terminal.Gui;

namespace Otis.Sim.Unit.Tests.TestBase.Services;

/// <summary>
/// An abstract base class for testing.
/// </summary>
public abstract class BaseTestService
{
    /// <summary>
    /// _mapperConfiguration
    /// </summary>
    protected MapperConfiguration? OtisMapperConfiguration;

    /// <summary>
    /// SetupIMapper function
    /// </summary>
    /// <returns></returns>
    protected IMapper SetupIMapper()
    {
        OtisMapperConfiguration = new MapperConfiguration(config =>
        {
            config.AddProfile<OtisMappingProfile>();
        });

        var mapper = OtisMapperConfiguration.CreateMapper();

        return mapper;
    }

    /// <summary>
    /// SetupMockTerminalGuiApplication
    /// </summary>
    /// <returns>The <see cref="Mock" /> <see cref="ISimTerminalGuiApplication" />result</returns>
    protected Mock<ISimTerminalGuiApplication> BuildMockTerminalGuiApplication()
    {
        var mockTerminalGuiApplication = new Mock<ISimTerminalGuiApplication>();
        mockTerminalGuiApplication.Setup(x => x.Init()).Verifiable();
        mockTerminalGuiApplication.Setup(x => x.Top).Returns(new Toplevel());
        mockTerminalGuiApplication.Setup(x => x.Run()).Verifiable();
        mockTerminalGuiApplication.Setup(x => x.Invoke(It.IsAny<Action>())).Callback<Action>(a => a.Invoke());

        return mockTerminalGuiApplication;
    }

    /// <summary>
    /// GetMockValidationFailures creates mock <see cref="List" /> of <see cref="ValidationFailure" /> 
    /// </summary>
    /// <param name="numberOfFailures"></param>
    /// <returns></returns>
    protected static List<ValidationFailure> GetMockValidationFailures(int numberOfFailures)
    {
        var failures = new List<ValidationFailure>();
        for (var i = 0; i < numberOfFailures; i++)
            failures.Add(new ValidationFailure($"property{i}", $"message{i}"));

        return failures;
    }

    /// <summary>
    /// GetNumberOfErrorsInValidationFailuresString returns the number of newline character elements in the string.
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    protected static int SplitByNewLineCharacterLength(string errorMessage)
    {
        var errors = errorMessage.Split(new string[] { UtilityConstants.NewLineCharacter }, 
            StringSplitOptions.None);
        return errors.Length;
    }    

    /// <summary>
    /// Gets a PropertyInfo[] of public instance properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="PropertyInfo"/></returns>
    protected static PropertyInfo[] GetPublicInstanceProperties<T>()
        => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

    /// <summary>
    /// Gets PropertyInfo? of a public instance property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns>An array of <see cref="PropertyInfo"/></returns>
    protected static PropertyInfo? GetPublicInstanceProperty<T>(string propertyName)
        => typeof(T)
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

    /// <summary>
    /// Gets PropertyInfo? of a nonpublic instance property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns>An array of <see cref="PropertyInfo"/></returns>
    protected static PropertyInfo? GetNonPublicInstanceProperty<T>(string propertyName)
        => typeof(T)
            .GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Gets a PropertyInfo[] of public static properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="PropertyInfo"/></returns>
    protected static PropertyInfo[] GetPublicStaticProperties<T>()
        => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Static);

    /// <summary>
    /// Gets a PropertyInfo[] of non public static fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected static FieldInfo[] GetNonPublicStaticFields<T>()
        => typeof(T)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static);

    /// <summary>
    /// Gets a PropertyInfo[] of non public instance fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected static FieldInfo[] GetNonPublicInstanceFields<T>()
        => typeof(T)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

    // <summary>
    /// Gets PropertyInfo of a public instance field.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected static FieldInfo? GetPublicInstanceField<T>(string fieldName)
        => typeof(T)
            .GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

    // <summary>
    /// Gets PropertyInfo of a non public instance field.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected static FieldInfo? GetNonPublicInstanceField<T>(string fieldName)
        => typeof(T)
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Gets an IEnumerable<string> of public instance property names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="string"/></returns>
    protected static IEnumerable<string> GetPublicPropertyNames<T>()
        => GetPublicInstanceProperties<T>()
            .Select(prop => prop.Name);

    /// <summary>
    /// Gets an IEnumerable<FieldInfo> of public, static, literal fields.
    /// </summary>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected static IEnumerable<FieldInfo> GetPublicStaticLiteralFieldsInfo<T>()
        => typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.IsLiteral && !field.IsInitOnly);

    /// <summary>
    /// Gets MethodInfo of a public instance method.
    /// </summary>
    /// <returns>An array of <see cref="MethodInfo"/></returns>
    protected static MethodInfo? GetPublicInstanceMethod<T>(string methodName)
        => typeof(T)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

    /// <summary>
    /// Gets MethodInfo of a nonpublic instance method.
    /// </summary>
    /// <returns>An array of <see cref="MethodInfo"/></returns>
    protected static MethodInfo? GetNonPublicInstanceMethod<T>(string methodName)
        => typeof(T)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Gets a nested type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="typeName"></param>
    /// <returns></returns>
    protected static Type? GetNestedType<T>(string typeName)
        => typeof(T)
            .GetNestedType(typeName, BindingFlags.Public | BindingFlags.NonPublic);

    /// <summary>
    /// Gets a non public instance field an asserts that it is not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected static FieldInfo GetPublicInstanceFieldNotNull<T>(string propertyName)
    {
        var field = GetPublicInstanceField<T>(propertyName);
        Assert.That(field, Is.Not.Null);

        return field;
    }

    /// <summary>
    /// Gets a non public instance field an asserts that it is not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected static FieldInfo GetNonPublicInstanceFieldNotNull<T>(string fieldName)
    {
        var field = GetNonPublicInstanceField<T>(fieldName);
        Assert.That(field, Is.Not.Null);

        return field;
    }

    /// <summary>
    /// Gets a public instance field an asserts that it is not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected static PropertyInfo GetPublicInstancePropertyNotNull<T>(string propertyName)
    {
        var property = GetPublicInstanceProperty<T>(propertyName);
        Assert.That(property, Is.Not.Null);

        return property;
    }

    /// <summary>
    /// Gets a non public instance field an asserts that it is not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected static PropertyInfo GetNonPublicInstancePropertyNotNull<T>(string propertyName)
    {
        var property = GetNonPublicInstanceProperty<T>(propertyName);
        Assert.That(property, Is.Not.Null);

        return property;
    }

    /// <summary>
    /// Validates that a field value is not null and returns it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo"></param>
    /// <param name="elevatorModel"></param>
    /// <param name="isNotNull"></param>
    /// <returns></returns>
    protected static object? ValidateFieldValue<T>(FieldInfo fieldInfo, T elevatorModel, bool isNotNull = true)
    {
        var value = fieldInfo.GetValue(elevatorModel);
        AssertObjectNullability(value, isNotNull);

        return value;
    }

    /// <summary>
    /// Validates that a property value is not null and returns it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldInfo"></param>
    /// <param name="elevatorModel"></param>
    /// <param name="isNotNull"></param>
    /// <returns></returns>
    protected static object? ValidatePropertyValue<T>(PropertyInfo propertyInfo, T elevatorModel, bool isNotNull = true)
    {
        var value = propertyInfo.GetValue(elevatorModel);
        AssertObjectNullability(value, isNotNull);

        return value;
    }

    /// <summary>
    /// Assert nullability of an object
    /// </summary>
    /// <param name="testObject"></param>
    /// <param name="isNotNull"></param>
    protected static void AssertObjectNullability(object? testObject, bool isNotNull)
    {
        if (isNotNull)
            Assert.That(testObject, Is.Not.Null);
        else
            Assert.That(testObject, Is.Null);
    }

    /// <summary>
    /// Assert nullability of a type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="typeName"></param>
    protected static void GetNestedTypeNotNull<T>(string typeName)
    {
        var type = GetNestedType<T>(typeName);
        Assert.That(type, Is.Not.Null);
    }

    /// <summary>
    /// Gets a public instance method and tests nullability
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodName"></param>
    /// <returns></returns>
    protected static MethodInfo GetPublicInstanceMethodNotNull<T>(string methodName)
    {
        var method = GetPublicInstanceMethod<T>(methodName);
        Assert.That(method, Is.Not.Null);

        return method;
    }

    /// <summary>
    /// Gets a nonpublic instance method and tests nullability
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodName"></param>
    /// <returns></returns>
    protected static MethodInfo GetNonPublicInstanceMethodNotNull<T>(string methodName)
    {
        var method = GetNonPublicInstanceMethod<T>(methodName);
        Assert.That(method, Is.Not.Null);

        return method;
    }
}
