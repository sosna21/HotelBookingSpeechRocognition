using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.DialogDriver.Model;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.Models;
using SWPSD_PROJEKT.UI.ViewModels;
using Room = SWPSD_PROJEKT.DialogDriver.Model.Room;

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
    
        private void SpeechRecognized(RecognitionResult result)
    {
        if (DataContext == null) return;
        float confidence = result.Confidence;
        if (confidence > 0.5)
        {
            if (result.Grammar.RuleName == "rootUserDataSelect")
            {
                string opcja = result.Semantics["UserDataSelect"].Value.ToString();
                switch (opcja)
                {
                    case "Imię":
                        TxtName.Focus();
                        _tts.SpeakAsync($"Wybrano pole {opcja} podaj wartość");
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
                    case "Wybierz łóżko":
                        RadioBtn.Focus();
                        _tts.SpeakAsync("Wybierz opcję łóżka dla rezerwowanego pokoju");
                        break;
                    case "Wybierz udogodnienia":
                        BreakfastCh.Focus();
                        _tts.SpeakAsync(
                            "Aby wybrać udogodnienie, powiedz jego nazwę i tak lub nie, na przykład Śniadanie tak");
                        break;
                    case "Wyczyść dane":
                        Reset();
                        _tts.SpeakAsync("Nastąpiło zresetowanie formularza");
                        break;
                    case "Wstecz":
                        var viewModel = (FacilitiesSelectionViewModel) DataContext;
                        if (viewModel.NavigateReservationDateSelectCommand.CanExecute(null))
                            viewModel.NavigateReservationDateSelectCommand.Execute(null);
                        break;
                    case "Pomoc":
                        _tts.SpeakAsync("Wypełnij podane pola, aby zakończyć rezerwację pokoju");
                        break;
                    case "Dalej":
                        var error = ValidateForm();
                        if (string.IsNullOrEmpty(error))
                            ContinueBtn.Command.Execute(null);
                        else
                        {
                            TxtError.Text = error;
                            TxtError.Visibility = Visibility.Visible;
                            _tts.SpeakAsync(error);
                        }
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
                    case "Śniadanie tak":
                        _tts.SpeakAsync("Dodano śniadanie");
                        BreakfastCh.IsChecked = true;
                        break;
                    case "Śniadanie nie":
                        _tts.SpeakAsync("Usunięto śniadanie");
                        BreakfastCh.IsChecked = false;
                        break;
                    case "Zwierzęta tak":
                        _tts.SpeakAsync("Dodano zwierzęta");
                        AnimalsCh.IsChecked = true;
                        break;
                    case "Zwierzęta nie":
                        _tts.SpeakAsync("Usunięto zwierzęta");
                        AnimalsCh.IsChecked = false;
                        break;
                    case "Barek z alkoholem tak":
                        _tts.SpeakAsync("Dodano barek z alkoholem");
                        AlcoholCh.IsChecked = true;
                        break;
                    case "Barek z alkoholem nie":
                        _tts.SpeakAsync("Usunięto barek z alkoholem");
                        AlcoholCh.IsChecked = false;
                        break;
                    case "Dostawka dla dziecka tak":
                        _tts.SpeakAsync("Dodano dostawkę dla dziecka");
                        ChildAddCh.IsChecked = true;
                        break;
                    case "Dostawka dla dziecka nie":
                        _tts.SpeakAsync("Usunięto dostawkę dla dziecka");
                        ChildAddCh.IsChecked = false;
                        break;
                }
            }
            else if (result.Grammar.RuleName == "rootSelectBed")
            {
                string opcja = result.Semantics["BedOption"].Value.ToString();
                switch (opcja)
                {
                    case "pojedyncze":
                        _tts.SpeakAsync("Wybrano pojedyncze łóżko do pokoju");
                        RadioBtn.IsChecked = true;
                        DoubleBedCh.IsChecked = false;
                        break;
                    case "podwójne":
                        _tts.SpeakAsync("Wybrano łóżko podwójne");
                        DoubleBedCh.IsChecked = true;
                        RadioBtn.IsChecked = false;
                        break;
                }
            }
        }
    }

    private void Reset()
    {
        TxtName.Text = string.Empty;
        TxtSurname.Text = string.Empty;
        TxtTelephone.Text = string.Empty;
        TxtCreditCardNumber.Text = string.Empty;
        TxtError.Text = string.Empty;
        TxtError.Visibility = Visibility.Hidden;
    }


    private string ValidateForm()
    {
        TextBox[] controls = {TxtName, TxtSurname, TxtTelephone, TxtCreditCardNumber};

        if (controls.Any(x => string.IsNullOrEmpty(x.Text)))
            return "Formularz zawiera puste pola, uzupełnij je zanim przejdziesz dalej.";

        if (!IsPhoneNumber(TxtTelephone.Text))
        {
            return "Niepoprawny numer Tel.";
        }

        if (!IsCreditCardNumberNumber(TxtCreditCardNumber.Text))
        {
            return "Niepoprawny numer karty płatniczej.";
        }

        return null;
    }


    private bool IsPhoneNumber(string number)
    {
        return Regex.Match(number, @"^([0-9]{9})$").Success;
    }

    private bool IsCreditCardNumberNumber(string number)
    {
        return Regex.Match(number, @"^([0-9]{16})$").Success;
    }
    
    private void SaveRoomReservation()
    {
        var container = (Application.Current as App)!.Container;
        var viewModel = (FacilitiesSelectionViewModel) DataContext;
        var unitOfWork = container.Resolve<UnitOfWork>();
        //Create guest
        var guest = new Guest
        {
            Name = TxtName.Text, Surname = TxtSurname.Text, PhoneNumber = TxtTelephone.Text,
            CreditCardNumber = TxtCreditCardNumber.Text
        };
        unitOfWork.Repository<Guest>().Add(guest);

        //Get room
        var roomName = viewModel.RoomStore.CurrentRoom.RoomName;
        var room = unitOfWork.Repository<Room>().GetQueryable().FirstOrDefault(x => x.Name == roomName);

        //Get From and To date
        var fromDate = viewModel.ReservationStore.CurrentReservationDates.FromDate;
        var toDate = viewModel.ReservationStore.CurrentReservationDates.ToDate;
        var days = (toDate - fromDate).Days;

        //Get selected facilities from db
        Facilities facilities = viewModel.Facilities;
        var dbFacilities = facilities.GetDbFacilities(unitOfWork.Repository<Facility>()).Where(x => x is not null)
            .ToList();

        //Calculate total price
        var price = room.PricePerNight * days;
        dbFacilities.ForEach(x => price += x.Price);

        //Create order
        var order = new Order
            {Room = room, Guest = guest, FromDate = fromDate, ToDate = toDate, Days = days, TotalPrice = price};
        unitOfWork.Repository<Order>().Add(order);

        var facilityOrdersRepo = unitOfWork.Repository<FacilityOrder>();
        foreach (var dbFacility in dbFacilities)
        {
            facilityOrdersRepo.Add(new FacilityOrder {Order = order, Facility = dbFacility});
        }

        unitOfWork.Complete();
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
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Reset();
    }

    private void ContinueBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var errors = ValidateForm();
        if (!string.IsNullOrEmpty(errors))
        {
            TxtError.Text = errors;
            TxtError.Visibility = Visibility.Visible;
            return;
        }
        
        var viewModel = (FacilitiesSelectionViewModel) DataContext;
        if (viewModel.NavigateSummaryOrderCommand.CanExecute(null) &&
            viewModel.SaveFacilities.CanExecute(null))
        {
            viewModel.SaveFacilities.Execute(null);
            viewModel.NavigateSummaryOrderCommand.Execute(null);
        }
        else return;

        SaveRoomReservation();
    }
    
    private void TxtName_OnGotFocus(object sender, RoutedEventArgs e)
    {
        _sre.LoadNameSelectGrammar();
    }

    private void TxtName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        _sre.UnloadNameSelectGrammar();
    }

    private void TxtSurname_OnGotFocus(object sender, RoutedEventArgs e)
    {
        _sre.LoadSurnameSelectGrammar();
    }

    private void TxtSurname_OnLostFocus(object sender, RoutedEventArgs e)
    {
        _sre.UnloadSurnameSelectGrammar();
    }

    private void TxtTelephone_OnGotFocus(object sender, RoutedEventArgs e)
    {
        _sre.LoadTelephoneGrammar();
    }

    private void TxtTelephone_OnLostFocus(object sender, RoutedEventArgs e)
    {
        _sre.UnloadTelephoneGrammar();
    }

    private void TxtCreditCardNumber_OnGotFocus(object sender, RoutedEventArgs e)
    {
        _sre.LoadCreditCardGrammar();
    }

    private void TxtCreditCardNumber_OnLostFocus(object sender, RoutedEventArgs e)
    {
        _sre.UnloadCreditCardGrammar();
    }

    private void RadioBtn_OnGotFocus(object sender, RoutedEventArgs e)
    {
        _sre.LoadBedSelectGrammar();
    }

    private void RadioBtn_OnLostFocus(object sender, RoutedEventArgs e)
    {
        _sre.UnloadBedSelectGrammar();
    }

    private void Checkbox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        _sre.LoadFacilitiesGrammar();
    }

    private void Checkbox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        _sre.UnloadFacilitiesGrammar();
    }
}