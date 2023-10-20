using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class RoomCategory
    {
        public RoomCategory()
        {
            Rooms = new HashSet<Room>();
        }

        public int RoomCategoriesId { get; set; }
        public string? RoomCategoriesName { get; set; }
        public string? Desciptions { get; set; }
        public string? Picture { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
