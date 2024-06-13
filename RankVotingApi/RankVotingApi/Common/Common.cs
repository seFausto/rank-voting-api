using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace RankVotingApi.Common
{
    public class Common
    {
        protected Common()
        {
            
        }

        public static Ranking JsonDeserialize(string value)
        {
            return JsonSerializer.Deserialize<Ranking>(value);
        }

        public static IEnumerable<KeyValuePair<string, string>> GetKafkaConfiguration(IConfigurationRoot configuration)
        {
            var configurationRoot = configuration;
            var iniProvider = configurationRoot.Providers
                        .FirstOrDefault(p => p is Microsoft.Extensions.Configuration.Ini.IniConfigurationProvider);

            List<KeyValuePair<string, string>> iniSettings = [];

            if (iniProvider == null)
            {
                return iniSettings;
            }

            // Iterate through all the settings and add them to the list
            foreach (var key in configuration.AsEnumerable().Select(x=> x.Key))
            {
                if (iniProvider.TryGet(key, out var value))
                {
                    iniSettings.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            return iniSettings;
        }
    }
}
