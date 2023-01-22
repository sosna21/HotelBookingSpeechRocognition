
using System;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Views;

public partial class ReservationDateSelect : UserControl
{
    private SpeechRecognition _sre;
    private SpeechSynthesis _tts;
    public ReservationDateSelect()
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
                    case "Kontynuuj":
                        if (ValidateForm())
                            BtnContinue_OnClick(null, null);
                        else
                            _tts.SpeakAsync(txtError.Text);
                        break;
                    case "Wyczyść":
                        Reset();
                        _tts.SpeakAsync("Nastąpiło wyczyszczenie formularza");
                        break;
                    case "Wstecz":
                        var viewModel = (ReservationDateSelectViewModel) DataContext;
                        if (viewModel.NavigateRoomDescriptionCommand.CanExecute(null))
                            viewModel.NavigateRoomDescriptionCommand.Execute(null);
                        break;
                    case "Pomoc":
                        _tts.SpeakAsync("Powiedz datę zameldowania i wymeldowania aby przejść dalej. Powiedz wstecz aby wrócić do opisu pokoju. Powiedz wyczyść aby wyczyścić formularz.");
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
            // else if (result.Grammar.RuleName == "rootNumer")
            // {
            //     var number = result.Semantics["Numer"].Value.ToString();
            // }
        }
        else
        {
            _tts.SpeakAsyncLowConfidence();
        }
    }

    // private string ValidateForm()
    // {
    //     if (string.IsNullOrWhiteSpace(ToDate.Text) || string.IsNullOrWhiteSpace(FromDate.Text))
    //         return "Formularz zawiera puste pola, uzupełnij je zanim przejdziesz dalej.";
    //     return null;
    // }
    private bool ValidateForm()
    {
        bool isValid = true;
        txtError.Text = string.Empty;
        if (EmptyFields()) isValid = false;
        if (string.IsNullOrWhiteSpace(FromDate.Text) || string.IsNullOrWhiteSpace(ToDate.Text))
        {
            txtError.Text += "Formularz zawiera puste pola, uzupełnij je zanim przejdziesz dalej.\n";
            isValid = false;
        }

        try
        {
            if (DateTime.Compare(DateTime.Parse(FromDate.Text), DateTime.Parse(ToDate.Text)) > 0)
            {
                txtError.Text += "Data zameldowania nie może być póniejsza niż data wymeldowania\n";
                isValid = false;
            }
        }
        catch (Exception e)
        {
            txtError.Text += "Data jest w niepoprawnym formacie\n";
            isValid = false;
        }
        return isValid;
        
        bool EmptyFields()
        {
            txtError.Text = string.Empty;
            txtError.Text += FromDate.Text == string.Empty ? "Pole data zameldowania jest wymagane\n" : string.Empty;
            txtError.Text += ToDate.Text == string.Empty ? "Pole data wymeldowania jest wymagane\n" : string.Empty;
            return !string.IsNullOrEmpty(txtError.Text);
        }
    }

    private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
    {
        
        //if (!ValidateForm1()) return;
        //TODO add validation
        //if (string.IsNullOrWhiteSpace(ToDate.Text) || string.IsNullOrWhiteSpace(FromDate.Text))
            // "Formularz zawiera puste pola, uzupełnij je zanim przejdziesz dalej.";

        var viewModel = (ReservationDateSelectViewModel) DataContext;
        if (viewModel.NavigateFacilitiesSelectionCommand.CanExecute(null) &&
            viewModel.SaveDatesCommand.CanExecute(null))
        {
            //TODO uncomment at end
            viewModel.FromDate = FromDate.Text;
            viewModel.ToDate = ToDate.Text;
            viewModel.SaveDatesCommand.Execute(null);
            viewModel.NavigateFacilitiesSelectionCommand.Execute(null);
        }
    }
}