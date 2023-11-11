using PetServices.Models;

namespace PetServices.DTO
{
    public class ServiceDTO
    {
        public ServiceDTO() { }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Desciptions { get; set; }
        public bool? Status { get; set; }
        public string Picture { get; set; }
        public double? Time { get; set; }
        public double Price { get; set; }
        public int? SerCategoriesId { get; set; }
        public string? SerCategoriesName { get; set; }
    }
}
