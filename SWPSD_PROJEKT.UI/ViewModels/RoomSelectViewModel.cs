using System.Windows.Controls;
using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Models;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class RoomSelectViewModel : ViewModelBase
{
    public Room SelectedRoom { get; set; }
    
    public ICommand NavigateRoomDescriptionCommand { get; }
    public ICommand SelectRoomCommand { get; }

    public RoomSelectViewModel(NavigationStore navigatorStore, RoomStore roomStore, ReservationDateStore reservationStore)
    {
        NavigateRoomDescriptionCommand=
            new NavigateCommand<RoomDescriptionViewModel>(navigatorStore, () => new RoomDescriptionViewModel(navigatorStore, roomStore, reservationStore));
        SelectRoomCommand = new SelectRoomCommand(roomStore, this);
    }
}