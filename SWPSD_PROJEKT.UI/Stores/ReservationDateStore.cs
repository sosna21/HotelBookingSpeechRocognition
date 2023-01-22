using System;
using SWPSD_PROJEKT.UI.Models;

namespace SWPSD_PROJEKT.UI.Stores;

public class ReservationDateStore
{
    private ReservationDates _currentReservationDates;
    private Facilities _currentFacilities;
    public ReservationDates CurrentReservationDates
    {
        get => _currentReservationDates;
        set
        {
            _currentReservationDates = value;
            CurrentReservationDatesChanged?.Invoke();
        }
    }
    public Facilities CurrentFacilities
    {
        get => _currentFacilities;
        set
        {
            _currentFacilities = value;
            CurrentFacilitiesChanged?.Invoke();
        }
    }

    public event Action CurrentReservationDatesChanged;
    public event Action CurrentFacilitiesChanged;
}