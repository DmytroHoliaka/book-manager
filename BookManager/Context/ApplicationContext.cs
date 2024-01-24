using BookManager.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookManager.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Genre> Genres => Set<Genre>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("Configurations.json")
                                        .Build();

            string? connectionLine = config.GetConnectionString("Default");
            optionsBuilder.UseSqlServer(connectionLine ?? 
                throw new ArgumentNullException(nameof(connectionLine), "Cannot find connection line"));
        }
    }
}
