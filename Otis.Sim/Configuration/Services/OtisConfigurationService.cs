using Otis.Sim.Configuration.Models;
using System.Text.Json;

namespace Otis.Sim.Configuration.Services
{
    public class OtisConfigurationService
    {
        private OtisConfiguration _configuration { get; set; }
        public OtisConfiguration Configuration => _configuration;

        public void LoadConfiguration()
        {
            string jsonConfiguration = File.ReadAllText("appsettings.json");
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var otisConfiguration = JsonSerializer.Deserialize<OtisConfiguration>(jsonConfiguration, serializerOptions);
            
            _configuration = otisConfiguration;
        }
    }
}
