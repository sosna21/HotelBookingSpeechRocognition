using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Views;

public partial class RoomSelect : UserControl
{
    private SpeechRecognition _sre;
    private SpeechSynthesis _tts;

    public RoomSelect()
    {
        InitializeComponent();
        Loaded += OnLoaded;
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
        _sre.LoadRoomSelectGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
        _tts.SpeakAsync("Witamy w systemie rezerwacji pokoi hotelu Royal");
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
        var txt = result.Text;
        if (confidence > 0.5)
        {
            //TODO dodać pozostałe wybory pokoii !! (dodac czytanie)
            if (result.Grammar.RuleName == "rootRoomSelect")
            {
                string opcja = result.Semantics["Opcja"].Value.ToString();
                switch (opcja)
                {
                    case "Pokój dwuosobowy":
                        //TODO potwierdzenie przed przejściem dalej (np powiedz potwierdz wybór)
                        _tts.SpeakAsync("Wybrano pokój dwuosobowy");
                        ComfortImgBtn.Command.Execute(null);
                        break;
                    case "Pokój trzyosobowy":
                        _tts.SpeakAsync("Wybrano pokój trzyosobowy");
                        ApartamentImgBtn.Command.Execute(null);
                        break;
                    //TODO usunac czytanie z gramatyki
                    case "Pokój czteroosobowy":
                        _tts.SpeakAsync("Wybrano pokój czteroosobowy");
                        ApartamentImgBtn.Command.Execute(null);
                        // ReadDescription_OnMouseDown(Room1Icon, null);
                        break;
                    case "Apartament czteroosobowy":
                        _tts.SpeakAsync("Wybrano apartament czteroosobowy");
                        SuperDeluxeImgBtn.Command.Execute(null);
                        // ReadDescription_OnMouseDown(Room2Icon, null);
                        break;
                    case "Pomoc":
                        _tts.SpeakAsync("Kliknij na pokój aby go zarezerwować");
                        break;
                    //TODO usunac z gramatyki dodac wstecz do pozostalych widokow
                    // case "Wstecz":
                    //     var viewModel = (RoomSelectViewModel) DataContext;
                    //     if (viewModel.NavigateRoomDescriptionCommand.CanExecute(null))
                    //         viewModel.NavigateRoomDescriptionCommand.Execute(null);
                    //     break;
                    default:
                        _tts.SpeakAsync("Nierozpoznana komenda");
                        break;
                }
            }
        }
    }
}