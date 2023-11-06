using PetServices.Models;

namespace PetServices.DTO
{
    public class OrdersDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public int? UserInfoId { get; set; }
        public int? PartnerInfoId { get; set; }

        public virtual PartnerInfoDTO? PartnerInfo { get; set; }
        public virtual UserInfoDTO? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetailDTO>? OrderProductDetails { get; set; }
        public virtual ICollection<BookingServicesDetailDTO>? BookingServicesDetails { get; set; }
        public virtual ICollection<BookingRoomDetailDTO>? BookingRoomDetails { get; set; }
        public virtual ICollection<OrderTypeDTO>? OrderTypes { get; set; }
    }
}
