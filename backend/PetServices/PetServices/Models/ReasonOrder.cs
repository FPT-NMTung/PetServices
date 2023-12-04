using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class ReasonOrder
    {
        public int ReasonOrderId { get; set; }
        public string? ReasonOrderTitle { get; set; }
        public string? ReasonOrderDescription { get; set; }
        public int? OrderId { get; set; }
        public string? EmailReject { get; set; }
        public DateTime? RejectTime { get; set; }

        public virtual Order? Order { get; set; }
    }
}
