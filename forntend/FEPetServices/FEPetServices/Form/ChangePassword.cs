using System.ComponentModel.DataAnnotations;

namespace FEPetServices.Form
{
    public class ChangePassword
    {
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Required]
        public string? ConfirmNewPassword { get; set; }
    }
}
