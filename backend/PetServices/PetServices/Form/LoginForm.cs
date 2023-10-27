using System.ComponentModel.DataAnnotations;

namespace PetServices.Form
{
    public class LoginForm
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
