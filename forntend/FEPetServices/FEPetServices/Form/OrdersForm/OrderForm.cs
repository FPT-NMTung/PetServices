using System.Drawing;

namespace FEPetServices.Form.OrdersForm
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
        public string? Phone { get; set; }
        public string? TypePay { get; set; }
        public string? FullName { get; set; }
        public int? UserInfoId { get; set; }
        public bool? StatusPayment { get; set; }
        public int? ReasonId { get; set; }  
        public double? TotalPrice { get; set; }
       // public int? ReasonId { get; set; }

        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetailForm> OrderProductDetails { get; set; }
        public virtual ICollection<BookingServicesDetailForm>? BookingServicesDetails { get; set; }
        public virtual ICollection<BookingRoomDetailForm>? BookingRoomDetails { get; set; }
        public virtual ICollection<BookingRoomServiceForm>? BookingRoomServices { get; set; }
        public virtual ICollection<ReasonOrdersForm>? ReasonOrders { get; set; }
    }
}
