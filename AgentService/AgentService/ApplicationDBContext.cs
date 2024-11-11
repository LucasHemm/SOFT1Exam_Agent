using AgentService.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentService;

public class ApplicationDbContext : DbContext
{

    public DbSet<Agent> Agents { get; set; }


    // Constructor that accepts DbContextOptions
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure SQL Server if no options are provided (to avoid overriding options in tests)
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=AgentServiceDB;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;");
        }
    }
}
