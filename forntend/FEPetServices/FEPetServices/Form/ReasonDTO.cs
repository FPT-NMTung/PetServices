using FEPetServices.Form.OrdersForm;

namespace FEPetServices.Form
{
    public class ReasonDTO
    {
        public int ReasonId { get; set; }
        public string? ReasonTitle { get; set; }
        public string? ReasonDescription { get; set; }

        public virtual OrderForm? Order { get; set; } = null!;

    }
}
