using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        public string? Content { get; set; }
        public string? Heading { get; set; }
        public string? PageTile { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? PublisheDate { get; set; }
    }
}
