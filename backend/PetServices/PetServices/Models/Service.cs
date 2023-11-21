using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Service
    {
        public Service()
        {
            BookingRoomServices = new HashSet<BookingRoomService>();
            BookingServicesDetails = new HashSet<BookingServicesDetail>();
            Rooms = new HashSet<Room>();
        }

        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Desciptions { get; set; }
        public bool? Status { get; set; }
        public double? Time { get; set; }
        public string? Picture { get; set; }
        public double? Price { get; set; }
        public int? SerCategoriesId { get; set; }

        public virtual ServiceCategory? SerCategories { get; set; }
        public virtual ICollection<BookingRoomService> BookingRoomServices { get; set; }
        public virtual ICollection<BookingServicesDetail> BookingServicesDetails { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
