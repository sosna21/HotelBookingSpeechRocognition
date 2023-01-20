using System.Data.Entity;
using SWPSD_PROJEKT.DialogDriver.Model;

namespace SWPSD_PROJEKT.DialogDriver
{
    public class HotelContext : DbContext
    {
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<FacilityOrder> FacilityOrders { get; set; }

        public HotelContext() : base("Server=localhost;Database=HotelDb;Trusted_Connection=True;Integrated Security=SSPI;")
        {
        }
    }
}