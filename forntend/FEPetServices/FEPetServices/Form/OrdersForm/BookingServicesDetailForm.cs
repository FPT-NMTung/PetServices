using PetServices.Models;

namespace FEPetServices.Form.OrdersForm
{
    public class BookingServicesDetailForm
    {
        public int ServiceId { get; set; }
        public int OrderId { get; set; }
        public double? Price { get; set; }
        public double? Weight { get; set; }
        public double? PriceService { get; set; }
        public int? PartnerInfoId { get; set; }
        public virtual PartnerInfo? PartnerInfo { get; set; }
        public virtual ServiceDTO? Service { get; set; } = null!;
    }
}
