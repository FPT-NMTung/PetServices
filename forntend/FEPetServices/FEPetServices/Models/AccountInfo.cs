namespace FEPetServices.Models
{
    public class AccountInfo
    {
        public int AccountID { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Status { get; set; }
        public int? UserInfoId { get; set; }
        public int? PartnerInfoId { get; set; }
        public int? RoleId { get; set; }

    }
}
