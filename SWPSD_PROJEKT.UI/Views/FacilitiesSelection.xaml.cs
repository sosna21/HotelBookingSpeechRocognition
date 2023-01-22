using System.Text;
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
        _sre.LoadFacilitiesGrammar();
        _sre.AddSpeechRecognizedEvent(SpeechRecognized);
        _tts.SpeakAsync("Proszę uzupełnić swoje dane oraz wybrać ewentualne udogodnienia");
    }

    private void Reset()
    {
        TxtName.Text = "";
        TxtSurname.Text = "";
        TxtTelephone.Text = "";
        TxtCreditCardNumber.Text = "";
    }

    private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
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
                    case "Numer karty płatniczej":
                        TxtCreditCardNumber.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
                        _sre.UnloadAllGrammar();
                        _sre.LoadCreditCardGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case "Wyczyść dane":
                        Reset();
                        _tts.SpeakAsync("Nastąpiło zresetowanie formularza");
                        break;
                    case "Wstecz":
                        var viewModel = (FacilitiesSelectionViewModel)DataContext;
                        if (viewModel.NavigateReservationDateSelectCommand.CanExecute(null))
                            viewModel.NavigateReservationDateSelectCommand.Execute(null);
                        break;
                    //to do bo nie wiem co zrobic
                    case "Pomoc":
                        _tts.SpeakAsync("Wypełnij podane pola, aby zakończyć rezerwację pokoju");
                        break;
                    case "Dalej":
                        var error = ValidateForm();
                        if (string.IsNullOrEmpty(error))
                            ContinueBtn.Command.Execute(null);
                        break;

                    default:
                        _tts.SpeakAsync("Nierozpoznana komenda");
                        break;
                }
            }
            else if (result.Grammar.RuleName == "rootNameSelect")
            {
                TxtName.Text = result.Text;
            }
            else if (result.Grammar.RuleName == "rootSurnameSelect")
            {
                TxtSurname.Text = result.Text;
            }
            else if (result.Grammar.RuleName == "rootTel")
            {
                var textResult = result.Text;
                var numbers = textResult.Split(' ');

                TxtTelephone.Text = SplitNumbers(numbers);
            }
            else if (result.Grammar.RuleName == "rootCreditCard")
            {
                var textResult = result.Text;
                var numbers = textResult.Split(' ');

                TxtCreditCardNumber.Text = SplitNumbers(numbers);
            }
            else if (result.Grammar.RuleName == "rootSelectFacility")
            {
                string opcja = result.Semantics["FacilityOption"].Value.ToString();
                switch (opcja)
                {
                    case ("Wybierz łóżko"):
                        _tts.SpeakAsync("Wybierz opcję łóżka dla rezerwowanego pokoju");
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("pojedyncze"):
                        _tts.SpeakAsync("Wybrano pojedyncze łóżko do pokoju");
                        RadioBtn.IsChecked = true;
                        DoubleBedCh.IsChecked = false;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("podwójne"):
                        _tts.SpeakAsync("Wybrano łóżko podwójne");
                        DoubleBedCh.IsChecked = true;
                        RadioBtn.IsChecked = false;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Wybierz udogodnienia"):
                        _tts.SpeakAsync(
                            "Aby wybrać udogodnienie, powiedz jego nazwę i tak lub nie, na przykład Śniadanie tak");
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Śniadanie tak"):
                        _tts.SpeakAsync("Dodano śniadanie");
                        BreakfastCh.IsChecked = true;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Śniadanie nie"):
                        _tts.SpeakAsync("Usunięto śniadanie");
                        BreakfastCh.IsChecked = false;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Zwierzęta tak"):
                        _tts.SpeakAsync("Dodano zwierzęta");
                        AnimalsCh.IsChecked = true;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Zwierzęta nie"):
                        _tts.SpeakAsync("Usunięto zwierzęta");
                        AnimalsCh.IsChecked = false;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Barek z alkoholem tak"):
                        _tts.SpeakAsync("Dodano barek z alkoholem");
                        AlcoholCh.IsChecked = true;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Barek z alkoholem nie"):
                        _tts.SpeakAsync("Usunięto barek z alkoholem");
                        AlcoholCh.IsChecked = false;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Dostawka dla dziecka tak"):
                        _tts.SpeakAsync("Dodano dostawkę dla dziecka");
                        ChildAddCh.IsChecked = true;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                    case ("Dostawka dla dziecka nie"):
                        _tts.SpeakAsync("Usunięto dostawkę dla dziecka");
                        ChildAddCh.IsChecked = false;
                        _sre.UnloadAllGrammar();
                        _sre.LoadFacilitiesGrammar();
                        _sre.LoadUserDataGrammar();
                        break;
                }
            }
        }
    }

    private string ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtSurname.Text)
                                                    || string.IsNullOrWhiteSpace(TxtTelephone.Text) ||
                                                    string.IsNullOrWhiteSpace(TxtCreditCardNumber.Text))
            return "Formularz zawiera puste pola, uzupełnij je zanim przejdziesz dalej.";
        return null;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Reset();
    }

    private void ContinueBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var viewModel = (FacilitiesSelectionViewModel)DataContext;
        if (viewModel.NavigateSummaryOrderCommand.CanExecute(null) &&
            viewModel.SaveFacilities.CanExecute(null))
        {
            viewModel.SaveFacilities.Execute(null);
            viewModel.NavigateSummaryOrderCommand.Execute(null);
        }

        //TODO save to database
    }

    string SplitNumbers(string[] numbers)
    {
        var txtBuilder = new StringBuilder();
        foreach (var number in numbers)
        {
            switch (number)
            {
                case "zero":
                    txtBuilder.Append('0');
                    break;
                case "jeden":
                    txtBuilder.Append('1');
                    break;
                case "dwa":
                    txtBuilder.Append('2');
                    break;
                case "trzy":
                    txtBuilder.Append('3');
                    break;
                case "cztery":
                    txtBuilder.Append('4');
                    break;
                case "pięć":
                    txtBuilder.Append('5');
                    break;
                case "sześć":
                    txtBuilder.Append('6');
                    break;
                case "siedem":
                    txtBuilder.Append('7');
                    break;
                case "osiem":
                    txtBuilder.Append('8');
                    break;
                case "dziewięć":
                    txtBuilder.Append('9');
                    break;
            }
        }

        return txtBuilder.ToString();
    }
}