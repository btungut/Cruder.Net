using Cruder.Core.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cruder.Core.Contract
{
    public interface ICruderRepository<TEntity> : IDisposable
    {
        Result<int> Delete(object id);
        Result<int> Delete(TEntity entity);

        Result<int> Save();
        Result<int> Save(TEntity entity);
        Result<int> Save(TEntity entity, ActionType actionType);

        Task<Result<int>> DeleteAsync(object id);
        Task<Result<int>> DeleteAsync(TEntity entity);

        Task<Result<int>> SaveAsync();
        Task<Result<int>> SaveAsync(TEntity entity);
        Task<Result<int>> SaveAsync(TEntity entity, ActionType actionType);

        IQueryable<TEntity> Query();
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, QueryOptions options);
    }
}
