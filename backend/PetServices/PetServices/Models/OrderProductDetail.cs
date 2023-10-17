using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class OrderProductDetail
    {
        public int? Quanlity { get; set; }
        public double? Price { get; set; }
        public int ProductId { get; set; }
        public int BookingId { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
