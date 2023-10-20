
namespace PetServices.Models
{
    public class ServiceCategoryDTO
    {
        public ServiceCategoryDTO() { }
        public int SerCategoriesId { get; set; }
        public string? SerCategoriesName { get; set; }
        public string? Desciptions { get; set; }
        public string? Picture { get; set; }
        public bool? Status { get; set; }
    }
}
