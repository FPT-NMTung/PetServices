namespace FEPetServices.Form
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? CardNumber { get; set; }
        public string? ImageCertificate { get; set; }
        public int OTP { get; set; }
    }
}
