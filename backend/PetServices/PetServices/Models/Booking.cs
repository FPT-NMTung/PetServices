using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Booking
    {
        public Booking()
        {
            OrderProductDetails = new HashSet<OrderProductDetail>();
            Rooms = new HashSet<Room>();
            Services = new HashSet<Service>();
        }

        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingStatus { get; set; }
        public int? UserInfoId { get; set; }

        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
