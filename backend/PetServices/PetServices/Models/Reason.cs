using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Reason
    {
        public int ReasonId { get; set; }
        public string? ReasonTitle { get; set; }
        public string? ReasonDescription { get; set; }
    }
}
