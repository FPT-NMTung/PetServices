using FEPetServices.Areas.DTO;
using PetServices.Models;

namespace FEPetServices.Form.OrdersForm
{
    public class BookingRoomServiceForm
    {
        public int OrderId { get; set; }
        public int RoomId { get; set; }
        public int ServiceId { get; set; }
        public double? PriceService { get; set; }
        public virtual OrderForm? Order { get; set; } = null!;
        public virtual RoomDTO? Room { get; set; } = null!;
        public virtual ServiceDTO? Service { get; set; } = null!;
    }
}
