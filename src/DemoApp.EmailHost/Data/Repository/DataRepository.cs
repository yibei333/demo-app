using DemoApp.EmailHost.Data.Context;
using DemoApp.EmailHost.Data.Entity;
using System.Linq.Expressions;

namespace DemoApp.EmailHost.Data.Repository;

public class DataRepository<TEntity>(DataContext dataContext) : IDataRepository<TEntity> where TEntity : BaseEntity
{
    public DataContext DataContext { get; } = dataContext;

    public TEntity Add(TEntity entity)
    {
        DataContext.Set<TEntity>().Add(entity);
        DataContext.SaveChanges();
        return entity;
    }

    public int AddRange(IEnumerable<TEntity> entities)
    {
        DataContext.Set<TEntity>().AddRange(entities);
        return DataContext.SaveChanges();
    }

    public TEntity Delete(TEntity entity)
    {
        DataContext.Set<TEntity>().Remove(entity);
        DataContext.SaveChanges();
        return entity;
    }

    public int DeleteRange(IEnumerable<TEntity> entities)
    {
        DataContext.Set<TEntity>().RemoveRange(entities);
        return DataContext.SaveChanges();
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
    {
        return DataContext.Set<TEntity>().FirstOrDefault(predicate);
    }

    public IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate)
    {
        return DataContext.Set<TEntity>().Where(predicate);
    }

    public TEntity Update(TEntity entity)
    {
        DataContext.Set<TEntity>().Update(entity);
        DataContext.SaveChanges();
        return entity;
    }

    public int UpdateRange(IEnumerable<TEntity> entities)
    {
        DataContext.Set<TEntity>().UpdateRange(entities);
        return DataContext.SaveChanges();
    }
}
