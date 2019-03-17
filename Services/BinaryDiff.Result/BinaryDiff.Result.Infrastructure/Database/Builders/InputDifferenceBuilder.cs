using BinaryDiff.Result.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BinaryDiff.Result.Infrastructure.Database.Builders
{
    public static class InputDifferenceBuilder
    {
        public static void BuildModel(this EntityTypeBuilder<InputDifference> builder)
        {
            builder.HasKey(dr => dr.Id);

            builder.HasIndex(dr => dr.ResultId);
        }
    }
}
