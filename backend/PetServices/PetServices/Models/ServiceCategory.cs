using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class ServiceCategory
    {
        public ServiceCategory()
        {
            Services = new HashSet<Service>();
        }

        public int SerCategoriesId { get; set; }
        public string? SerCategoriesName { get; set; }
        public string? Desciptions { get; set; }
        public string? Picture { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}
