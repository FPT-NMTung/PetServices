using PetServices.Models;

namespace PetServices.DTO
{
    public class OrderProductDetailDTO
    {
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public bool? FeedbackStatus { get; set; }
        public string? StatusOrderProduct { get; set; }
        public virtual ProductDTO? Product { get; set; } = null!;
    }
}
