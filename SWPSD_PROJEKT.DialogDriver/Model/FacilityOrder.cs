namespace SWPSD_PROJEKT.DialogDriver.Model;

public class FacilityOrder
{
    public int FacilityOrderId { get; set; }
    public int OrderId { get; set; }
    public int FacilityId { get; set; }
    public Order Order { get; set; }
    public Facility Facility { get; set; }
    public int Quantity { get; set; }
}