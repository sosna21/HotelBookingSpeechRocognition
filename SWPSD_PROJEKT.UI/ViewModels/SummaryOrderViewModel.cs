using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class SummaryOrderViewModel : ViewModelBase
{
    public ICommand NavigateRoomSelectCommand { get; }

    public SummaryOrderViewModel(NavigationStore navigatorStore)
    {

        NavigateRoomSelectCommand=
            new NavigateCommand<RoomSelectViewModel>(navigatorStore, () => new RoomSelectViewModel(navigatorStore, new RoomStore(), new ReservationDataStore()));
    }
}