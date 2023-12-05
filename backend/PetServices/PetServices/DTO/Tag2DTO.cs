namespace PetServices.DTO
{
    public class Tag2DTO
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }

        public virtual ICollection<BlogDTO> Blogs { get; set; }
    }
}
