using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CRIApp.Repository;

public class AppContext : DbContext
{
    public AppContext(DbContextOptions<AppContext> options) : base(options) { }

    public DbSet<Answer> Answer { get; set; }
    public DbSet<AudiovisualResource> AudiovisualResource { get; set; }
    public DbSet<Award> Award { get; set; }
    public DbSet<Child> Child { get; set; }
    public DbSet<ChildBook> ChildBook { get; set; }
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Faq> Faq { get; set; }
    public DbSet<Forum> Forum { get; set; }
    public DbSet<Question> Question { get; set; }
    public DbSet<Recommendation> Recommendation { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserResetPasswordCode> UserResetPasswordCode { get; set; }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
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