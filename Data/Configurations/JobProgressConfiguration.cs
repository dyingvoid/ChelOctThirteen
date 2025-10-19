using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class JobProgressConfiguration : IEntityTypeConfiguration<JobProgress>
{
    public void Configure(EntityTypeBuilder<JobProgress> builder)
    {
        builder.ToTable("job_progress");
        builder.HasKey(x => x.JobId);
        builder.Property(x => x.JobId).HasColumnName("job_id");
        
        builder.Property(x => x.LastCheckpoint)
            .HasColumnName("last_checkpoint")
            .IsRequired();
        builder.Property(x => x.IsComplete)
            .HasColumnName("is_complete")
            .IsRequired();
    }
}