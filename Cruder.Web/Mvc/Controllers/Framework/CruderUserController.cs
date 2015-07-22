using Cruder.Data.Repository;
using System.Linq;

namespace Cruder.Web.Mvc.Controllers.Framework
{
    public class CruderUserController : CruderWebController<Cruder.Data.Model.UserEntity>
    {
        private readonly CruderUserGroupRepository userGroupRepository = null;

        public CruderUserController()
            : base(new CruderUserRepository())
        {
            userGroupRepository = new CruderUserGroupRepository();
        }

        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            if (CurrentActionType == Cruder.Core.ActionType.Create || CurrentActionType == Cruder.Core.ActionType.Retrieve || CurrentActionType == Cruder.Core.ActionType.Update)
            {
                ViewBag.UserGroups = userGroupRepository.Query().ToList();
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnSaveExecuting(Data.Model.UserEntity entity, ViewModel.DetailViewModel<Data.Model.UserEntity> viewModel, Model.ActionParameters parameters)
        {
            string password = Request.Form["External.Password"];
            string rePassword = Request.Form["External.RePassword"];

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(rePassword))
            {
                if (password == rePassword)
                {
                    entity.Password = Definition.Cryptology.Encrypt(password);
                }
                else
                {
                    parameters.OperationResult.HasError = true;
                    parameters.OperationResult.Message = string.Format(Cruder.Resource.ResourceManager.GetString("Common.FieldsNotMatch"), "Password", "Re-Password");
                }
            }
        }

        public override async System.Threading.Tasks.Task<System.Web.Mvc.ActionResult> Save(ViewModel.DetailViewModel<Data.Model.UserEntity> model)
        {
            if (ModelState["Data.Password"] != null)
            {
                ModelState["Data.Password"].Errors.Clear();
            }

            return await base.Save(model);
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
            userGroupRepository.Dispose();
        }
    }
}
