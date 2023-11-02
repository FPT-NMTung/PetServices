using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Booking
    {
        public Booking()
        {
            BookingRoomDetails = new HashSet<BookingRoomDetail>();
            BookingServicesDetails = new HashSet<BookingServicesDetail>();
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
        public virtual ICollection<BookingRoomDetail> BookingRoomDetails { get; set; }
        public virtual ICollection<BookingServicesDetail> BookingServicesDetails { get; set; }
    }
}
