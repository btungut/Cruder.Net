using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cruder.Data.Model
{
    [Table("UserGroupRouteMappings", Schema="Cruder")]
    public class UserGroupRouteMappingEntity
    {
        [Key]
        [Column(Order=1)]
        public int UserGroupId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int RouteId { get; set; }

        public virtual UserGroupEntity UserGroup { get; set; }
        public virtual RouteEntity Route { get; set; }
    }
}
