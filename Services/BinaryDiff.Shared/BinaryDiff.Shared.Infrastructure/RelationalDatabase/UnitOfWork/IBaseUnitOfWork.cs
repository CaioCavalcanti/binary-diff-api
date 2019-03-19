using System;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.RelationalDatabase
{
    public interface IBaseUnitOfWork : IDisposable
    {
        Task SaveChangesAsync();
    }
}
