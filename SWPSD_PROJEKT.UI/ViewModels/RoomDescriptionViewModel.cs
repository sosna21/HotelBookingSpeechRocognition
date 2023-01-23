using System.Windows.Controls;
using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class RoomDescriptionViewModel: ViewModelBase
{
    private RoomStore _roomStore;
    
    //Used by binding
    public string RoomName => _roomStore.CurrentRoom.RoomName;
    //Used by binding
    public Image RoomImage => _roomStore.CurrentRoom.RoomImg;

    public ReservationDataStore ReservationStore { get; set; }
    public ICommand NavigateRoomSelectCommand { get; }
    public ICommand NavigateReservationDateSelectCommand { get; }

    public RoomDescriptionViewModel(NavigationStore navigatorStore, RoomStore roomStore, ReservationDataStore reservationStore)
    {
        _roomStore = roomStore;
        ReservationStore = reservationStore;
        NavigateRoomSelectCommand =
            new NavigateCommand<RoomSelectViewModel>(navigatorStore, () => new RoomSelectViewModel(navigatorStore, roomStore, reservationStore));
        NavigateReservationDateSelectCommand=
            new NavigateCommand<ReservationDateSelectViewModel>(navigatorStore, () => new ReservationDateSelectViewModel(navigatorStore, roomStore, reservationStore));
    }
}