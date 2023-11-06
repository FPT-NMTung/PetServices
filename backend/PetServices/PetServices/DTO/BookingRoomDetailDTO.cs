using PetServices.Models;

namespace PetServices.DTO
{
    public class BookingRoomDetailDTO
    {
        public int RoomId { get; set; }
        public int OrderId { get; set; }
        public double? Price { get; set; }
        public virtual RoomDTO? Room { get; set; } = null!;
    }
}
