namespace Otis.Sim.Unit.Tests.Constants;

/// <summary>
/// A list of category constants.
/// </summary>
public abstract class TestConstants
{
    /// <summary>
    /// The Utilities constant.
    /// </summary>
    public const string Utilities = nameof(Utilities);
    /// <summary>
    /// The UtilitiesConstantsCategory constant.
    /// </summary>
    public const string UtilitiesConstantsCategory = $"{Utilities}-Constants";
    /// <summary>
    /// The ExtensionsCategory constant.
    /// </summary>
    public const string ExtensionsCategory = $"{Utilities}-Extensions";
    /// <summary>
    /// The HelperCategory constant.
    /// </summary>
    public const string HelperCategory = $"{Utilities}-Helper";
    /// <summary>
    /// The ConfigurationCategory constant.
    /// </summary>
    public const string ConfigurationCategory = "Configuration";
    /// <summary>
    /// The Constants constant.
    /// </summary>
    public const string Constants = "Constants";
    /// <summary>
    /// The Elevator constant.
    /// </summary>
    public const string Elevator = "Elevator";
    /// <summary>
    /// The Interface constant.
    /// </summary>
    public const string Interface = "Interface";
    /// <summary>
    /// The MappingProfilesCategory constant
    /// </summary>
    public const string MappingProfilesCategory = "Mapping Profiles";
    /// <summary>
    /// The MessagesCategory constant.
    /// </summary>
    public const string MessagesCategory = "Messages";
    /// <summary>
    /// The MappingProfilesCategory constant
    /// </summary>
    public const string SimulationServiceCategory = "Simulation Service";

    /// <summary>
    /// The EmptyPropertiesMessage constant.
    /// </summary>
    public const string EmptyPropertiesMessage = "The list of properties is empty.";
    /// <summary>
    /// The EmptyLiteralFieldsMessage constant.
    /// </summary>
    public const string EmptyLiteralFieldsMessage = "Literal fields were not retrieved.";
    /// <summary>
    /// The HasAnEmptyValue constant.
    /// </summary>
    public const string HasAnEmptyValue = "has an empty value";

    public static string NewLineMessageLengthError(int expectedLength, int actualLength)
        => $"Expected {expectedLength} new line messages, received {actualLength}";
}
