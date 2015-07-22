using System.Security.Principal;

namespace Cruder.Core.Security
{
    public class CruderIdentity : IIdentity
    {
        private string authenticationType;
        public string AuthenticationType
        {
            get { return authenticationType; }
        }

        private bool isAuthenticated;
        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private int? userId;
        public int? UserId
        {
            get { return userId; }
        }

        public CruderIdentity(string name, string authenticationType, bool isAuthenticated, int? userId)
        {
            this.name = name;
            this.isAuthenticated = isAuthenticated;
            this.authenticationType = authenticationType;
            this.userId = userId;
        }
    }
}
