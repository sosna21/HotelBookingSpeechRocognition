using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.Models;
using SWPSD_PROJEKT.UI.ViewModels;
using Xceed.Wpf.AvalonDock.Controls;

namespace SWPSD_PROJEKT.UI.Views;

public partial class RoomSelect : UserControl
{
    private SpeechRecognition _sre;
    private SpeechSynthesis _tts;

    public RoomSelect()
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
        _sre.LoadRoomSelectGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
        _tts.SpeakAsync("Witamy w systemie rezerwacji pokoi hotelu Royal");
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
        if (confidence > 0.5)
        {
            if (result.Grammar.RuleName == "rootRoomSelect")
            {
                string opcja = result.Semantics["Opcja"].Value.ToString();
                switch (opcja)
                {
                    case "Pokój dwuosobowy":
                        ImgBtn_OnClick(ComfortImgBtn, null);
                        break;
                    case "Pokój trzyosobowy":
                        ImgBtn_OnClick(ApartamentImgBtn, null);
                        break;
                    case "Pokój czteroosobowy":
                        ImgBtn_OnClick(DeluxeImgBtn, null);
                        break;
                    case "Apartament czteroosobowy":
                        ImgBtn_OnClick(SuperDeluxeImgBtn, null);
                        break;
                    case "Pomoc":
                        _tts.SpeakAsync("Powiedz jaki pokój chcesz zarezerwować");
                        break;
                    default:
                        _tts.SpeakAsync("Nierozpoznana komenda");
                        break;
                }
            }
        }
    }

    private void ImgBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        var img = btn.FindLogicalChildren<Image>().FirstOrDefault();
        var selectedRoom = btn.FindLogicalChildren<TextBlock>().FirstOrDefault()?.Text ?? "Not found";
        var viewModel = (RoomSelectViewModel)DataContext;
        if (viewModel.SelectRoomCommand.CanExecute(null) && viewModel.NavigateRoomDescriptionCommand.CanExecute(null))
        {
            viewModel.SelectedRoom = new Room {RoomName = selectedRoom, RoomImg = img};
            viewModel.SelectRoomCommand.Execute(null);
            viewModel.NavigateRoomDescriptionCommand.Execute(null);
        }
    }
}