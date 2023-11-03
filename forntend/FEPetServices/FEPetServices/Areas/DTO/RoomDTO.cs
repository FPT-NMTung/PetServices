namespace FEPetServices.Areas.DTO
{
    public class RoomDTO
    {
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public string? Desciptions { get; set; }
        public bool Status { get; set; }
        public string? Picture { get; set; }
        public double? Price { get; set; }
        public int? RoomCategoriesId { get; set; }
        public string? RoomCategoriesName { get; set; }
        public int? Slot { get; set; }
        public List<int>? ServiceIds { get; set; }

    }
}
