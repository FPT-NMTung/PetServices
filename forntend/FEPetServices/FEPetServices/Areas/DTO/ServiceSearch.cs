namespace FEPetServices.Areas.DTO
{
    public class ServiceSearch
    {
        public int? page { get; set; }
        public int? pagesize { get; set; }
        public string? sortby { get; set; }
        public string? servicename { get; set; }
        public string? sevicecategory { get; set; }
        public string? pricefrom { get; set; }
        public string? priceto { get; set; }
  
    }
}
