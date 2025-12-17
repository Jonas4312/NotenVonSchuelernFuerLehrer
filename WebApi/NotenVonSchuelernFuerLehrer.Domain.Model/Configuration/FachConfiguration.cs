using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Configuration;

public class FachConfiguration : IEntityTypeConfiguration<Fach>
{
    public void Configure(EntityTypeBuilder<Fach> builder)
    {
        builder.HasKey(f => f.Id).HasName("PK_Fach");

        builder.Property(f => f.Id).IsRequired();
        builder.Property(f => f.Bezeichnung).IsRequired().HasMaxLength(255);
        builder.Property(f => f.Kurzbezeichnung).IsRequired().HasMaxLength(255);
        
        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}