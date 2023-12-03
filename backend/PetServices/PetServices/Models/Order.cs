using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Order
    {
        public Order()
        {
            BookingRoomDetails = new HashSet<BookingRoomDetail>();
            BookingRoomServices = new HashSet<BookingRoomService>();
            BookingServicesDetails = new HashSet<BookingServicesDetail>();
            OrderProductDetails = new HashSet<OrderProductDetail>();
            OrderTypes = new HashSet<OrderType>();
            ReasonOrders = new HashSet<ReasonOrder>();
        }

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
        public double? TotalPrice { get; set; }

        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<BookingRoomDetail> BookingRoomDetails { get; set; }
        public virtual ICollection<BookingRoomService> BookingRoomServices { get; set; }
        public virtual ICollection<BookingServicesDetail> BookingServicesDetails { get; set; }
        public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; }
        public virtual ICollection<OrderType> OrderTypes { get; set; }
        public virtual ICollection<ReasonOrder> ReasonOrders { get; set; }
    }
}
