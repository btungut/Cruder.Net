using System.ComponentModel.DataAnnotations;

namespace Cruder.Web.ViewModel
{
    public class LogonViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
