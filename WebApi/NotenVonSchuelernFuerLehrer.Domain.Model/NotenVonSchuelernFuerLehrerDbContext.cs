using Microsoft.EntityFrameworkCore;

namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class NotenVonSchuelernFuerLehrerDbContext : DbContext
{
    public NotenVonSchuelernFuerLehrerDbContext(DbContextOptions<NotenVonSchuelernFuerLehrerDbContext> options) : base(options)
    {
    }
    
    public required DbSet<Fach> Fach { get; init; }
    public required DbSet<Klasse> Klasse { get; init; }
    public required DbSet<Lehrer> Lehrer { get; init; }
    public required DbSet<Note> Note { get; init; }
    public required DbSet<Schueler> Schueler { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}