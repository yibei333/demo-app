using DemoApp.EmailHost.Data.Entity;
using System.Linq.Expressions;

namespace DemoApp.EmailHost.Data.Repository;

public interface IDataRepository<TEntity> where TEntity : BaseEntity
{
    TEntity Add(TEntity entity);
    int AddRange(IEnumerable<TEntity> entities);
    TEntity Update(TEntity entity);
    int UpdateRange(IEnumerable<TEntity> entities);
    TEntity Delete(TEntity entity);
    int DeleteRange(IEnumerable<TEntity> entities);
    TEntity? Get(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate);
}
