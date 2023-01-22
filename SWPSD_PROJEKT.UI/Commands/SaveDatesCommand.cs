using System;
using System.Globalization;
using SWPSD_PROJEKT.UI.Models;
using SWPSD_PROJEKT.UI.Stores;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Commands;

public class SaveDatesCommand : CommandBase
{
    private readonly ReservationDateStore _reservationStore;
    private readonly ReservationDateSelectViewModel _roomSelectViewModel;

    public SaveDatesCommand(ReservationDateStore reservationStore, ReservationDateSelectViewModel roomSelectViewModel)
    {
        _reservationStore = reservationStore;
        _roomSelectViewModel = roomSelectViewModel;
    }

    public override void Execute(object parameter)
    {
        var reservationDates = new ReservationDates
        {
            FromDate = DateTime.Parse(_roomSelectViewModel.FromDate),
            ToDate = DateTime.Parse(_roomSelectViewModel.ToDate)
        };
        
        _reservationStore.CurrentReservationDates = reservationDates;
    }
}