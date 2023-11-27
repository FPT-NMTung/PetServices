using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class OrderProductDetail
    {
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public bool? FeedbackStatus { get; set; }
        public string? StatusOrderProduct { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
