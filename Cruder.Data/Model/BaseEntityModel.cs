using Cruder.Core.Contract;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cruder.Data.Model
{
    public class BaseEntityModel : IEntity<int>, ICreationTrackable, IUpdateTrackable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime UpdatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }
    }
}
