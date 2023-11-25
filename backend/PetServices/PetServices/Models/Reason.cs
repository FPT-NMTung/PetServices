using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Reason
    {
        public Reason()
        {
            Orders = new HashSet<Order>();
        }

        public int ReasonId { get; set; }
        public string? ReasonTitle { get; set; }
        public string? ReasonDescription { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
