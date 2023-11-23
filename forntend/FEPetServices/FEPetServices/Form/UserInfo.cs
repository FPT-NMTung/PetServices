using FEPetServices.Form.OrdersForm;

namespace FEPetServices.Form
{
    public class UserInfo
    {
        public int UserInfoId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public string? Descriptions { get; set; }
        public DateTime? Dob { get; set; }
        public string? ImageUser { get; set; }
        public virtual ICollection<PetInfo> PetInfos { get; set; }  
        public virtual ICollection<OrderForm> Orders { get; set; }
    }
}
