using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Validators;
using Otis.Sim.Utilities.Extensions;
using System.Text.Json;

namespace Otis.Sim.Configuration.Services;

/// <summary>
/// OtisConfigurationService
/// </summary>
public class OtisConfigurationService
{
    /// <summary>
    /// _configuration
    /// </summary>
    private OtisConfiguration? _configuration { get; set; }
    /// <summary>
    /// BuildingConfiguration
    /// </summary>
    public BuildingConfiguration? BuildingConfiguration => _configuration?.BuildingConfiguration;
    /// <summary>
    /// ElevatorsConfiguration
    /// </summary>
    public List<ElevatorConfiguration>? ElevatorsConfiguration => _configuration?.ElevatorsConfiguration;

    /// <summary>
    /// LoadConfiguration
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void LoadConfiguration()
    {
        string jsonConfiguration = ReadAppSettings();
        var serializerOptions    = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var otisConfiguration    = JsonSerializer.Deserialize<OtisConfiguration>(jsonConfiguration, serializerOptions);

        var validator        = new OtisConfigurationValidator();
        var validationResult = validator.Validate(otisConfiguration!);

        if (!validationResult.IsValid)
        {
            throw new ArgumentException(validationResult.Errors.ToNewLineString());
        }

        _configuration = otisConfiguration;
    }

    /// <summary>
    /// ReadAppSettings
    /// </summary>
    /// <returns>The string result</returns>
    protected virtual string ReadAppSettings()
        => File.ReadAllText("appsettings.json");
}
