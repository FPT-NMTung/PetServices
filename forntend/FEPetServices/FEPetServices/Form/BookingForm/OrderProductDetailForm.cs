namespace FEPetServices.Form.BookingForm
{
    public class OrderProductDetailForm
    {
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public int ProductId { get; set; }
        public int BookingId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public virtual ProductDTO Product { get; set; } = null!;
    }
}
