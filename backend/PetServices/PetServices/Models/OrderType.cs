using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class OrderType
    {
        public int OrderTypeId { get; set; }
        public bool? OrderProduct { get; set; }
        public bool? BookingRoom { get; set; }
        public bool? BookingService { get; set; }
        public int? OrderId { get; set; }

        public virtual Order? Order { get; set; }
    }
}
