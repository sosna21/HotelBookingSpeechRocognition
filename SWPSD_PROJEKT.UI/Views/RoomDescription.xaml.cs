using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    // public string roomDescription { get; set; } = "Opis pokoju taki że ojej";


    public RoomDescription()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        // this.DataContext = this;
        // Unloaded += OnUnloaded;
    }

    // private void OnUnloaded(object sender, RoutedEventArgs e)
    // {
    //     //TODO reset subscriptions here (trudne)
    // }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var container = (Application.Current as App)!.Container;
        _sre = container.Resolve<SpeechRecognition>();
        _tts = container.Resolve<SpeechSynthesis>();
        _sre.UnloadAllGrammar();
        _sre.LoadRoomDescriptionGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
        _tts.SpeakAsync(RoomName.Text);
        RoomDesc.Text = GetRoomDescription();
        //TODO load description from DB
        
    }

    private string GetRoomDescription()
    {
        var container = (Application.Current as App)!.Container;
        var unitOfWork = container.Resolve<UnitOfWork>();
        var roomName = RoomName.Text;
        var room = unitOfWork.Repository<Room>().GetQueryable().FirstOrDefault(x => x.Name == roomName);
        if (room != null)
        {
            var description = room.Description;
            return description;
        }
        return "Nie znaleziono";
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
        var txt = result.Text;
        if (confidence > 0.5)
        {
            //TODO dodać pozostałe wybory pokoii !! (dodac czytanie)
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

    private void StopBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _tts.TerminateSynthesis();
    }
}