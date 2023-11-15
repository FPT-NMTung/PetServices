namespace FEPetServices.Areas.DTO
{
    public class RoomSearchDTO
    {
        public string? roomcategory { get; set; }
        public string? pricefrom { get; set; }
        public string? priceto { get; set; }
        public string? sortby { get; set; }
        public string? roomname { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set;}

    }
}
