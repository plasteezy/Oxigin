using System.ComponentModel.DataAnnotations;

namespace Web.Admin.Models
{
    public class RoleMgt
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email addres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}