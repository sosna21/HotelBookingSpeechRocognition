using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class RoomSelectViewModel : ViewModelBase
{

    public ICommand NavigateRoomDescriptionCommand { get; }

    public RoomSelectViewModel(NavigationStore navigatorStore)
    {

        NavigateRoomDescriptionCommand=
            new NavigateCommand<RoomDescriptionViewModel>(navigatorStore, () => new RoomDescriptionViewModel(navigatorStore));
    }
}