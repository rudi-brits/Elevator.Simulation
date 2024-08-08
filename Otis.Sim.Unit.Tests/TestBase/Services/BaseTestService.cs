﻿using AutoMapper;
using FluentValidation.Results;
using Otis.Sim.MappingProfiles;
using Otis.Sim.Utilities.Constants;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.TestBase.Services;

/// <summary>
/// An abstract base class for testing.
/// </summary>
public abstract class BaseTestService
{
    /// <summary>
    /// SetupIMapper function
    /// </summary>
    /// <returns></returns>
    protected IMapper SetupIMapper()
    {
        var configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<OtisMappingProfile>();
        });

        var mapper = new Mapper(configuration);

        return mapper;
    }

    /// <summary>
    /// GetMockValidationFailures creates mock <see cref="List" /> of <see cref="ValidationFailure" /> 
    /// </summary>
    /// <param name="numberOfFailures"></param>
    /// <returns></returns>
    protected List<ValidationFailure> GetMockValidationFailures(int numberOfFailures)
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
    protected int SplitByNewLineCharacterLength(string errorMessage)
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
    /// Gets a PropertyInfo[] of non public instance fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected FieldInfo[] GetNonPublicInstanceFields<T>()
        => typeof(T)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

    // <summary>
    /// Gets PropertyInfo of a non public instance field.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of <see cref="FieldInfo"/></returns>
    protected FieldInfo? GetNonPublicInstanceField<T>(string fieldName)
        => typeof(T)
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

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
    /// Gets MethodInfo of a public instance method.
    /// </summary>
    /// <returns>An array of <see cref="MethodInfo"/></returns>
    protected MethodInfo? GetPublicInstanceMethod<T>(string methodName)
        => typeof(T)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

    /// <summary>
    /// Gets MethodInfo of a nonpublic instance method.
    /// </summary>
    /// <returns>An array of <see cref="MethodInfo"/></returns>
    protected MethodInfo? GetNonPublicInstanceMethod<T>(string methodName)
        => typeof(T)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
}
