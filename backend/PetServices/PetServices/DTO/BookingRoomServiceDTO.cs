using PetServices.Models;

namespace PetServices.DTO
{
    public class BookingRoomServiceDTO
    {
        public int OrderId { get; set; }
        public int RoomId { get; set; }
        public int ServiceId { get; set; }
        public double? PriceService { get; set; }
        public bool? FeedbackStatus { get; set; }
        public virtual RoomDTO? Room { get; set; } = null!;
        public virtual ServiceDTO? Service { get; set; } = null!;

    }
}
