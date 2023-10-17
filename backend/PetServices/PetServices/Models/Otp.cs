using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Otp
    {
        public Otp()
        {
            Accounts = new HashSet<Account>();
        }

        public int Otpid { get; set; }
        public string? Code { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
