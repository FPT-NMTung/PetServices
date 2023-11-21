namespace PetServices.DTO
{
    public class PetInfoDTO
    {
        public int PetInfoId { get; set; }
        public string? PetName { get; set; }
        public string? ImagePet { get; set; }
        public string? Species { get; set; }
        public bool? Gender { get; set; }
        public string? Descriptions { get; set; }
        public int? UserInfoId { get; set; }
        public double? Weight { get; set; }
        public DateTime? Dob { get; set; }
    }
}
