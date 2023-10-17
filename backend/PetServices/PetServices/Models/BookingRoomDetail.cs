using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class BookingRoomDetail
    {
        public int RoomId { get; set; }
        public int BookingId { get; set; }

        public virtual Room Room { get; set; } = null!;
    }
}
