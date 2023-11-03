namespace FEPetServices.Areas.DTO
{
    public class RoomSearchDTO
    {
        public int? page { get; set; }
        public int? pagesize { get; set; }
        public string? roomcategory { get; set; }
        public string? pricefrom { get; set; }
        public string? priceto { get; set; }
        public string? sortby { get; set; }
        public string? roomname { get; set; }
        public string? viewstyle { get; set; }

    }
}
