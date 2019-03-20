using BinaryDiff.Result.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BinaryDiff.Result.Infrastructure.Database.Builders
{
    public static class DiffResultBuilder
    {
        public static void BuildModel(this EntityTypeBuilder<DiffResult> builder)
        {
            builder.HasKey(dr => dr.Id);

            builder.HasIndex(dr => dr.DiffId);

            builder
                .Property(dr => dr.DiffId)
                .IsRequired();

            builder
                .Property(dr => dr.Result)
                .IsRequired();

            builder
                .Property(dr => dr.Timestamp)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder
                .Property(dr => dr.TriggerInputId)
                .IsRequired();

            builder
                .Property(dr => dr.TriggerInputPosition)
                .IsRequired();

            builder
                .HasMany(dr => dr.Differences)
                .WithOne(d => d.Result)
                .HasForeignKey(d => d.ResultId);
        }
    }
}
