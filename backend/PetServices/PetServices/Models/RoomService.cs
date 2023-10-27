using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class RoomService
    {
        public int RoomServiceId { get; set; }
        public int ServiceId { get; set; }
        public int RoomId { get; set; }
        public bool? Status { get; set; }
    }
}
