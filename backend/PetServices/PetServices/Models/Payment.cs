using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public double? Salary { get; set; }
        public DateTime? DateSalary { get; set; }
        public bool? StatusSalary { get; set; }
        public int? PartnerInfoId { get; set; }

        public virtual PartnerInfo? PartnerInfo { get; set; }
    }
}
