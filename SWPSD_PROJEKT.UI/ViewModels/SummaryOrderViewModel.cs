using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class SummaryOrderViewModel : ViewModelBase
{
    public ICommand NavigateHomeCommand { get; }
    public ICommand NavigateFacilitiesSelectionCommand { get; }

    public SummaryOrderViewModel(NavigationStore navigatorStore)
    {
        NavigateHomeCommand =
            new NavigateCommand<HomeViewModel>(navigatorStore, () => new HomeViewModel(navigatorStore));
        NavigateFacilitiesSelectionCommand=
            new NavigateCommand<FacilitiesSelectionViewModel>(navigatorStore, () => new FacilitiesSelectionViewModel(navigatorStore));
    }
}