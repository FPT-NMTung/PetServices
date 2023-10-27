using PetServices.Models;

namespace PetServices.DTO
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingStatus { get; set; }
        public int? UserInfoId { get; set; }
        public virtual UserInfoDTO? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetailDTO> OrderProductDetails { get; set; }
    }
}
