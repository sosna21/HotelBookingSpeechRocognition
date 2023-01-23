using System;
using SWPSD_PROJEKT.UI.Models;
using Room = SWPSD_PROJEKT.DialogDriver.Model.Room;

namespace SWPSD_PROJEKT.UI.Stores;

public class ReservationDataStore
{
    private ReservationDates _currentReservationDates;
    private Facilities _currentFacilities;
    private Room _currentRoom;
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
    
    public Room CurrentRoom
    {
        get => _currentRoom;
        set
        {
            _currentRoom = value;
            CurrentRoomChanged?.Invoke();
        }
    }

    public event Action CurrentReservationDatesChanged;
    public event Action CurrentFacilitiesChanged;
    public event Action CurrentRoomChanged;
}