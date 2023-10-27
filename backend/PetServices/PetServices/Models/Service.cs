using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Service
    {
        public Service()
        {
            Bookings = new HashSet<Booking>();
            Rooms = new HashSet<Room>();
        }

        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Desciptions { get; set; }
        public bool? Status { get; set; }
        public string? Picture { get; set; }
        public double? Price { get; set; }
        public int? SerCategoriesId { get; set; }

        public virtual ServiceCategory? SerCategories { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
