using Aydex.SemanticKernel.NL2EF.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Aydex.SemanticKernel.NL2EF.Data;

public class AppDbContext : DbContext
{
    public const string SchemaMemoryCollectionName = $"Schema-{nameof(AppDbContext)}";

    public DbSet<Director> Directors { get; set; } = default!;
    public DbSet<Gender> Genders { get; set; } = default!;
    public DbSet<Movie> Movies { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
