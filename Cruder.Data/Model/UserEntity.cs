using System.ComponentModel.DataAnnotations;

namespace Cruder.Data.Model
{
    public class UserEntity : BaseEntityModel
    {
        [Required]
        [MaxLength(50)]
        public string Fullname { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string Mail { get; set; }

        [Required]
        public bool IsSystemAdmin { get; set; }

        //One to Many relationship on UserGroup
        public int UserGroupId { get; set; }
        public virtual UserGroupEntity UserGroup { get; set; }
    }
}
