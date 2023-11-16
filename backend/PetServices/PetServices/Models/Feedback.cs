using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public string? Content { get; set; }
        public int? NumberStart { get; set; }
        public int? ServiceId { get; set; }
        public int? RoomId { get; set; }
        public int? PartnerId { get; set; }
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
    }
}
