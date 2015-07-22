using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cruder.Data.Model
{
    public class RouteEntity : BaseEntityModel
    {
        [Required]
        [MaxLength(50)]
        public string Controller { get; set; }

        [MaxLength(50)]
        public string Action { get; set; }

        [MaxLength(10)]
        public string HttpMethod { get; set; }

        //Many to Many relationship on UserGroups
        public virtual List<UserGroupRouteMappingEntity> UserGroupMappings { get; set; }
    }
}
