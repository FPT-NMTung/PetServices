namespace PetServices.DTO
{
    public class RegisterPartnerDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }
        public string? Address { get; set; }
        public string? CardNumber { get; set; }
        public string ImageCertificate { get; set; }
    }
}
