using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Configuration;

public class KlasseConfiguration : IEntityTypeConfiguration<Klasse>
{
    public void Configure(EntityTypeBuilder<Klasse> builder)
    {
        builder.HasKey(k => k.Id).HasName("PK_Klasse");
        
        builder.Property(k => k.Id).IsRequired();
        builder.Property(k => k.Bezeichnung).IsRequired().HasMaxLength(255);
        builder.Property(k => k.Kurzbezeichnung).IsRequired().HasMaxLength(255);
        
        builder
            .HasMany(k => k.Lehrer)
            .WithMany(l => l.Klassen)
            .UsingEntity(
                "LehrerKlasse",
                j => j.HasOne(typeof(Lehrer)).WithMany().HasForeignKey("LehrerId").HasConstraintName("FK_LehrerKlasse_Lehrer"),
                j => j.HasOne(typeof(Klasse)).WithMany().HasForeignKey("KlasseId").HasConstraintName("FK_LehrerKlasse_Klasse"), 
                j =>
                { 
                    j.HasKey("LehrerId", "KlasseId").HasName("PK_LehrerKlasse"); 
                    j.ToTable("LehrerKlasse");
                });
    }
}