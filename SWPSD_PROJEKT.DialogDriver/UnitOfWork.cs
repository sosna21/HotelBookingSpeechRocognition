using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace SWPSD_PROJEKT.DialogDriver
{
    public class UnitOfWork
    {
        private readonly HotelContext _context;
        private readonly Hashtable _repositories = new();

        public UnitOfWork(HotelContext context)
        {
            _context = context;
        }
        
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, new()
        {
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<TEntity>)_repositories[type]!;
        }

        public Task<int> Complete( CancellationToken cancellationToken = default ) {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        
    }
}