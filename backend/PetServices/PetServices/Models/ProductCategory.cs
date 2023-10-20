using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public int ProCategoriesId { get; set; }
        public string? ProCategoriesName { get; set; }
        public string? Desciptions { get; set; }
        public string? Picture { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
