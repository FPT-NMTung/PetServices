using PetServices.Models;

namespace FEPetServices.Form.OrdersForm
{
    public class BookingServicesDetailForm
    {
        public int ServiceId { get; set; }
        public int OrderId { get; set; }
        public virtual ServiceDTO? Service { get; set; } = null!;
    }
}
