using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class BookingRoomDetail
    {
        public int RoomId { get; set; }
        public int OrderId { get; set; }
        public double? Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Booking Order { get; set; } = null!;
        public virtual Order OrderNavigation { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }
}
