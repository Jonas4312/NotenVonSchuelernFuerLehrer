using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Configuration;

public class SchuelerConfiguration : IEntityTypeConfiguration<Schueler>
{
    public void Configure(EntityTypeBuilder<Schueler> builder)
    {
        builder.HasKey(s => s.Id).HasName("PK_Schueler");
        
        builder.Property(s => s.Id).IsRequired();
        builder.Property(s => s.KlasseId).IsRequired();
        builder.Property(s => s.Vorname).IsRequired().HasMaxLength(255);
        builder.Property(s => s.Nachname).IsRequired().HasMaxLength(255);
        builder.Property(s => s.BildByteArray).IsRequired();
        
        builder.HasOne(s => s.Klasse)
            .WithMany(k => k.Schueler)
            .HasForeignKey(s => s.KlasseId)
            .HasConstraintName("FK_Schueler_Klasse");
    }
}