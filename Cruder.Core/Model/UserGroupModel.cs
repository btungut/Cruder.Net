using System.Collections.Generic;

namespace Cruder.Core.Model
{
    public class UserGroupModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<RouteModel> Routes { get; set; }
    }
}
