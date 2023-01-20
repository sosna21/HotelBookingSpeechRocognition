using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SWPSD_PROJEKT.DialogDriver;

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly HotelContext _context;

        public Repository(HotelContext context)
        {
            _context = context;
        }
        
        //Queries
        public async Task<TEntity?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().FindAsync(cancellationToken, new object[]{ id });
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _context.Set<TEntity>().AsQueryable();
        }
        
        public async Task<PaginatedList<TResult>> PaginatedListFindAsync<TResult>(int pageNumber, int pageSize, Func<TEntity, TResult> mapToResult, IQueryable<TEntity> customQuery, CancellationToken cancellationToken = default)
        {
            var count = await customQuery.CountAsync(cancellationToken);
            var items = await customQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PaginatedList<TResult>(items.Select(mapToResult), count, pageNumber, pageSize);
        }

        public async Task<bool> ContainsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _context.Set<TEntity>().Where(predicate).CountAsync(cancellationToken);
        }
        

        // Commands
        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        
    }