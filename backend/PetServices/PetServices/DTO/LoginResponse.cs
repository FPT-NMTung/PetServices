namespace PetServices.DTO
{
    public class LoginResponse
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public bool Status { get; set; }
        public string RoleName { get; set; } 
        public string Token { get; set; }
    }
}
