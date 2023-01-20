using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Views;

public partial class HomeView : UserControl
{
    private SpeechRecognition _sre;
    private SpeechSynthesis _tts;
    public HomeView()
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
        _sre.LoadHomePageSystemGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
    }

    private void Reset()
    {
        FromDate.Text = "";
        ToDate.Text = "";
        PeopleNb.Text = "";
        //TODO reset error msg here
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return; 
        float confidence = result.Confidence;
        var txt = result.Text;
        if (confidence > 0.5)
        {
            if (result.Grammar.RuleName == "homePageSystemGrammar")
            {
                string opcja = result.Semantics["Opcja"].Value.ToString();
                switch (opcja)
                {
                    case "Data zameldowania":
                        FromDate.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
                        _sre.UnloadAllGrammar();
                        _sre.LoadCalendarGrammar1();
                        _sre.LoadHomePageSystemGrammar();
                        break;
                    case "Data wymeldowania":
                        ToDate.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
                        _sre.UnloadAllGrammar();
                        _sre.LoadCalendarGrammar2();
                        _sre.LoadHomePageSystemGrammar();
                        break;
                    case "Liczba osób":
                        PeopleNb.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
                        _sre.UnloadAllGrammar();
                        _sre.LoadNumberGrammar();
                        _sre.LoadHomePageSystemGrammar();
                        break;
                    case "Kontynuuj":
                        var error = ValidateForm();
                        if (string.IsNullOrEmpty(error))
                            BtnContinue.Command.Execute(null);
                        else
                            _tts.SpeakAsync(error);
                        //TODO add error msg control and add error text to it
                        break;
                    case "Reset":
                        Reset();
                        _tts.SpeakAsync("Nastąpiło zresetowanie formularza");
                        break;
                    default:
                        _tts.SpeakAsync("Nierozpoznana komenda");
                        break;
                }
            }
            else if (result.Grammar.RuleName == "rootData1")
            {
                var day = result.Semantics["Dzien"].Value.ToString();
                var month = result.Semantics["Miesiac"].Value.ToString();
                var year = result.Semantics["Rok"].Value.ToString();
                FromDate.Text = $"{day} {month} {year}";
            }
            else if (result.Grammar.RuleName == "rootData2")
            {
                var day = result.Semantics["Dzien"].Value.ToString();
                var month = result.Semantics["Miesiac"].Value.ToString();
                var year = result.Semantics["Rok"].Value.ToString();
                ToDate.Text = $"{day} {month} {year}";
            }
            else if (result.Grammar.RuleName == "rootNumer")
            {
                var number = result.Semantics["Numer"].Value.ToString();
                PeopleNb.Text = number;
            }
        }
        else
        {
            _tts.SpeakAsyncLowConfidence();
        }
    }

    private string ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(ToDate.Text) || string.IsNullOrWhiteSpace(FromDate.Text) ||
            string.IsNullOrEmpty(PeopleNb.Text))
            return "Formularz zawiera puste pola, uzupełnij je zanim przejdziesz dalej.";

        if (Regex.IsMatch(PeopleNb.Text, "[^0-9]+"))
        {
            PeopleNb.Focus();
            return "Pole liczba osób jest uzupełnione nieprawidłowo.\nPodaj właściwą wartość przed kontynuowaniem.";
        }

        return null;
    }

    private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
    {
        var errors = ValidateForm();
        if (string.IsNullOrEmpty(errors))
        {
            var viewModel = (HomeViewModel) DataContext;
            if (viewModel.NavigateRoomSelectCommand.CanExecute(null))
                viewModel.NavigateRoomSelectCommand.Execute(null);
        }
        else
        {
            //TODO add to error msg 
        }
    }
}