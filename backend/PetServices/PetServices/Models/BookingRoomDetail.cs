using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class BookingRoomDetail
    {
        public int RoomId { get; set; }
        public int OrderId { get; set; }
        public double? Price { get; set; }
        public string? Note { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? FeedbackStatus { get; set; }
        public double? TotalPrice { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }
}
