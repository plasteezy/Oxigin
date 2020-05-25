using System.ComponentModel.DataAnnotations;

namespace Web.Admin.Models
{
    public class CheckRole
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email addres")]
        public string Email { get; set; }
    }
}