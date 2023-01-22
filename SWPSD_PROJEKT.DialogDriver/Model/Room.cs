namespace SWPSD_PROJEKT.DialogDriver.Model;

public class Room
{
    public int RoomId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
}