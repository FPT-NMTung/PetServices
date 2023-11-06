using FEPetServices.Areas.DTO;

namespace FEPetServices.Form.OrdersForm
{
    public class BookingRoomDetailForm
    {
        public int RoomId { get; set; }
        public int OrderId { get; set; }
        public virtual RoomDTO? Room { get; set; } = null!;
    }
}
