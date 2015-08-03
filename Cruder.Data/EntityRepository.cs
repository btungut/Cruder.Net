using Cruder.Core;
using Cruder.Core.Contract;
using Cruder.Core.ExceptionHandling;
using Cruder.Core.Module;
using Cruder.Core.Repository;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cruder.Data
{
    public abstract class EntityRepository<T> : BaseRepository<T>, ICruderRepository<T>, IDisposable
        where T : class, IEntity
    {
        protected DbContext DbContext { get; private set; }
        protected DbSet<T> Tracking { get; private set; }

        public EntityRepository()
            : this(IoC.Resolve<DbContext>())
        {
        }

        public EntityRepository(DbContext dbContext)
        {
            this.DbContext = dbContext;
            this.Tracking = DbContext.Set<T>();
        }

        public override IQueryable<T> Queryable
        {
            get { return Tracking; }
        }

        public Result<int> Delete(object id)
        {
            if (id == null) throw new ArgumentNullException("id");

            T entity = Tracking.Find(id);
            return this.Delete(entity);
        }

        public Result<int> Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnDeleteExecuting(entity, parameters);

            if (!parameters.OperationResult.HasError)
            {
                DbEntityEntry<T> entry = null;
                EntityState originalState = EntityState.Detached;

                try
                {
                    entry = DbContext.Entry<T>(entity);
                    originalState = entry.State;
                    entry.State = EntityState.Deleted;

                    DbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    if (entry != null) //It equals null when entity is null as well.
                    {
                        entry.State = originalState;
                    }

                    var exception = new RepositoryException("EntityRepository<>.Delete()", "An error occurred while deleting entity.", e);
                    exception.Data.Add("TEntity", EntityType.FullName);
                    var log = Logger.Log(LogType.Error, Priority.High, "EntityRepository<>.Delete()", exception, LogModule.Repository);

                    parameters.OperationResult = new Result<int>(log.Data, exception);
                }
            }

            OnDeleteExecuted(entity, parameters);

            return parameters.OperationResult;
        }

        public async Task<Result<int>> DeleteAsync(object id)
        {
            if (id == null) throw new ArgumentNullException("id");

            T entity = await Tracking.FindAsync(id);
            return await this.DeleteAsync(entity);
        }

        public async Task<Result<int>> DeleteAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnDeleteExecuting(entity, parameters);
            await OnDeleteExecutingAsync(entity, parameters);

            DbEntityEntry<T> entry = null;
            EntityState originalState = EntityState.Detached;

            if (!parameters.OperationResult.HasError)
            {
                try
                {
                    entry = DbContext.Entry<T>(entity);
                    originalState = entry.State;
                    entry.State = EntityState.Deleted;

                    await DbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    if (entry != null) //It equals null when entity is null as well.
                    {
                        entry.State = originalState;
                    }

                    var exception = new RepositoryException("EntityRepository<>.DeleteAsync()", "An error occurred while deleting entity.", e);
                    exception.Data.Add("TEntity", EntityType.FullName);
                    var log = Logger.Log(LogType.Error, Priority.High, "EntityRepository<>.DeleteAsync()", exception, LogModule.Repository);

                    parameters.OperationResult = new Result<int>(log.Data, exception);
                }
            }

            OnDeleteExecuted(entity, parameters);
            await OnDeleteExecutedAsync(entity, parameters);

            return parameters.OperationResult;
        }

        public Result<int> Save()
        {
            Result<int> retVal = new Result<int>();

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                var exception = new RepositoryException("EntityRepository<>.Save()", "An error occurred while saving pending changes.", e);
                var log = Logger.Log(LogType.Error, Priority.High, "EntityRepository<>.Save()", exception, LogModule.Repository);

                retVal = new Result<int>(log.Data, exception);
            }

            return retVal;
        }

        public Result<int> Save(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            return this.Save(entity, ActionType.Create);
        }

        public Result<int> Save(T entity, ActionType actionType)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnSaveExecuting(entity, actionType, parameters);

            if (!parameters.OperationResult.HasError)
            {
                DbEntityEntry<T> entry = null;
                EntityState originalState = EntityState.Unchanged;

                try
                {
                    entry = DbContext.Entry<T>(entity);
                    originalState = entry.State;

                    if (actionType == ActionType.Create)
                    {
                        entry.State = EntityState.Added;
                    }
                    else if (actionType == ActionType.Update)
                    {
                        entry.State = EntityState.Modified;
                    }

                    BindTrackableProperties(entity, actionType);

                    DbContext.SaveChanges();

                    entry.State = EntityState.Unchanged;
                }
                catch (Exception e)
                {
                    entry.State = originalState;

                    var exception = new RepositoryException("EntityRepository<>.Save()", "An error occurred while saving entity.", e);
                    exception.Data.Add("TEntity", EntityType.FullName);
                    var log = Logger.Log(LogType.Error, Priority.High, "EntityRepository<>.Save()", exception, LogModule.Repository);

                    parameters.OperationResult = new Result<int>(log.Data, exception);
                }
            }

            OnSaveExecuted(entity, actionType, parameters);

            return parameters.OperationResult;
        }

        public async Task<Result<int>> SaveAsync()
        {
            Result<int> retVal = new Result<int>();

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var exception = new RepositoryException("EntityRepository<>.SaveAsync()", "An error occurred while saving pending changes.", e);
                var log = Logger.Log(LogType.Error, Priority.High, "EntityRepository<>.SaveAsync()", exception, LogModule.Repository);

                retVal = new Result<int>(log.Data, exception);
            }

            return retVal;
        }

        public async Task<Result<int>> SaveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            return await this.SaveAsync(entity, ActionType.Create);
        }

        public async Task<Result<int>> SaveAsync(T entity, ActionType actionType)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnSaveExecuting(entity, actionType, parameters);
            await OnSaveExecutingAsync(entity, actionType, parameters);

            DbEntityEntry<T> entry = null;
            EntityState originalState = EntityState.Unchanged;

            if (!parameters.OperationResult.HasError)
            {
                try
                {
                    entry = DbContext.Entry<T>(entity);
                    originalState = entry.State;

                    if (actionType == ActionType.Create)
                    {
                        entry.State = EntityState.Added;
                    }
                    else if (actionType == ActionType.Update)
                    {
                        entry.State = EntityState.Modified;
                    }

                    BindTrackableProperties(entity, actionType);

                    await DbContext.SaveChangesAsync();

                    entry.State = EntityState.Unchanged;
                }
                catch (Exception e)
                {
                    entry.State = originalState;

                    var exception = new RepositoryException("EntityRepository<>.SaveAsync()", "An error occurred while saving entity.", e);
                    exception.Data.Add("TEntity", EntityType.FullName);
                    var log = Logger.Log(LogType.Error, Priority.High, "EntityRepository<>.SaveAsync()", exception, LogModule.Repository);

                    parameters.OperationResult = new Result<int>(log.Data, exception);
                }
            }

            OnSaveExecuted(entity, actionType, parameters);
            await OnSaveExecutedAsync(entity, actionType, parameters);

            return parameters.OperationResult;
        }

        private void BindTrackableProperties(T entity, ActionType type)
        {
            if (!(Thread.CurrentPrincipal.Identity is Cruder.Core.Security.CruderIdentity))
            {
                throw new FrameworkException("EntityRepository<>.BindTrackableProperties()", "Identity in CurrentPrincipal must be typeof(CruderIdentity) to use EntityRepository functionalities.");
            }

            var identity = (Thread.CurrentPrincipal.Identity as Cruder.Core.Security.CruderIdentity);

            if (!identity.IsAuthenticated || !identity.UserId.HasValue)
            {
                throw new FrameworkException("EntityRepository<>.BindTrackableProperties()", "Identity must be authenticated to use EntityRepository functionalities.");
            }

            if (type == ActionType.Create && entity is ICreationTrackable)
            {
                ICreationTrackable creationTrackableEntity = (ICreationTrackable)entity;

                creationTrackableEntity.CreatedOn = DateTime.UtcNow;
                creationTrackableEntity.CreatedBy = identity.UserId.Value;
            }

            if (entity is IUpdateTrackable)
            {
                IUpdateTrackable updateTrackableEntity = (IUpdateTrackable)entity;

                updateTrackableEntity.UpdatedOn = DateTime.UtcNow;
                updateTrackableEntity.UpdatedBy = identity.UserId.Value;
            }
        }

        public virtual IQueryable<T> Query()
        {
            return this.Query(null);
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return this.Query(predicate, null);
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderby)
        {
            return this.Query(predicate, orderby, null);
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderby, QueryOptions options)
        {
            return base.Query(predicate, orderby, options);
        }

        protected virtual void OnSaveExecuting(T entity, ActionType actionType, ActionParameters parameters) { }
        protected virtual void OnSaveExecuted(T entity, ActionType actionType, ActionParameters parameters) { }
        protected virtual async Task OnSaveExecutingAsync(T entity, ActionType actionType, ActionParameters parameters) { }
        protected virtual async Task OnSaveExecutedAsync(T entity, ActionType actionType, ActionParameters parameters) { }

        protected virtual void OnDeleteExecuting(T entity, ActionParameters parameters) { }
        protected virtual void OnDeleteExecuted(T entity, ActionParameters parameters) { }
        protected virtual async Task OnDeleteExecutingAsync(T entity, ActionParameters parameters) { }
        protected virtual async Task OnDeleteExecutedAsync(T entity, ActionParameters parameters) { }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class ActionParameters
    {
        public Result<int> OperationResult { get; set; }
    }
}
