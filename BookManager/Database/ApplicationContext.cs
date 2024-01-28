using BookManager.Entities;
using BookManager.Filtering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookManager.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<Book> Books => Set<Book>();

        public IConfiguration Config { get; set; }


        public ApplicationContext(IConfiguration? config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config), "Config cannot be null");
            }

            Config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionLine = Config.GetConnectionString("Default");

            optionsBuilder.UseSqlServer(connectionLine ??
                throw new ArgumentNullException(nameof(connectionLine), "Cannot find connection line"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                            .Property(a => a.Name).IsRequired();

            modelBuilder.Entity<Publisher>()
                            .Property(p => p.Name).IsRequired();

            modelBuilder.Entity<Genre>()
                            .Property(p => p.Name).IsRequired();

            modelBuilder.Entity<Book>()
                            .Property(b => b.Title).IsRequired(); ;
        }
    }
}
