using System.ComponentModel.DataAnnotations;

namespace Cruder.Data.Model
{
    public class LogEntity : BaseEntityModel
    {
        [Required]
        public int Type { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public string Description { get; set; }

        public string Details { get; set; }

        public string Request { get; set; }

        public string Module { get; set; }

        public string Notes { get; set; }
    }
}
