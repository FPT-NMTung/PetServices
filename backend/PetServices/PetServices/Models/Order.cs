using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProductDetails = new HashSet<OrderProductDetail>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public int? UserInfoId { get; set; }

        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; }
    }
}
