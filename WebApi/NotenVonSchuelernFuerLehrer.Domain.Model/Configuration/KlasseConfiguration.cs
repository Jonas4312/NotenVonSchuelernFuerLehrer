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
            .HasMany(k => k.Faecher)
            .WithMany(f => f.Klassen)
            .UsingEntity(
                "KlasseFach",
                j => j.HasOne(typeof(Fach)).WithMany().HasForeignKey("FachId").HasConstraintName("FK_KlasseFach_Fach"),
                j => j.HasOne(typeof(Klasse)).WithMany().HasForeignKey("KlasseId").HasConstraintName("FK_KlasseFach_Klasse"), 
                j =>
                { 
                    j.HasKey("KlasseId", "FachId").HasName("PK_KlasseFach"); 
                    j.ToTable("KlasseFach");
                });
    }
}