using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class ReservationDateSelectViewModel : ViewModelBase
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string RoomName { get; set; }
    public ICommand NavigateFacilitiesSelectionCommand { get; }
    public ICommand NavigateRoomDescriptionCommand { get; }
    public ICommand SaveDatesCommand { get; }

    public ReservationDateSelectViewModel(NavigationStore navigatorStore, RoomStore roomStore, ReservationDataStore reservationStore)
    {
        NavigateFacilitiesSelectionCommand = new NavigateCommand<FacilitiesSelectionViewModel>(navigatorStore, () => new FacilitiesSelectionViewModel(navigatorStore, roomStore, reservationStore));
        NavigateRoomDescriptionCommand = new NavigateCommand<RoomDescriptionViewModel>(navigatorStore, () => new RoomDescriptionViewModel(navigatorStore, roomStore, reservationStore));
        SaveDatesCommand = new SaveDatesCommand(reservationStore, this);

        FromDate = reservationStore.CurrentReservationDates?.FromDate.ToString("MM/dd/yyyy");
        ToDate = reservationStore.CurrentReservationDates?.ToDate.ToString("MM/dd/yyyy");
        RoomName = roomStore.CurrentRoom?.RoomName;
    }
}