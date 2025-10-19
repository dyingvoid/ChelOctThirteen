using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class ArchiveConfiguration : IEntityTypeConfiguration<Archive>
{
    public void Configure(EntityTypeBuilder<Archive> builder)
    {
        builder.ToTable("delta_archive");
        builder.HasKey(a => a.VersionId);
        builder.Property(a => a.VersionId)
            .HasColumnName("version_id")
            .ValueGeneratedNever();
        
        builder.Property(a => a.ArchiveLink)
            .HasColumnName("archive_link")
            .IsRequired();
        builder.Property(a => a.ExpDate)
            .HasColumnName("exp_date")
            .IsRequired();

        builder.Property(a => a.DownloadedAt)
            .HasColumnName("downloaded_at")
            .HasDefaultValueSql("NOW()")
            .HasColumnType("timestamptz")
            .IsRequired();
        
        builder.Property(a => a.UnarchivedAt)
            .HasColumnName("unarchived_at")
            .HasColumnType("timestamptz");
    }
}