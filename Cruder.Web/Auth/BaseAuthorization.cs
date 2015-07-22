using Cruder.Core.Configuration;
using Cruder.Core.ExceptionHandling;
using Cruder.Data.Model;
using Cruder.Data.Repository;
using System;
using System.Linq;
using System.Web.Mvc;
using Cruder.Web.Security;
using Cruder.Core.Model;

namespace Cruder.Web.Auth
{
    public abstract class BaseAuthorization : AuthorizeAttribute
    {
        public abstract string AuthenticationType { get; }

        public static void RenewPrincipalIdentity()
        {
            CruderPrincipal.Current.Identity = null; //Identity will be created again on next Identity.get calling.
        }

        public static bool LoginIfDevelopmentEnvironment()
        {
            bool retVal = false;

            try
            {
                if (Definition.IsDevelopmentEnvironment && CruderPrincipal.Current.User == null)
                {
                    using(CruderUserRepository userRepository = new CruderUserRepository())
                    {
                        UserEntity userInstance = userRepository.Query(user => user.Id == ConfigurationFactory.Application.DevelopmentEnvironmentUserId).SingleOrDefault();

                        if (userInstance != null)
                        {
                            BindUser(userInstance);
                            retVal = true;
                        }
                        else
                        {
                            throw new FrameworkException("BaseAuthorization.LoginIfDevelopmentEnvironment()", "Development mode user has not been found.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("BaseAuthorization.LoginIfDevelopmentEnvironment()", "An error occurred while logging by development mode user.", e);
                throw exception;
            }

            return retVal;
        }

        public static bool Login(string username, string password)
        {
            bool retVal = false;

            try
            {
                using (CruderUserRepository userRepository = new CruderUserRepository())
                {
                    string encryptedPassword = Definition.Cryptology.Encrypt(password);
                    UserEntity userInstance = userRepository.Query(q => q.Username == username && q.Password == encryptedPassword).SingleOrDefault();

                    if (userInstance != null)
                    {
                        BindUser(userInstance);
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                var exception = new FrameworkException("BaseAuthorization.Login()", "An error occurred while logging on.", e);
                throw exception;
            }

            return retVal;
        }

        public static void Logout()
        {
            UserSessionManager.Unbind();
            RenewPrincipalIdentity();
        }

        public static bool IsAllowedAnonymousAccess(ControllerBase controller)
        {
            bool isAllowedAnonymous = controller.GetType().IsDefined(typeof(Cruder.Web.Auth.Attributes.AllowAnonymousAccess), true);

            return isAllowedAnonymous;
        }

        private static void BindUser(UserEntity userInstance)
        {
            if (userInstance != null)
            {
                UserModel userModel = new UserModel
                {
                    Id = userInstance.Id,
                    Fullname = userInstance.Fullname,
                    Username = userInstance.Username,
                    Password = userInstance.Password,
                    Mail = userInstance.Mail,
                    IsSystemAdmin = userInstance.IsSystemAdmin,

                    UserGroup = new UserGroupModel
                    {
                        Id = userInstance.UserGroup.Id,
                        Name = userInstance.UserGroup.Name,
                        Description = userInstance.UserGroup.Description,

                        Routes = userInstance.UserGroup.Routes.Select(q => new RouteModel
                        {
                            Action = q.Action,
                            Controller = q.Controller,
                            HttpMethod = q.HttpMethod
                        }).ToList()
                    }
                };

                UserSessionManager.Bind(userModel);
            }
        }
    }
}
