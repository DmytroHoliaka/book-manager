using BookManager.Filtering;
using Microsoft.Extensions.Configuration;

namespace BookManager.Configuration
{
    public class ConfiguratorJson : IConfigurator
    {
        public IConfiguration Config { get; }

        public ConfiguratorJson(string jsonName)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + @"\Configuration\")
                .AddJsonFile(jsonName)
                .Build();
        }

        public IConfiguration GetConfig()
        {
            return Config;
        }

        public Filter GetFilter(string sectionName)
        {
            return Config.GetSection(sectionName).Get<Filter>() ??
                throw new InvalidDataException("Cannot create filter from json file");
        }
    }
}
