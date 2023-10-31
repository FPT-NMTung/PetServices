using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Booking
    {
        public Booking()
        {
            Rooms = new HashSet<Room>();
            Services = new HashSet<Service>();
        }

        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingStatus { get; set; }
        public int? UserInfoId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }

        public virtual UserInfo? UserInfo { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
