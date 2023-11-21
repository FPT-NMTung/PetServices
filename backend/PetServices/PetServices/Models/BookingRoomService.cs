using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class BookingRoomService
    {
        public int OrderId { get; set; }
        public int RoomId { get; set; }
        public int ServiceId { get; set; }
        public double? PriceService { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}
