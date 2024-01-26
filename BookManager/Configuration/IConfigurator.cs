using BookManager.Filtering;
using Microsoft.Extensions.Configuration;

namespace BookManager.Configuration
{
    public interface IConfigurator
    {
        IConfiguration Config { get; }

        Filter GetFilter(string sectionName);
    }
}
