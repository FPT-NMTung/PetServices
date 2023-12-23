namespace PetServices.DTO
{
    public class OrderPartnerDTO
    {
        public int Stt { get; set; }
        public int PartnerId { get; set; }
        public string? ImagePartner { get; set; }
        public string? NamePartner { get; set; }
        public string? TotalOrder { get; set; } 
        public double? TotalSalary { get; set; }
    }
}
