using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Account
    {
        public Account()
        {
            Bookings = new HashSet<Booking>();
        }

        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Status { get; set; }
        public int? UserInfoId { get; set; }
        public int? PartnerInfoId { get; set; }
        public int? RoleId { get; set; }
        public int? Otpid { get; set; }

        public virtual Otp? Otp { get; set; }
        public virtual PartnerInfo? PartnerInfo { get; set; }
        public virtual Role? Role { get; set; }
        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
