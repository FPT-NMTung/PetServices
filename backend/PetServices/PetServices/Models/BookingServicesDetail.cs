using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class BookingServicesDetail
    {
        public int ServiceId { get; set; }
        public int BookingId { get; set; }

        public virtual Service Service { get; set; } = null!;
    }
}
