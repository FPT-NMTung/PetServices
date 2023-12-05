namespace PetServices.DTO
{
    public class BlogDTO
    {
        public int BlogId { get; set; }
        public string? Content { get; set; }
        public string? Heading { get; set; }
        public string? PageTile { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? PublisheDate { get; set; }
        public bool? Status { get; set; }
        public int? TagId { get; set; }
        public string? TagName { get; set; }
    }
}
