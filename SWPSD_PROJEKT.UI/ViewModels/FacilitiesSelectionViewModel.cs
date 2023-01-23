using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Models;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class FacilitiesSelectionViewModel : ViewModelBase
{
    public ReservationDataStore ReservationStore { get; set; }

    public string FromDate => ReservationStore.CurrentReservationDates.FromDate.ToLongDateString();
    public string ToDate => ReservationStore.CurrentReservationDates.ToDate.ToLongDateString();

    public decimal Price =>
        (ReservationStore.CurrentReservationDates.ToDate - ReservationStore.CurrentReservationDates.FromDate).Days *
        ReservationStore.CurrentRoom.PricePerNight;

    public RoomStore RoomStore { get; }
    public Facilities Facilities { get; } = new();
    public ICommand NavigateReservationDateSelectCommand { get; }
    public ICommand NavigateSummaryOrderCommand { get; }
    public ICommand SaveFacilities { get; }

    public FacilitiesSelectionViewModel(NavigationStore navigatorStore, RoomStore roomStore,
        ReservationDataStore reservationStore)
    {
        ReservationStore = reservationStore;
        RoomStore = roomStore;
        NavigateReservationDateSelectCommand = new NavigateCommand<ReservationDateSelectViewModel>(navigatorStore,
            () => new ReservationDateSelectViewModel(navigatorStore, roomStore, reservationStore));
        NavigateSummaryOrderCommand =
            new NavigateCommand<SummaryOrderViewModel>(navigatorStore, () => new SummaryOrderViewModel(navigatorStore));
        SaveFacilities = new SaveFacilities(reservationStore, this);
    }
}