using System;
using SWPSD_PROJEKT.UI.Models;


namespace SWPSD_PROJEKT.UI.Stores;

public class RoomStore
{
    private Room _currentRoom;
    public Room CurrentRoom
    {
        get => _currentRoom;
        set
        {
            _currentRoom = value;
            CurrentAccountChanged?.Invoke();
        }
    }

    public event Action CurrentAccountChanged;
}