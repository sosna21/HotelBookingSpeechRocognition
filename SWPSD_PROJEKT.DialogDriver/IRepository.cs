using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SWPSD_PROJEKT.DialogDriver;

public interface IRepository<TEntity> where TEntity : class, new()
{
    TEntity? FindById(int id);
    IQueryable<TEntity> GetQueryable();
    bool Contains(Expression<Func<TEntity, bool>> predicate);
    int Count(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
}