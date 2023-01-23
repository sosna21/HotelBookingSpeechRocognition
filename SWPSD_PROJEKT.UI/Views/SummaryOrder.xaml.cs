using System.Windows;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Views;

public partial class SummaryOrder : UserControl
{
    private SpeechRecognition _sre;

    private SpeechSynthesis _tts;

    public SummaryOrder()
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
        _sre.LoadSummaryOrderGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
        _tts.SpeakAsync(GoodbyeMsg.Text);
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
        var txt = result.Text;
        if (confidence > 0.5)
        {
            if (result.Grammar.RuleName == "rootSummaryOrder")
            {
                string opcja = result.Semantics["Opcja"].Value.ToString();
                switch (opcja)
                {
                    case "Powrót do głównej":
                        _tts.SpeakAsync(
                            "Powracam do strony głównej");
                        BackBtn.Command.Execute(null);
                        break;
                    case "Pomoc":
                        _tts.SpeakAsync(
                            "Powiedz Powrót do głównej aby wrócić do strony głównej i zacząć rezerwacje od nowa");
                        break;
                    default:
                        _tts.SpeakAsync("Nierozpoznana komenda");
                        break;
                }
            }
        }
    }
    private void ContinueBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var viewModel = (SummaryOrderViewModel) DataContext;
        if (viewModel.NavigateRoomSelectCommand.CanExecute(null))
        {
            viewModel.NavigateRoomSelectCommand.Execute(null);
        }
        else return;
    }
    
}