using System;
using SWPSD_PROJEKT.UI.Models;
using SWPSD_PROJEKT.UI.Stores;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Commands;

public class SaveFacilities : CommandBase
{
    private readonly ReservationDateStore _reservationStore;
    private readonly FacilitiesSelectionViewModel _facilitiesSelectionViewModel;

    public SaveFacilities(ReservationDateStore reservationStore, FacilitiesSelectionViewModel facilitiesSelectionViewModel)
    {
        _reservationStore = reservationStore;
        _facilitiesSelectionViewModel = facilitiesSelectionViewModel;
    }

    public override void Execute(object parameter)
    {
        _reservationStore.CurrentFacilities = _facilitiesSelectionViewModel.Facilities;
    }
}