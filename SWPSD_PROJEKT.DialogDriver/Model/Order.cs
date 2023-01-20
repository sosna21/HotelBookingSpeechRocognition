using System;

namespace SWPSD_PROJEKT.DialogDriver.Model;

public class Order
{
    public int OrderId { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public Guest Guest { get; set; }
    public Room Room { get; set; }
    
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int NumberOfPeople { get; set; }
    public int Days { get; set; }
    public decimal TotalPrice { get; set; }
}