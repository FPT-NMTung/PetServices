using PetServices.Models;

namespace PetServices.DTO
{
    public class BookingServicesDetailDTO
    {
        public int ServiceId { get; set; }
        public int OrderId { get; set; }
        public virtual ServiceDTO Service { get; set; } = null!;
    }
}
