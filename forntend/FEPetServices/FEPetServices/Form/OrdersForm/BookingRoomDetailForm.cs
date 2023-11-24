using FEPetServices.Areas.DTO;

namespace FEPetServices.Form.OrdersForm
{
    public class BookingRoomDetailForm
    {
        public int RoomId { get; set; }
        public int OrderId { get; set; }
        public double? Price { get; set; }
        public string? Note { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? FeedbackStatus { get; set; }
        public double? TotalPrice { get; set; }
        public virtual RoomDTO? Room { get; set; } = null!;
    }
}
