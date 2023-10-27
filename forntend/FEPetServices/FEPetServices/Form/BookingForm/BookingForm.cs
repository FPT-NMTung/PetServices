namespace FEPetServices.Form.BookingForm
{
    public class BookingForm
    {
        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingStatus { get; set; }
        public int? UserInfoId { get; set; }
        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<OrderProductDetailForm> OrderProductDetails { get; set; }
    }
}
