using Cruder.Core;
using Cruder.Core.Contract;
using Cruder.Core.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Cruder.Web.Mvc.Controllers
{
    public abstract class RepositoryController<T> : BaseController where T : IEntity
    {
        protected virtual Expression<Func<T, bool>> DefaultFilter { get; private set; }
        protected virtual Func<IQueryable<T>, IOrderedQueryable<T>> DefaultSorter { get; private set; }
        protected abstract ICruderRepository<T> Repository { get; }

        public RepositoryController()
        {
            DefaultFilter = null;
            DefaultSorter = null;
            ViewData["DefaultPage"] = DefaultPageNo;
            ViewData["DefaultRecordPerPage"] = DefaultRecordPerPage;
        }

        private Type entityType = null;
        protected Type EntityType
        {
            get
            {
                if (entityType == null)
                {
                    entityType = typeof(T);
                }

                return entityType;
            }
        }

        protected ActionType CurrentActionType
        {
            get
            {
                ActionType retVal = ActionType.Unknown;

                if (base.RouteData.Values.ContainsKey("action"))
                {
                    string actionName = base.RouteData.Values["action"].ToString().ToLower(System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                    if (actionName == "addedit" || actionName == "save")
                    {
                        if (CurrentEntityId == null)
                        {
                            retVal = Cruder.Core.ActionType.Create;
                        }
                        else
                        {
                            retVal = Cruder.Core.ActionType.Update;
                        }
                    }
                    else if (actionName == "index")
                    {
                        retVal = Cruder.Core.ActionType.Retrieve;
                    }
                    else if (actionName == "delete")
                    {
                        retVal = Cruder.Core.ActionType.Delete;
                    }
                }

                return retVal;
            }
        }

        protected object CurrentEntityId
        {
            get
            {
                object retVal = null;

                if (RouteData.Values.ContainsKey(DefaultIdentificationFieldName))
                {
                    retVal = RouteData.Values[DefaultIdentificationFieldName];
                }
                else if (!string.IsNullOrEmpty(Request.QueryString[DefaultIdentificationFieldName]))
                {
                    retVal = Request.QueryString[DefaultIdentificationFieldName];
                }

                return retVal;
            }
        }

        private QueryOrdering queryOrdering = null;
        protected QueryOrdering QueryOrdering
        {
            get
            {
                if (queryOrdering == null)
                {
                    string orderBy = Request.QueryString.Get("OrderBy");
                    string orderTypeAsString = Request.QueryString.Get("OrderType");

                    if (!string.IsNullOrEmpty(orderBy))
                    {
                        OrderType orderType;

                        if (string.IsNullOrEmpty(orderTypeAsString) || orderTypeAsString.ToLowerInvariant() == "asc")
                        {
                            orderType = OrderType.Ascending;
                        }
                        else
                        {
                            orderType = OrderType.Descending;
                        }

                        queryOrdering = new QueryOrdering();
                        queryOrdering.Add(orderBy, orderType);
                    }
                }

                return queryOrdering;
            }
        }

        private QueryCriterias queryCriterias = null;
        protected QueryCriterias QueryCriterias
        {
            get
            {
                if (queryCriterias == null)
                {
                    queryCriterias = QueryCriterias.Parse(HttpUtility.UrlDecode(Request.QueryString.ToString()));

                    foreach (var item in queryCriterias)
                    {
                        ViewData["Search." + item.Key] = item.Value;
                        ViewData["Option." + item.Key] = item.Option;
                    }
                }

                return queryCriterias;
            }
        }

        private QueryOptions queryOptions = null;
        public QueryOptions QueryOptions
        {
            get
            {
                if (queryOptions == null)
                {
                    queryOptions = new QueryOptions(QueryOrdering, QueryCriterias);
                }

                return queryOptions;
            }
        }

        protected int RecordPerPage
        {
            get
            {
                int retVal = DefaultRecordPerPage;

                if (!string.IsNullOrEmpty(Request.QueryString.Get("record")))
                {
                    retVal = Convert.ToInt32(Request.QueryString.Get("record"));
                }

                return retVal;
            }
        }

        protected int PageNo
        {
            get
            {
                int retVal = DefaultPageNo;

                if (!string.IsNullOrEmpty(Request.QueryString.Get("page")))
                {
                    retVal = Convert.ToInt32(Request.QueryString.Get("page"));
                }

                return retVal;
            }
        }

        protected virtual string DefaultIdentificationFieldName
        {
            get { return "Id"; }
        }

        protected virtual int DefaultPageNo
        {
            get { return 1; }
        }

        protected virtual int DefaultRecordPerPage
        {
            get { return 10; }
        }
    }
}
