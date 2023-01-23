using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autofac;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.DialogDriver.Model;
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
        var reservedDates = GetReservedDates();
        BlackoutDates(reservedDates);
    }

    private void BlackoutDates(List<(DateTime FromDate, DateTime ToDate)> dates)
    {
        var pastDates = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1));
        FromDate.BlackoutDates.Add(pastDates);
        ToDate.BlackoutDates.Add(pastDates);
        
        dates.ForEach(x =>
        {
            var dateRange = new CalendarDateRange(x.FromDate, x.ToDate);
            FromDate.BlackoutDates.Add(dateRange);
            ToDate.BlackoutDates.Add(dateRange);
        });
    }
    
    private List<(DateTime FromDate, DateTime ToDate)> GetReservedDates()
    {
        var container = (Application.Current as App)!.Container;
        var unitOfWork = container.Resolve<UnitOfWork>();
        var viewModel = (ReservationDateSelectViewModel) DataContext;
        
        var roomName = viewModel.RoomName;
        var room = unitOfWork.Repository<Room>().GetQueryable().FirstOrDefault(x => x.Name == roomName);
        if (room is null) return null;
        var ordersDates = unitOfWork.Repository<Order>().GetQueryable().Where(x => x.RoomId == room.RoomId).ToList();
        
        List<(DateTime FromDate, DateTime ToDate)> valueTuple = new();
        ordersDates.ForEach(x =>
        {
            valueTuple.Add((x.FromDate, x.ToDate));
        });
        
        return valueTuple;
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
                            _tts.SpeakAsync(TxtError.Text);
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
                        _tts.SpeakAsync(
                            "Powiedz datę zameldowania i wymeldowania aby przejść dalej. Powiedz wstecz aby wrócić do opisu pokoju. Powiedz wyczyść aby wyczyścić formularz.");
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
        }
        else
        {
            _tts.SpeakAsyncLowConfidence();
        }
    }

    private bool ValidateForm()
    {
        TxtError.Text = string.Empty;
        if (EmptyFields()) return false;

        try
        {
            if (DateTime.Compare(DateTime.Parse(FromDate.Text), DateTime.Parse(ToDate.Text)) >= 0)
            {
                TxtError.Text += "Data zameldowania nie może być późniejsza niż data wymeldowania";
                return false;
            }
        }
        catch (Exception)
        {
            TxtError.Text += "Data jest w niepoprawnym formacie";
            return false;
        }

        return true;

        bool EmptyFields()
        {
            TxtError.Text = string.Empty;
            TxtError.Text += FromDate.Text == string.Empty ? "Pole data zameldowania jest puste\n" : string.Empty;
            TxtError.Text += ToDate.Text == string.Empty ? "Pole data wymeldowania jest puste" : string.Empty;
            return !string.IsNullOrEmpty(TxtError.Text);
        }
    }
    private void Reset()
    {
        FromDate.Text = "";
        ToDate.Text = "";
        TxtError.Text = "";
    }
    
    private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
    {
        if (!ValidateForm()) return;
        var viewModel = (ReservationDateSelectViewModel) DataContext;
        if (viewModel.NavigateFacilitiesSelectionCommand.CanExecute(null) &&
            viewModel.SaveDatesCommand.CanExecute(null))
        {
            viewModel.FromDate = FromDate.Text;
            viewModel.ToDate = ToDate.Text;
            viewModel.SaveDatesCommand.Execute(null);
            viewModel.NavigateFacilitiesSelectionCommand.Execute(null);
        }
    }
}