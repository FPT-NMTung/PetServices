using PetServices.Models;

namespace PetServices.DTO
{
    public class BookingServicesDetailDTO
    {
        public int ServiceId { get; set; }
        public int OrderId { get; set; }
        public double? Price { get; set; }
        public double? Weight { get; set; }
        public double? PriceService { get; set; }
        public int? PetInfoId { get; set; }
        public int? PartnerInfoId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? FeedbackPartnerStatus { get; set; }
        public bool? FeedbackStatus { get; set; }
        public string? StatusOrderService { get; set; }

        public virtual PartnerInfoDTO? PartnerInfo { get; set; }
        public virtual PetInfoDTO? PetInfo { get; set; }
        public virtual ServiceDTO? Service { get; set; } = null!;
    }
}
