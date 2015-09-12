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
    public abstract class EntityRepository<TEntity, TKey> : BaseRepository<TEntity, TKey>, ICruderRepository<TEntity>, IDisposable
        where TEntity : class, IEntity<TKey>
    {
        protected DbContext DbContext { get; private set; }
        protected DbSet<TEntity> Tracking { get; private set; }

        public EntityRepository()
            : this(IoC.Resolve<DbContext>())
        {
        }

        public EntityRepository(DbContext dbContext)
        {
            this.DbContext = dbContext;
            this.Tracking = DbContext.Set<TEntity>();
        }

        public override IQueryable<TEntity> Queryable
        {
            get { return Tracking; }
        }

        public Result<int> Delete(object id)
        {
            if (id == null) throw new ArgumentNullException("id");

            TEntity entity = Tracking.Find(id);
            return this.Delete(entity);
        }

        public Result<int> Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnDeleteExecuting(entity, parameters);

            if (!parameters.OperationResult.HasError)
            {
                DbEntityEntry<TEntity> entry = null;
                EntityState originalState = EntityState.Detached;

                try
                {
                    entry = DbContext.Entry<TEntity>(entity);
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

            TEntity entity = await Tracking.FindAsync(id);
            return await this.DeleteAsync(entity);
        }

        public async Task<Result<int>> DeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnDeleteExecuting(entity, parameters);
            await OnDeleteExecutingAsync(entity, parameters);

            DbEntityEntry<TEntity> entry = null;
            EntityState originalState = EntityState.Detached;

            if (!parameters.OperationResult.HasError)
            {
                try
                {
                    entry = DbContext.Entry<TEntity>(entity);
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

        public Result<int> Save(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            return this.Save(entity, ActionType.Create);
        }

        public Result<int> Save(TEntity entity, ActionType actionType)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnSaveExecuting(entity, actionType, parameters);

            if (!parameters.OperationResult.HasError)
            {
                DbEntityEntry<TEntity> entry = null;
                EntityState originalState = EntityState.Unchanged;

                try
                {
                    entry = DbContext.Entry<TEntity>(entity);
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

        public async Task<Result<int>> SaveAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            return await this.SaveAsync(entity, ActionType.Create);
        }

        public async Task<Result<int>> SaveAsync(TEntity entity, ActionType actionType)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            ActionParameters parameters = new ActionParameters
            {
                OperationResult = new Result<int>()
            };

            OnSaveExecuting(entity, actionType, parameters);
            await OnSaveExecutingAsync(entity, actionType, parameters);

            DbEntityEntry<TEntity> entry = null;
            EntityState originalState = EntityState.Unchanged;

            if (!parameters.OperationResult.HasError)
            {
                try
                {
                    entry = DbContext.Entry<TEntity>(entity);
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

        private void BindTrackableProperties(TEntity entity, ActionType type)
        {
            if (entity is IUpdateTrackable || entity is ICreationTrackable)
            {
                if (!(Thread.CurrentPrincipal.Identity is Cruder.Core.Security.CruderIdentity))
                {
                    throw new FrameworkException(
                        "EntityRepository<>.BindTrackableProperties()", 
                        string.Format("Identity in CurrentPrincipal must be typeof(CruderIdentity) to use EntityRepository functionalities cause '{0}' has trackable properties.", EntityType.Name));
                }

                var identity = (Thread.CurrentPrincipal.Identity as Cruder.Core.Security.CruderIdentity);

                if (!identity.IsAuthenticated || !identity.UserId.HasValue)
                {
                    throw new FrameworkException(
                        "EntityRepository<>.BindTrackableProperties()", 
                        string.Format("Identity must be authenticated to use EntityRepository functionalities cause '{0}' has trackable properties.", EntityType.Name));
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
        }

        public virtual IQueryable<TEntity> Query()
        {
            return this.Query(null);
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
        {
            return this.Query(predicate, null);
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby)
        {
            return this.Query(predicate, orderby, null);
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby, QueryOptions options)
        {
            return base.Query(predicate, orderby, options);
        }

        protected virtual void OnSaveExecuting(TEntity entity, ActionType actionType, ActionParameters parameters) { }
        protected virtual void OnSaveExecuted(TEntity entity, ActionType actionType, ActionParameters parameters) { }
        protected virtual async Task OnSaveExecutingAsync(TEntity entity, ActionType actionType, ActionParameters parameters) { }
        protected virtual async Task OnSaveExecutedAsync(TEntity entity, ActionType actionType, ActionParameters parameters) { }

        protected virtual void OnDeleteExecuting(TEntity entity, ActionParameters parameters) { }
        protected virtual void OnDeleteExecuted(TEntity entity, ActionParameters parameters) { }
        protected virtual async Task OnDeleteExecutingAsync(TEntity entity, ActionParameters parameters) { }
        protected virtual async Task OnDeleteExecutedAsync(TEntity entity, ActionParameters parameters) { }

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
