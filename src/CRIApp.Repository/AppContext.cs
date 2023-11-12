using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CRIApp.Repository;

public class AppContext : DbContext
{
    public DbSet<Answer> Answer { get; set; }
    public DbSet<AudiovisualResource> AudiovisualResource { get; set; }
    public DbSet<Award> Award { get; set; }
    public DbSet<Child> Child { get; set; }
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Faq> Faq { get; set; }
    public DbSet<Forum> Forum { get; set; }
    public DbSet<Question> Question { get; set; }
    public DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath($"{Directory.GetCurrentDirectory()}/../CRIApp.Api")
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();
            var connectionString = configuration["ConnectionString:Postgres"];
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
    }
}