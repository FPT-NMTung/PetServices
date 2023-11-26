using PetServices.Models;

namespace PetServices.DTO
{
    public class ReasonDTO
    {
        public int ReasonId { get; set; }
        public string? ReasonTitle { get; set; }
        public string? ReasonDescription { get; set; }

        public virtual ICollection<OrdersDTO> Orders { get; set; }
    }
}
