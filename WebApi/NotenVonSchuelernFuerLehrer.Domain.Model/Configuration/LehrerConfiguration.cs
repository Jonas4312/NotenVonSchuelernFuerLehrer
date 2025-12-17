using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Configuration;

public class LehrerConfiguration : IEntityTypeConfiguration<Lehrer>
{
    public void Configure(EntityTypeBuilder<Lehrer> builder)
    {
        builder.HasKey(l => l.Id).HasName("PK_Lehrer");
        
        builder.Property(l => l.Id).IsRequired();
        builder.Property(l => l.Vorname).IsRequired().HasMaxLength(255);
        builder.Property(l => l.Nachname).IsRequired().HasMaxLength(255);
        builder.Property(l => l.Benutzername).IsRequired().HasMaxLength(255);
        // Keine Unique-Constraint mehr, da SoftDelete gelöschte Lehrer beibehält.
        // Die Eindeutigkeit wird programmatisch beim Erstellen/Ändern geprüft (nur aktive Lehrer).
        builder.Property(l => l.PasswortHash).IsRequired().HasMaxLength(255);
        builder.Property(l => l.BildByteArray).IsRequired();
        
        builder.HasQueryFilter(l => !l.IsDeleted);
        
        builder
            .HasMany(l => l.Faecher)
            .WithMany(f => f.Lehrer)
            .UsingEntity(
                "LehrerFach", 
                j => j.HasOne(typeof(Fach)).WithMany().HasForeignKey("FachId").HasConstraintName("FK_LehrerFach_Fach"), 
                j => j.HasOne(typeof(Lehrer)).WithMany().HasForeignKey("LehrerId").HasConstraintName("FK_LehrerFach_Lehrer"), 
                j => 
                { 
                    j.HasKey("LehrerId", "FachId").HasName("PK_LehrerFach"); 
                    j.ToTable("LehrerFach");
                });
    }
}