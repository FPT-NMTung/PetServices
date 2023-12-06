using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Tag
    {
        public Tag()
        {
            Blogs = new HashSet<Blog>();
        }

        public int TagId { get; set; }
        public string? TagName { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
