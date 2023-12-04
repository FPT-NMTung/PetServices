namespace PetServices.DTO
{
    public class ReasonOrderDTO
    {
        public int ReasonOrderId { get; set; }
        public string? ReasonOrderTitle { get; set; }
        public string? ReasonOrderDescription { get; set; }
        public int? OrderId { get; set; }
        public string? EmailReject { get; set; }
        public DateTime? RejectTime { get; set; }
    }
}
