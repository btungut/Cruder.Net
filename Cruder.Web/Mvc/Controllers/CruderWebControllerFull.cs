using Cruder.Core;
using Cruder.Core.Contract;
using Cruder.Core.ExceptionHandling;
using Cruder.Core.Module;
using Cruder.Resource;
using Cruder.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Cruder.Core.Repository;
using Cruder.Web.Model;
using System.Data.Entity.Validation;

namespace Cruder.Web.Mvc.Controllers
{
    public abstract class CruderWebController<T, TListViewModel, TDetailViewModel> : RepositoryController<T>
        where T : IEntity
        where TListViewModel : ListViewModel<T>
        where TDetailViewModel : DetailViewModel<T>
    {
        private readonly ICruderRepository<T> repository = null;

        public bool IsValidationEnabled { get; protected set; }

        protected override ICruderRepository<T> Repository
        {
            get { return this.repository; }
        }

        public CruderWebController()
            : this(IoC.Resolve<ICruderRepository<T>>())
        {
        }

        public CruderWebController(ICruderRepository<T> repository)
        {
            this.repository = repository;
            this.IsValidationEnabled = true;
        }

        protected abstract TListViewModel GetListViewModelInstance();

        protected abstract TDetailViewModel GetDetailViewModelInstance();

        public async virtual Task<ActionResult> Index()
        {
            TListViewModel listViewModel = GetListViewModelInstance();

            OnListViewModelPreparing(listViewModel);
            await OnListViewModelPreparingAsync(listViewModel);

            int count = await Repository.Query(DefaultFilter, null, base.QueryOptions).CountAsync();

            listViewModel.Data = await GetCurrentEntitiesAsync();
            listViewModel.PageModel = new PageModel(base.PageNo, base.RecordPerPage, count);

            OnListViewModelPrepared(listViewModel);
            await OnListViewModelPreparedAsync(listViewModel);

            OnCruderViewsExecuting();
            await OnCruderViewsExecutingAsync();

            return View(listViewModel);
        }

        public async virtual Task<ActionResult> AddEdit()
        {
            TDetailViewModel detailViewModel = GetDetailViewModelInstance();

            OnDetailViewModelPreparing(detailViewModel);
            await OnDetailViewModelPreparingAsync(detailViewModel);

            T entity = await GetCurrentEntityAsync();

            if (entity == null)
            {
                throw new HttpException(404, ResourceManager.GetString("Common.RecordNotFound"));
            }

            detailViewModel.Data = entity;

            OnDetailViewModelPrepared(detailViewModel);
            await OnDetailViewModelPreparedAsync(detailViewModel);

            await OnCruderViewsExecutingAsync();
            OnCruderViewsExecuting();

            return View(detailViewModel);
        }


        public async virtual Task<ActionResult> Delete()
        {
            ActionParameters parameters = new ActionParameters
            {
                ContinuationActionResult = RedirectToReferrer(),
                OperationResult = new Result<int>()
            };

            T entity = await GetCurrentEntityAsync();

            OnDeleteExecuting(entity, parameters);
            await OnDeleteExecutingAsync(entity, parameters);

            if (!parameters.OperationResult.HasError)
            {
                parameters.OperationResult = await Repository.DeleteAsync(entity);

                if (!parameters.OperationResult.HasError)
                {
                    parameters.OperationResult.Message = ResourceManager.GetString("Web.Mvc.Controller.CruderWebController.DeletedSuccessfully");
                }
            }

            OnDeleteExecuted(entity, parameters);
            await OnDeleteExecutedAsync(entity, parameters);

            WriteMessage(parameters.OperationResult);

            return parameters.ContinuationActionResult;
        }

        [ActionName("AddEdit")]
        [AcceptVerbs(HttpVerbs.Post)]
        public async virtual Task<ActionResult> Save(TDetailViewModel model)
        {
            ActionParameters parameters = new ActionParameters
            {
                ContinuationActionResult = RedirectToAction("Index"),
                OperationResult = new Result<int>()
            };

            T entity = await GetCurrentEntityAsync();

            if (CheckValidationStatus())
            {
                parameters.OperationResult = PrepareEntity(entity);

                if (!parameters.OperationResult.HasError)
                {
                    OnSaveExecuting(entity, model, parameters);
                    await OnSaveExecutingAsync(entity, model, parameters);

                    if (!parameters.OperationResult.HasError)
                    {
                        if (CheckValidationStatus())
                        {
                            parameters.OperationResult = await Repository.SaveAsync(entity, CurrentActionType);

                            if (!parameters.OperationResult.HasError)
                            {
                                if (CurrentActionType == Cruder.Core.ActionType.Create)
                                {
                                    parameters.OperationResult.Message = ResourceManager.GetString("Web.Mvc.Controller.CruderWebController.CreatedSuccessfully");
                                }
                                else if (CurrentActionType == Cruder.Core.ActionType.Update)
                                {
                                    parameters.OperationResult.Message = ResourceManager.GetString("Web.Mvc.Controller.CruderWebController.UpdatedSuccessfully");
                                }
                            }
                        }
                        else
                        {
                            BindModelStateErrors(parameters);
                        }
                    }
                }
            }
            else
            {
                BindModelStateErrors(parameters);
            }

            if (parameters.OperationResult.HasError)
            {
                if (parameters.OperationResult.Exception != null)
                {
                    if (parameters.OperationResult.Exception.InnerException != null &&
                        parameters.OperationResult.Exception.InnerException is DbEntityValidationException)
                    {
                        DbEntityValidationException validationException = (DbEntityValidationException)parameters.OperationResult.Exception.InnerException;

                        IEnumerable<string> errorMessages = validationException.EntityValidationErrors.SelectMany(q => q.ValidationErrors.Select(x => x.ErrorMessage));
                        StringBuilder builder = new StringBuilder();

                        errorMessages.ToList().ForEach(error =>
                        {
                            builder.AppendFormat("</br>- {0}", error);
                        });

                        if (!string.IsNullOrEmpty(parameters.OperationResult.Message))
                        {
                            parameters.OperationResult.Message = string.Format("{0} <br /><br />{1}", parameters.OperationResult.Message, builder.ToString());
                        }
                        else
                        {
                            parameters.OperationResult.Message = builder.ToString();
                        }
                    }
                }

                parameters.ContinuationActionResult = View(model);
            }

            WriteMessage(parameters.OperationResult);
            
            await OnSaveExecutedAsync(entity, model, parameters);
            OnSaveExecuted(entity, model, parameters);

            await OnCruderViewsExecutingAsync();
            OnCruderViewsExecuting();

            return parameters.ContinuationActionResult;
        }

        private bool CheckValidationStatus()
        {
            return (IsValidationEnabled && ModelState.IsValid) || (!IsValidationEnabled);
        }

        private void BindModelStateErrors(ActionParameters parameters)
        {
            IEnumerable<ModelState> modelStates = ModelState.Values.Where(state => state.Errors.Count > 0);
            IEnumerable<ModelError> modelErrors = modelStates.SelectMany(q => q.Errors);

            if (modelErrors.Any())
            {
                StringBuilder builder = new StringBuilder();

                modelErrors.ToList().ForEach(error =>
                {
                    builder.AppendFormat("</br>- {0} {1}", error.ErrorMessage, error.Exception != null ? error.Exception.Message : null);
                });

                parameters.OperationResult.Message = builder.ToString();
                parameters.OperationResult.HasError = true;
            }
        }

        private T currentEntity;
        protected async Task<T> GetCurrentEntityAsync()
        {
            if (currentEntity == null)
            {
                if (CurrentActionType == Cruder.Core.ActionType.Create)
                {
                    currentEntity = Activator.CreateInstance<T>();
                }
                else if (
                    CurrentActionType == Cruder.Core.ActionType.Delete ||
                    CurrentActionType == Cruder.Core.ActionType.Update)
                {
                    QueryCriterias criterias = new QueryCriterias();
                    criterias.Add(DefaultIdentificationFieldName, CurrentEntityId.ToString(), CriteriaOptionEnum.Equals);

                    currentEntity = await repository.Query(null, null, new Cruder.Core.Repository.QueryOptions(null, criterias))
                        .SingleOrDefaultAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    throw new RepositoryException("CruderWebController.GetCurrentEntityAsync()", "Unable to get current entity. Given ActionType isn't suitable for CurrentEntity.");
                }
            }

            return currentEntity;
        }

        private List<T> currentEntities;
        protected async Task<List<T>> GetCurrentEntitiesAsync()
        {
            if (currentEntities == null)
            {
                if (CurrentActionType == Cruder.Core.ActionType.Retrieve)
                {
                    currentEntities = await Repository.Query(DefaultFilter, DefaultSorter, QueryOptions)
                        .Skip((PageNo - 1) * RecordPerPage).Take(RecordPerPage)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    throw new RepositoryException("CruderWebController.GetCurrentEntities()", "Entities couldn't be retrieved. Given ActionType was not suitable for this action.");
                }
            }

            return currentEntities;
        }

        private Result<int> PrepareEntity(T target)
        {
            Result<int> retVal = new Result<int>();

            Dictionary<string, string> forms = new Dictionary<string, string>();

            //Following three variables were defined for logging. These fills in loop and uses in catch.
            string lastPropertyType = "";
            string lastKey = "";
            string lastValue = "";

            try
            {
                foreach (string item in Request.Form.AllKeys.Where(q => q.StartsWith("Data.")))
                {
                    forms.Add(item.Replace("Data.", string.Empty), Request.Form.Get(item));
                }

                foreach (var key in forms.Keys)
                {
                    Type propertyType = EntityType.GetProperty(key).PropertyType;

                    lastPropertyType = propertyType.ToString();
                    lastKey = key;
                    lastValue = forms[key];

                    if (!propertyType.Name.ToLower().Contains("bool"))
                    {
                        var convertingType = TypeDescriptor.GetConverter(propertyType).ConvertFrom(null, Thread.CurrentThread.CurrentCulture, forms[key]);
                        EntityType.GetProperty(key).SetValue(target, convertingType, null);
                    }
                    else if (forms[key].Contains("false") || forms[key].Contains("true"))
                    {
                        bool value = (forms[key].Equals("false")) ? false : true;
                        EntityType.GetProperty(key).SetValue(target, value, null);
                    }
                }
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("CruderWebController<>.PrepareEntity()", "An error has occured while preparing entity for save/update.", e);
                exception.Data.Add("lastPropertyType", lastPropertyType);
                exception.Data.Add("lastKey", lastKey);
                exception.Data.Add("lastValue", lastValue);

                var log = Logger.Log(LogType.Error, Priority.High, "ControllerHelper.PrepareEntity()", exception, LogModule.Framework);
                retVal = new Result<int>(log.Data, exception);
            }

            return retVal;
        }

        protected virtual void OnListViewModelPreparing(TListViewModel listViewModel) { }
        protected virtual void OnListViewModelPrepared(TListViewModel listViewModel) { }
        protected async virtual Task OnListViewModelPreparingAsync(TListViewModel listViewModel) { }
        protected async virtual Task OnListViewModelPreparedAsync(TListViewModel listViewModel) { }

        protected virtual void OnDetailViewModelPreparing(TDetailViewModel detailViewModel) { }
        protected virtual void OnDetailViewModelPrepared(TDetailViewModel detailViewModel) { }
        protected async virtual Task OnDetailViewModelPreparingAsync(TDetailViewModel detailViewModel) { }
        protected async virtual Task OnDetailViewModelPreparedAsync(TDetailViewModel detailViewModel) { }

        protected virtual void OnDeleteExecuted(T entity, ActionParameters parameters) { }
        protected virtual void OnDeleteExecuting(T entity, ActionParameters parameters) { }
        protected async virtual Task OnDeleteExecutedAsync(T entity, ActionParameters parameters) { }
        protected async virtual Task OnDeleteExecutingAsync(T entity, ActionParameters parameters) { }

        protected virtual void OnSaveExecuted(T entity, TDetailViewModel viewModel, ActionParameters parameters) { }
        protected virtual void OnSaveExecuting(T entity, TDetailViewModel viewModel, ActionParameters parameters) { }
        protected async virtual Task OnSaveExecutedAsync(T entity, TDetailViewModel viewModel, ActionParameters parameters) { }
        protected async virtual Task OnSaveExecutingAsync(T entity, TDetailViewModel viewModel, ActionParameters parameters) { }

        protected async virtual Task OnCruderViewsExecutingAsync(){}

        protected virtual void OnCruderViewsExecuting(){}
    }
}
