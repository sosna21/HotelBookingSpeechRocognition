using System;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Views;

public partial class FacilitiesSelection : UserControl
{
    private SpeechRecognition _sre;
    private SpeechSynthesis _tts;

    public FacilitiesSelection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RadioBtn.IsChecked = true;
        var container = (Application.Current as App)!.Container;
        _sre = container.Resolve<SpeechRecognition>();
        _tts = container.Resolve<SpeechSynthesis>();
        _sre.UnloadAllGrammar();
        _sre.LoadUserDataGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
        _tts.SpeakAsync("Proszę uzupełnić swoje dane oraz wybrać ewentualne udogodnienia");
    }

    private void Reset()
    {
        TxtName.Text = "";
        TxtSurname.Text = "";
        TxtTelephone.Text = "";
        TxtCardNumber.Text = "";
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
        var txt = result.Text;
        // _tts.SpeakAsync(confidence.ToString("0.00"));
        if (confidence > 0.5)
        {
            if (result.Grammar.RuleName == "rootUserDataSelect")
            {
                string opcja = result.Semantics["UserDataSelect"].Value.ToString();
                switch (opcja)
                {
                    case "Imie":
                        TxtName.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
                        _sre.UnloadAllGrammar();
                        _sre.LoadNameSelectGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case "Nazwisko":
                        TxtSurname.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
                        _sre.UnloadAllGrammar();
                        _sre.LoadSurnameSelectGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case "Telefon":
                        TxtTelephone.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
                        _sre.UnloadAllGrammar();
                        _sre.LoadTelephoneGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case "Wyczyść":
                        Reset();
                        _tts.SpeakAsync("Nastąpiło zresetowanie formularza");
                        break;
                    case "Wstecz":
                        var viewModel = (FacilitiesSelectionViewModel)DataContext;
                        break;
                    //to do bo nie wiem co zrobic
                    case "Pomoc":
                        _tts.SpeakAsync("Wypełnij podane pola, aby zakończyć rezerwację pokoju");
                        break;
                    case "dalej":
                        var error = ValidateForm();
                        if (string.IsNullOrEmpty(error))
                            ContinueBtn.Command.Execute(null);
                        break;


                    default:
                        _tts.SpeakAsync("Nierozpoznana komenda");
                        break;
                }
            }
            // else if (re)
            // {
            //     
            // }
        }
    }


    private string ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtSurname.Text)
                                                    || string.IsNullOrWhiteSpace(TxtTelephone.Text) ||
                                                    string.IsNullOrWhiteSpace(TxtCardNumber.Text))
            return "Formularz zawiera puste pola, uzupełnij je zanim przejdziesz dalej.";
        return null;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Reset();
    }
    
    private void ContinueBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var viewModel = (FacilitiesSelectionViewModel) DataContext;
        if (viewModel.NavigateSummaryOrderCommand.CanExecute(null) &&
            viewModel.SaveFacilities.CanExecute(null))
        {
            viewModel.SaveFacilities.Execute(null);
            viewModel.NavigateSummaryOrderCommand.Execute(null);
        }
        
        //TODO save to database
    }
}