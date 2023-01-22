using SWPSD_PROJEKT.UI.Stores;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Commands;

public class SelectRoomCommand : CommandBase
{
    private readonly RoomStore _roomStore;
    private readonly RoomSelectViewModel _roomSelectViewModel;

    public SelectRoomCommand(RoomStore roomStore, RoomSelectViewModel roomSelectViewModel)
    {
        _roomStore = roomStore;
        _roomSelectViewModel = roomSelectViewModel;
    }

    public override void Execute(object parameter)
    {
        _roomStore.CurrentRoom = _roomSelectViewModel.SelectedRoom;
    }
}