using BookManager.Filtering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookManager.Configuration
{
    public class ConfiguratorJson : IConfigurator
    {
        public IConfiguration Config { get; }

        public ConfiguratorJson(string jsonName, string subFolder = "Configuration")
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(@$"{Directory.GetCurrentDirectory()}\{subFolder}")
                .AddJsonFile(jsonName)
                .Build();
        }

        public IConfiguration GetConfig()
        {
            return Config;
        }

        public Filter GetFilter(string sectionName)
        {
            return Config.GetSection(sectionName).Get<Filter>()
                   ?? throw new InvalidDataException("Cannot create filter from file");
        }

        public void TrimSectionValues(string sectionName)
        {
            foreach (IConfigurationSection? section in Config.GetSection(sectionName).GetChildren())
            {
                if (section is null)
                {
                    continue;
                }

                section.Value = section.Value?.Trim();
            }
        }
    }
}
