using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ICommand NavigateRoomSelectCommand { get; }

    public HomeViewModel(NavigationStore navigatorStore)
    {
        NavigateRoomSelectCommand = new NavigateCommand<RoomSelectViewModel>(navigatorStore, () => new RoomSelectViewModel(navigatorStore));
    }
}