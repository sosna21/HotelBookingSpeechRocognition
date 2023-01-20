using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class RoomDescriptionViewModel: ViewModelBase
{
    public ICommand NavigateRoomSelectCommand { get; }
    public ICommand NavigateReservationDateSelectCommand { get; }

    public RoomDescriptionViewModel(NavigationStore navigatorStore)
    {
        NavigateRoomSelectCommand =
            new NavigateCommand<RoomSelectViewModel>(navigatorStore, () => new RoomSelectViewModel(navigatorStore));
        NavigateReservationDateSelectCommand=
            new NavigateCommand<ReservationDateSelectViewModel>(navigatorStore, () => new ReservationDateSelectViewModel(navigatorStore));
    }
}