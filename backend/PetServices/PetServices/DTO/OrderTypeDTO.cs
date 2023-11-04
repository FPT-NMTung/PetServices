namespace PetServices.DTO
{
    public class OrderTypeDTO
    {
        public int OrderTypeId { get; set; }
        public bool? OrderProduct { get; set; }
        public bool? BookingRoom { get; set; }
        public bool? BookingService { get; set; }
        public int? OrderId { get; set; }
    }
}
