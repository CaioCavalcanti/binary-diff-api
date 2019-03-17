using BinaryDiff.Input.Domain.Models;

namespace BinaryDiff.Input.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Diff> DiffRepository { get; }

        IRepository<InputData> InputRepository { get; }
    }
}
