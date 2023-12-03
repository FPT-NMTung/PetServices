namespace PetServices.DTO
{
    public class ReasonOrderDTO
    {
        public int ReasonOrderId { get; set; }
        public string? ReasonTitle { get; set; }
        public string? ReasonDescription { get; set; }
        public int? OrderId { get; set; }
        public int? UserInfoId { get; set; }
    }
}
