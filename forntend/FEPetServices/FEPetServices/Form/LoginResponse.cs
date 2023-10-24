namespace FEPetServices.Form
{
    public class LoginResponse
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string RoleName { get; set; }
        public string Token { get; set; }
    }
}
