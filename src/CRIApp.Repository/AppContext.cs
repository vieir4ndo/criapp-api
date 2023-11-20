using CRIApp.Domain.Entities;
using CRIApp.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration["ConnectionStrings:Postgres"];
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
    }
}