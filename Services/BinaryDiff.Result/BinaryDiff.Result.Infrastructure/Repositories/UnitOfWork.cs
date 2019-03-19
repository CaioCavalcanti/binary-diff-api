using BinaryDiff.Result.Infrastructure.Database;
using BinaryDiff.Result.Infrastructure.Repositories.Implementation;
using BinaryDiff.Shared.Infrastructure.RelationalDatabase;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public class UnitOfWork : BaseUnitOfWork<ResultContext>, IUnitOfWork
    {
        private IDiffResultsRepository _diffResultsRepository;
        private IInputDifferencesRepository _inputDifferencesRepository;

        public UnitOfWork(ResultContext context) : base(context) { }

        public IDiffResultsRepository DiffResultsRepository
        {
            get
            {
                if (_diffResultsRepository == null)
                {
                    _diffResultsRepository = new DiffResultsRepository(context);
                }

                return _diffResultsRepository;
            }
        }

        public IInputDifferencesRepository InputDifferencesRepository
        {
            get
            {
                if (_inputDifferencesRepository == null)
                {
                    _inputDifferencesRepository = new InputDifferencesRepository(context);
                }

                return _inputDifferencesRepository;
            }
        }
    }
}
