using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Database.Builders;
using Microsoft.EntityFrameworkCore;

namespace BinaryDiff.Result.Infrastructure.Database
{
    public class ResultContext : DbContext
    {
        public ResultContext(DbContextOptions options) : base(options) { }

        public DbSet<DiffResult> DiffResults { get; set; }

        public DbSet<InputDifference> Differences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiffResult>().BuildModel();
            modelBuilder.Entity<InputDifference>().BuildModel();
        }
    }
}
