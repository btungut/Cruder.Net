using Cruder.Core.ExceptionHandling;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Cruder.Core.Repository
{
    public abstract class BaseRepository<TEntity, TKey> 
        where TEntity : Contract.IEntity<TKey>
    {
        public abstract IQueryable<TEntity> Queryable { get; }

        public virtual Expression<Func<TEntity, bool>> DefaultFilter { get; private set; }

        public virtual Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> DefaultSorter { get; private set; }

        public BaseRepository()
        {
            this.DefaultFilter = null;
            this.DefaultSorter = q => q.OrderBy(x => x.Id);
        }

        public Type EntityType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, QueryOptions options)
        {
            IQueryable<TEntity> retVal = this.Queryable;
            bool isOrdered = false;

            try
            {
                if (DefaultFilter != null)
                {
                    retVal = retVal.Where(DefaultFilter);
                }

                if (predicate != null)
                {
                    retVal = retVal.Where(predicate);
                }

                if (options != null)
                {
                    if (options.Criterias != null && options.Criterias.Count() > 0)
                    {
                        DynamicQueryParameters parameters = DynamicQueryParameters.Parse(options.Criterias, this.EntityType);

                        if (!string.IsNullOrWhiteSpace(parameters.Query))
                        {
                            retVal = retVal.Where(parameters.Query, parameters.Values);
                        }
                    }

                    if (options.Ordering != null && options.Ordering.Count() > 0)
                    {
                        isOrdered = true;
                        foreach (QueryOrderItem orderItem in options.Ordering)
                        {
                            retVal = retVal.OrderBy(orderItem.ToString(), new object[] { });
                        }
                    }
                }

                //Query string ile order edilmediyse buraya gir
                if (!isOrdered)
                {
                    //Parametre ile gelen order null değilse
                    if (orderBy != null)
                    {
                        retVal = orderBy(retVal);
                    }
                    else
                    {
                        //Ne querystring ile ne de parametre ile order olmadıysa ve DefaultSorter null değilse default u uygala
                        if (DefaultSorter != null)
                        {
                            retVal = DefaultSorter(retVal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var exception = new RepositoryException("BaseRepository<>.Query()", "An exception occurred while getting IQueryable by params.", e);

                throw exception;
            }

            return retVal;
        }

        public Result<int> CreateExistRecordResult(string value)
        {
            return CreateErrorResult(string.Format(Cruder.Resource.ResourceManager.GetString("Core.Repository.ExistRecordMessage"), value));
        }

        public Result<int> CreateErrorResult(string message)
        {
            return new Result<int>(0, message, true);
        }
    }
}
