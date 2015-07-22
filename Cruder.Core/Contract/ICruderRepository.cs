using Cruder.Core.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cruder.Core.Contract
{
    public interface ICruderRepository<T> : IDisposable
    {
        Result<int> Delete(object id);
        Result<int> Delete(T entity);

        Result<int> Save();
        Result<int> Save(T entity);
        Result<int> Save(T entity, ActionType actionType);

        Task<Result<int>> DeleteAsync(object id);
        Task<Result<int>> DeleteAsync(T entity);

        Task<Result<int>> SaveAsync();
        Task<Result<int>> SaveAsync(T entity);
        Task<Result<int>> SaveAsync(T entity, ActionType actionType);

        IQueryable<T> Query();
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        IQueryable<T> Query(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, QueryOptions options);
    }
}
