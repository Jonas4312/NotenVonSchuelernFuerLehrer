using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Configuration;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(n => n.Id).HasName("PK_Note");
        
        builder.Property(n => n.Id).IsRequired();
        builder.Property(n => n.FachId).IsRequired();
        builder.Property(n => n.SchuelerId).IsRequired();
        builder.Property(n => n.Wert).IsRequired();
        builder.Property(n => n.ErstelltAm).IsRequired();
        builder.Property(n => n.AngepasstAm).IsRequired();
        
        builder.HasOne(n => n.Schueler)
            .WithMany(s => s.Noten)
            .HasForeignKey(n => n.SchuelerId)
            .HasConstraintName("FK_Note_Schueler");
        
        builder.HasOne(n => n.Fach)
            .WithMany(f => f.Noten)
            .HasForeignKey(n => n.FachId)
            .HasConstraintName("FK_Note_Fach");
    }
}