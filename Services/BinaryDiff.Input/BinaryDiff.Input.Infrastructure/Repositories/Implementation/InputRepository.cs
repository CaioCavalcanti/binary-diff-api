using BinaryDiff.Input.Domain.Models;

namespace BinaryDiff.Input.Infrastructure.Repositories.Implementation
{
    public class InputRepository : DocumentRepository<InputData>, IInputRepository
    {
        public InputRepository(IMongoContext context) : base(context.Inputs)
        {
        }
    }
}
