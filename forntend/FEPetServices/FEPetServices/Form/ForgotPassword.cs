using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FEPetServices.Form
{
    public class ForgotPassword
    {
        [Required, EmailAddress, Display(Name = "Registered email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
        public string Otp { get; set; }

    }
}
