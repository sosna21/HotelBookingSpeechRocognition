﻿using System.Windows.Input;
using SWPSD_PROJEKT.UI.Commands;
using SWPSD_PROJEKT.UI.Stores;

namespace SWPSD_PROJEKT.UI.ViewModels;

public class ReservationDateSelectViewModel : ViewModelBase
{
    //Set by binding
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public ICommand NavigateFacilitiesSelectionCommand { get; }
    public ICommand NavigateRoomDescriptionCommand { get; }
    public ICommand SaveDatesCommand { get; }

    public ReservationDateSelectViewModel(NavigationStore navigatorStore, RoomStore roomStore, ReservationDateStore reservationStore)
    {
        NavigateFacilitiesSelectionCommand = new NavigateCommand<FacilitiesSelectionViewModel>(navigatorStore, () => new FacilitiesSelectionViewModel(navigatorStore, roomStore, reservationStore));
        NavigateRoomDescriptionCommand = new NavigateCommand<RoomDescriptionViewModel>(navigatorStore, () => new RoomDescriptionViewModel(navigatorStore, roomStore, reservationStore));
        SaveDatesCommand = new SaveDatesCommand(reservationStore, this);
    }
}