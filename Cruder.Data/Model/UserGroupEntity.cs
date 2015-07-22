using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cruder.Data.Model
{
    public class UserGroupEntity : BaseEntityModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        //Many to Many relationship on Routes
        public virtual List<UserGroupRouteMappingEntity> RouteMappings { get; set; }

        //One to Many relationship on Users
        public virtual List<UserEntity> Users { get; set; }

        public UserGroupEntity()
        {
            this.RouteMappings = new List<UserGroupRouteMappingEntity>();
            this.Users = new List<UserEntity>();
        }

        public List<RouteEntity> Routes
        {
            get
            {
                return this.RouteMappings.Select(mapping => mapping.Route).ToList();
            }
        }
    }
}
