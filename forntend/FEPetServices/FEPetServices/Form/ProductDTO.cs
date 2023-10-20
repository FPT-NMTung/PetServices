namespace FEPetServices.Form
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Desciption { get; set; }
        public string? Prictue { get; set; }
        public bool? Status { get; set; }
        public double? Price { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? ProCategoriesId { get; set; }
        public string? ProCategoriesName { get; set; }
    }
}
