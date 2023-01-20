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
                    case "Apartament Luxus":
                        //TODO potwierdzenie przed przejściem dalej (np powiedz potwierdz wybór)
                        _tts.SpeakAsync("Wybrano apartament luxus");
                        ComfortImgBtn.Command.Execute(null);
                        break;
                    case "Apartament Comfort":
                        _tts.SpeakAsync("Wybrano apartament comfort");
                        ApartamentImgBtn.Command.Execute(null);
                        break;
                    //TODO usunac czytanie z gramatyki
                    case "Czytaj Apartament Luxus":
                        // ReadDescription_OnMouseDown(Room1Icon, null);
                        break;
                    case "Czytaj Apartament Comfort":
                        // ReadDescription_OnMouseDown(Room2Icon, null);
                        break;
                    case "Pomoc":
                        //TODO implement
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