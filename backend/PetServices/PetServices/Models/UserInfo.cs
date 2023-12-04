using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class UserInfo
    {
        public UserInfo()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
            PetInfos = new HashSet<PetInfo>();
        }

        public int UserInfoId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public string? Descriptions { get; set; }
        public string? ImageUser { get; set; }
        public DateTime? Dob { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PetInfo> PetInfos { get; set; }
    }
}
