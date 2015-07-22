using Cruder.Core.Model;
using Cruder.Core.Security;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Cruder.Web.Security
{
    public class CruderPrincipal : IPrincipal
    {
        public string AuthenticationType { get; private set; }

        public UserModel User { get; private set; }

        public CruderPrincipal(string authenticationType, UserModel user)
        {
            this.AuthenticationType = authenticationType;
            this.User = user;
        }

        public static CruderPrincipal Current
        {
            get
            {
                CruderPrincipal retVal = null;

                if (Thread.CurrentPrincipal is CruderPrincipal)
                {
                    retVal = (CruderPrincipal)Thread.CurrentPrincipal;
                }

                return retVal;
            }
            set
            {
                Thread.CurrentPrincipal = value;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = value;
                }
            }
        }

        private IIdentity identity;
        public IIdentity Identity
        {
            get
            {
                if (identity == null)
                {
                    string identityName = this.User == null ? null : this.User.Username;
                    bool isAuthenticated = this.User != null;
                    int? userId = this.User == null ? default(int?) : this.User.Id;

                    identity = new CruderIdentity(identityName, this.AuthenticationType, isAuthenticated, userId);
                }

                return identity;
            }
            set
            {
                identity = value;
            }
        }

        public bool IsInRole(string role)
        {
            bool retVal = false;

            if (this.User != null)
            {
                if (!this.User.IsSystemAdmin)
                {
                    string[] split = role.Split('.');
                    string controller = split[0];
                    string action = split[1];
                    string httpMethod = split[2];

                    var userRoutes = this.User.UserGroup.Routes;

                    var check = userRoutes.Where(q => string.Equals(q.Controller, controller, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(q.Controller));

                    if (check.Count() > 0)
                    {
                        check = userRoutes.Where(q => string.Equals(q.Action, action, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(q.Action));

                        if (check.Count() > 0)
                        {
                            check = userRoutes.Where(q => string.Equals(q.HttpMethod, httpMethod, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(q.HttpMethod));

                            if (check.Count() > 0)
                            {
                                retVal = true;
                            }
                        }
                    }
                }
                else
                {
                    retVal = true;
                }
            }

            return retVal;
        }
    }
}
