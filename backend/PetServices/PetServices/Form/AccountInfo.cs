using PetServices.DTO;
using PetServices.Models;

namespace PetServices.Form
{
    public class AccountInfo
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Status { get; set; }
        public int? UserInfoId { get; set; }
        public int? PartnerInfoId { get; set; }
        public string RoleName { get; set; } 
        public virtual PartnerInfoDTO? PartnerInfo { get; set; }
        public virtual UserInfoDTO? UserInfo { get; set; }
    }
}
