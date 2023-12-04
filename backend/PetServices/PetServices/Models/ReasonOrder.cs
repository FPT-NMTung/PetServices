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
        public int? UserInfoId { get; set; }

        public virtual Order? Order { get; set; }
        public virtual UserInfo? UserInfo { get; set; }
    }
}
