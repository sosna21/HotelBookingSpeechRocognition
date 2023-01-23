using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.DialogDriver.Model;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Views;

public partial class RoomDescription : UserControl
{
    private SpeechRecognition _sre;

    private SpeechSynthesis _tts;

    public RoomDescription()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var container = (Application.Current as App)!.Container;
        _sre = container.Resolve<SpeechRecognition>();
        _tts = container.Resolve<SpeechSynthesis>();
        _sre.UnloadAllGrammar();
        _sre.LoadRoomDescriptionGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
        _tts.SpeakAsync(RoomName.Text);
        var room = GetRoom();
        RoomDesc.Text = room.Description;
        SaveRoomData(room);
    }

    private Room GetRoom()
    {
        var container = (Application.Current as App)!.Container;
        var unitOfWork = container.Resolve<UnitOfWork>();
        var roomName = RoomName.Text;
        var room = unitOfWork.Repository<Room>().GetQueryable().FirstOrDefault(x => x.Name == roomName);
        if (room != null)
        {
            return room;
        }
        return null;
    }

    private void SaveRoomData(Room room)
    {
        var viewModel = (RoomDescriptionViewModel) DataContext;
        viewModel.ReservationStore.CurrentRoom = room;
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
        var txt = result.Text;
        if (confidence > 0.5)
        {
            if (result.Grammar.RuleName == "rootRoomDescription")
            {
                string opcja = result.Semantics["Opcja"].Value.ToString();
                switch (opcja)
                {
                    case "Czytaj opis":
                        ReadDescription();
                        break;
                    case "Zarezerwuj":
                        ContinueBtn.Command.Execute(null);
                        break;
                    case "Pomoc":
                        _tts.SpeakAsync(
                            "Powiedz Zarezerwuj jeśli chcesz przejść do rezerwacji tego pokoju lub wstecz jeśli chcesz przejśc do rezerwacji innego pokoju. Dodatkowo możesz poprosić o przeczytanie opisu pokoju komendą Czytaj opis i zastopować czytanie przyciskiem Zakończ czytanie opisu");
                        break;
                    case "Wstecz":
                        var viewModel = (RoomDescriptionViewModel) DataContext;
                        if (viewModel.NavigateRoomSelectCommand.CanExecute(null))
                            viewModel.NavigateRoomSelectCommand.Execute(null);
                        break;
                    default:
                        _tts.SpeakAsync("Nierozpoznana komenda");
                        break;
                }
            }
        }
    }

    private void ReadDescription()
    {
        _tts.SpeakAsync(RoomDesc.Text);
    }
}