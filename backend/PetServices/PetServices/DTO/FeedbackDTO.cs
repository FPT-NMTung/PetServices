namespace PetServices.DTO
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public string? Content { get; set; }
        public int? NumberStart { get; set; }
        public int? ServiceId { get; set; }
        public int? RoomId { get; set; }
        public int? PartnerId { get; set; }
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserImage { get; set; }
        public int? OrderId { get; set; }
    }
}
