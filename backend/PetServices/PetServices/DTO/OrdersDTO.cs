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
        public string? Phone { get; set; }
        public string? TypePay { get; set; }
        public string? FullName { get; set; }
        public int? UserInfoId { get; set; }
        public bool? StatusPayment { get; set; }
        public int? ReasonId { get; set; }
        public double? TotalPrice { get; set; }

        public virtual UserInfoDTO? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetailDTO>? OrderProductDetails { get; set; }
        public virtual ICollection<BookingRoomServiceDTO>? BookingRoomServices { get; set; }
        public virtual ICollection<BookingRoomDetailDTO>? BookingRoomDetails { get; set; }
        public virtual ICollection<BookingServicesDetailDTO>? BookingServicesDetails { get; set; }
        public virtual ICollection<OrderTypeDTO>? OrderTypes { get; set; }
        public virtual ICollection<ReasonOrderDTO>? ReasonOrders { get; set; }
    }
}
