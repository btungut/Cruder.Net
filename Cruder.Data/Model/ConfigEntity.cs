using System.ComponentModel.DataAnnotations;

namespace Cruder.Data.Model
{
    public class ConfigEntity : BaseEntityModel
    {
        [Required]
        [MaxLength(100)]
        public string Key { get; set; }

        [Required]
        public string DevelopmentValue { get; set; }

        [Required]
        public string ProductionValue { get; set; }
    }
}
