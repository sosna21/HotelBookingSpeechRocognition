using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class FacilitiesSelectionViewModel : ViewModelBase
{
    public ICommand NavigateReservationDateSelectCommand { get; }
    public ICommand NavigateSummaryOrderCommand { get; }

    public FacilitiesSelectionViewModel(NavigationStore navigatorStore)
    {
        NavigateReservationDateSelectCommand = new NavigateCommand<ReservationDateSelectViewModel>(navigatorStore, () => new ReservationDateSelectViewModel(navigatorStore));
        NavigateSummaryOrderCommand = new NavigateCommand<SummaryOrderViewModel>(navigatorStore, () => new SummaryOrderViewModel(navigatorStore));
    }
}