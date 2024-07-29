using Otis.Sim.Configuration.Models;
using Otis.Sim.Configuration.Validators;
using Otis.Sim.Utilities.Extensions;
using System.Text.Json;

namespace Otis.Sim.Configuration.Services
{
    public class OtisConfigurationService
    {
        private OtisConfiguration? _configuration { get; set; }

        public BuildingConfiguration? BuildingConfiguration => _configuration?.BuildingConfiguration;
        public List<ElevatorConfiguration>? ElevatorsConfiguration => _configuration?.ElevatorsConfiguration;

        public void LoadConfiguration()
        {
            string jsonConfiguration = File.ReadAllText("appsettings.json");
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var otisConfiguration = JsonSerializer.Deserialize<OtisConfiguration>(jsonConfiguration, serializerOptions);

            var validator = new OtisConfigurationValidator();
            var validationResult = validator.Validate(otisConfiguration);

            if (!validationResult.IsValid)
            {
                throw new Exception(validationResult.Errors.ToNewLineString());
            }

            _configuration = otisConfiguration;
        }
    }
}
