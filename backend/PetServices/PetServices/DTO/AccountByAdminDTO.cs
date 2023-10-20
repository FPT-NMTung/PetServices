namespace PetServices.DTO
{
    public class AccountByAdminDTO
    {
        public int Stt { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } 
        public string? UserName { get; set; }
        public string? Address { get; set; }
        public int? RoleId { get; set; }
        public bool Status { get; set; }
    }
}
