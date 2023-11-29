using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProductDetails = new HashSet<OrderProductDetail>();
        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Desciption { get; set; }
        public string? Picture { get; set; }
        public bool? Status { get; set; }
        public double? Price { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? ProCategoriesId { get; set; }
        public int? Quantity { get; set; }
        public int? QuantitySold { get; set; }

        public virtual ProductCategory? ProCategories { get; set; }
        public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; }
    }
}
