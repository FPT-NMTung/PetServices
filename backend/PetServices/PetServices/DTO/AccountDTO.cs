namespace PetServices.DTO
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Status { get; set; }
        public int? UserInfoId { get; set; }
        public int? PartnerInfoId { get; set; }
        public int? RoleId { get; set; }
        public int? Otpid { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
