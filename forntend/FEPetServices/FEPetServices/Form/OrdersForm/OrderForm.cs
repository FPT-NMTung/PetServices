using FEPetServices.Form.OrdersForm;

namespace FEPetServices.Form.BookingForm
{
    public class OrderForm
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public int? UserInfoId { get; set; }
        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetailForm> OrderProductDetails { get; set; }
        public virtual ICollection<BookingServicesDetailForm>? BookingServicesDetails { get; set; }
        public virtual ICollection<BookingRoomDetailForm>? BookingRoomDetails { get; set; }
    }
}
