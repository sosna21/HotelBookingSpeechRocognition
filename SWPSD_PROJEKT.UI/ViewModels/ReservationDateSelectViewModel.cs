using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class ReservationDateSelectViewModel : ViewModelBase
{
    public ICommand NavigateFacilitiesSelectionCommand { get; }
    public ICommand NavigateRoomDescriptionCommand { get; }

    public ReservationDateSelectViewModel(NavigationStore navigatorStore)
    {
        NavigateFacilitiesSelectionCommand = new NavigateCommand<FacilitiesSelectionViewModel>(navigatorStore, () => new FacilitiesSelectionViewModel(navigatorStore));
        NavigateRoomDescriptionCommand = new NavigateCommand<RoomDescriptionViewModel>(navigatorStore, () => new RoomDescriptionViewModel(navigatorStore));
    }
}