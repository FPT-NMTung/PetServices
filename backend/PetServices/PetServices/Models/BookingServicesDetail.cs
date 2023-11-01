using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class BookingServicesDetail
    {
        public int ServiceId { get; set; }
        public int OrderId { get; set; }

        public virtual Booking Order { get; set; } = null!;
        public virtual Order OrderNavigation { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}
