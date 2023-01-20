using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SWPSD_PROJEKT.DialogDriver;

public interface IRepository<TEntity> where TEntity : class, new()
{
    Task<TEntity?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<TEntity> GetQueryable();
    Task<PaginatedList<TResult>> PaginatedListFindAsync<TResult>(int pageNumber, int pageSize, Func<TEntity, TResult> mapToResult, IQueryable<TEntity> customQuery, CancellationToken cancellationToken = default);
    Task<bool> ContainsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
}