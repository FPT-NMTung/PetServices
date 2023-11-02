using PetServices.Models;

namespace PetServices.DTO
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingStatus { get; set; }
        public int? UserInfoId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public virtual UserInfoDTO? UserInfo { get; set; }
    }
}
