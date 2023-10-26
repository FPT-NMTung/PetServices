using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class OrderProductDetail
    {
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public int ProductId { get; set; }
        public int BookingId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
