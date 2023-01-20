namespace SWPSD_PROJEKT.DialogDriver.Model;

public class Room
{
    public int RoomId { get; set; }
    public int RoomTypeId { get; set; }
    public RoomType RoomType { get; set; }
    
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
}