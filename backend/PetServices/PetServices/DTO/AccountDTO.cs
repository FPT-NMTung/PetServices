namespace PetServices.DTO
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Status { get; set; }
        public int? UserInfoId { get; set; }
        public int? PartnerInfo { get; set; }


    }
}
