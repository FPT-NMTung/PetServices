using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class RoomService
    {
        public RoomService()
        {
            Rooms = new HashSet<Room>();
        }

        public int RoomServicesId { get; set; }
        public string? RoomServiceName { get; set; }
        public double? Price { get; set; }
        public string? Picture { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
