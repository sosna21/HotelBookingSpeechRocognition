using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.TTS;

namespace SWPSD_PROJEKT.ASR;

public class SpeechRecognition
{
    private readonly SpeechRecognitionEngine _sre;
    private Dictionary<GrammarName, Grammar> _grammarsDict;
    private DialogControl _dialogControl;
    private readonly SpeechSynthesis _tts;


    public SpeechRecognition(DialogControl dialogControl, SpeechSynthesis tts)
    {
        _dialogControl = dialogControl;
        _tts = tts;
        _sre = new SpeechRecognitionEngine(new CultureInfo("pl-PL"));
        _sre.SetInputToDefaultAudioDevice();
        // _sre.SpeechRecognized += SRE_SpeechRecognized;

        InitializeGrammars();
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.HomePageSystemGrammar]);
        _sre.RecognizeAsync(RecognizeMode.Multiple);
    }


    public void AddAudioStateChangeEvent(Action<AudioState> audioStateChange)
    {
        _sre.AudioStateChanged += delegate { audioStateChange(_sre.AudioState); };
    }

    public void AddSpeechRecognizedEvent(Action<RecognitionResult> speechRecognized)
    {
        _sre.SpeechRecognized += delegate(object sender, SpeechRecognizedEventArgs args)
        {
            speechRecognized(args.Result);
        };
    }
    // TODO implement ResetSpeechRecognizedEventSubscriptions method (trudne)
    //Wymyślić sposób usuwania subskrypcji przy zmianie widoku
    // public void ResetSpeechRecognizedEventSubscriptions()
    // {
    //     
    // }

    private void InitializeGrammars()
    {
        _grammarsDict = new Dictionary<GrammarName, Grammar>();
        var homePageSystemGrammar = CreateHomePageSystemGrammar();
        var calendarGrammar1 = CreateCalendarGrammar("rootData1");
        var calendarGrammar2 = CreateCalendarGrammar("rootData2");
        var numberGrammar = CreateNumberGrammar();
        var roomSelectGrammar = CreateRoomSelectGrammar();

        _grammarsDict.Add(GrammarName.HomePageSystemGrammar, homePageSystemGrammar);
        _grammarsDict.Add(GrammarName.CalendarGrammar1, calendarGrammar1);
        _grammarsDict.Add(GrammarName.CalendarGrammar2, calendarGrammar2);
        _grammarsDict.Add(GrammarName.NumberGrammar, numberGrammar);
        _grammarsDict.Add(GrammarName.RoomSelectGrammar, roomSelectGrammar);
    }

    private Grammar CreateRoomSelectGrammar()
    {
        SrgsRule option = new SrgsRule("Opcja");
        
        SrgsOneOf options = new SrgsOneOf(new SrgsItem[]
        {
            new("Apartament Luxus"),
            new("Apartament Comfort"),
            new("Czytaj Apartament Luxus"),
            new("Czytaj Apartament Comfort"),
            new("Pomoc"),
            new("Wstecz")
        });
        option.Add(options);

        SrgsRule rootRoomSelect = new SrgsRule("rootRoomSelect");
        rootRoomSelect.Scope = SrgsRuleScope.Public;
        rootRoomSelect.Add(new SrgsRuleRef(option, "Opcja"));

        SrgsDocument docWeight = new SrgsDocument();
        docWeight.Root = rootRoomSelect;
        docWeight.Culture = new CultureInfo("pl-PL");
        docWeight.Rules.Add(new SrgsRule[]
            {rootRoomSelect, option}
        );

        Grammar gramatyka = new Grammar(docWeight, "rootRoomSelect");
        return gramatyka;
    }
    
    private Grammar CreateNumberGrammar()
    {
        SrgsRule rulenumber = new SrgsRule("Numer");

        SrgsOneOf numbers = new SrgsOneOf();
        for (int i = 0; i <= 20; i++)
        {
            numbers.Add(new SrgsItem($"{i}"));
        }

        rulenumber.Add(numbers);

        SrgsRule rootNumber = new SrgsRule("rootNumer");
        rootNumber.Scope = SrgsRuleScope.Public;
        rootNumber.Add(new SrgsRuleRef(rulenumber, "Numer"));


        SrgsDocument docWeight = new SrgsDocument();
        docWeight.Root = rootNumber;
        docWeight.Culture = new CultureInfo("pl-PL");
        docWeight.Rules.Add(new SrgsRule[]
            {rootNumber, rulenumber}
        );

        Grammar gramatyka = new Grammar(docWeight, "rootNumer");
        return gramatyka;
    }


    private Grammar CreateCalendarGrammar(string rootName)
    {
        SrgsRule ruleDay = new SrgsRule("Dzień");
        SrgsOneOf day = new SrgsOneOf();
        for (int i = 1; i <= 31; i++)
        {
            day.Add(new SrgsItem($"{i}"));
        }

        ruleDay.Add(day);


        SrgsRule ruleMonth = new SrgsRule("Miesiac");
        SrgsOneOf months = new SrgsOneOf(new SrgsItem[]
        {
            new("styczeń"),
            new("luty"),
            new("marzec"),
            new("kwiecień"),
            new("maj"),
            new("czerwiec"),
            new("lipiec"),
            new("sierpień"),
            new("wrzesień"),
            new("październik"),
            new("listopad"),
            new("grudzień")
        });
        ruleMonth.Add(months);


        SrgsRule ruleYear = new SrgsRule("Rok");
        SrgsOneOf year = new SrgsOneOf();
        for (int i = 1990; i <= 2050; i++)
        {
            year.Add(new SrgsItem($"{i}"));
        }

        ruleYear.Add(year);

        SrgsRule rootBirthDate = new SrgsRule(rootName);
        rootBirthDate.Scope = SrgsRuleScope.Public;

        rootBirthDate.Add(new SrgsItem(0, 1, "Wprowadź"));
        rootBirthDate.Add(new SrgsItem(0, 1, "date"));
        rootBirthDate.Add(new SrgsItem(0, 1, "date urodzenia"));
        rootBirthDate.Add(new SrgsRuleRef(ruleDay, "Dzien"));
        rootBirthDate.Add(new SrgsRuleRef(ruleMonth, "Miesiac"));
        rootBirthDate.Add(new SrgsRuleRef(ruleYear, "Rok"));
        rootBirthDate.Add(new SrgsItem(0, 1, "rok"));

        SrgsDocument docBirthDate = new SrgsDocument();
        docBirthDate.Root = rootBirthDate;
        docBirthDate.Culture = new CultureInfo("pl-PL");
        docBirthDate.Rules.Add(new SrgsRule[]
            {rootBirthDate, ruleDay, ruleMonth, ruleYear}
        );

        Grammar gramatyka = new Grammar(docBirthDate, rootName);
        return gramatyka;
    }

    private Grammar CreateHomePageSystemGrammar()
    {
        SrgsRule optionRule = new SrgsRule("Opcja");

        SrgsOneOf options = new SrgsOneOf(new SrgsItem[]
        {
            new("Data zameldowania"),
            new("Data wymeldowania"),
            new("Liczba osób"),
            new("Kontynuuj"),
            new("Reset"),
        });
        optionRule.Add(options);

        SrgsRule systemRule = new SrgsRule("homePageSystemGrammar");
        systemRule.Scope = SrgsRuleScope.Public;

        systemRule.Add(new SrgsItem(0, 1, "Wprowadz"));
        systemRule.Add(new SrgsItem(0, 1, "Pole"));
        systemRule.Add(new SrgsRuleRef(optionRule, "Opcja"));


        SrgsDocument systemGrammar = new SrgsDocument();
        systemGrammar.Root = systemRule;
        systemGrammar.Culture = new CultureInfo("pl-PL");

        systemGrammar.Rules.Add(systemRule, optionRule);

        Grammar gramatyka = new Grammar(systemGrammar, "homePageSystemGrammar");
        return gramatyka;
    }

    /*private Grammar CreateHomePageSystemGrammar()
    {
        GrammarBuilder grammarBuilder = new GrammarBuilder();
        Choices stopChoice = new Choices();
        stopChoice.Add("Data zamelodowania");
        stopChoice.Add("Data wymeldowania");
        stopChoice.Add("Liczba osób");
        stopChoice.Add("Kontyniuj");
        stopChoice.Add("Pomoc");
        grammarBuilder.Append(stopChoice);
        Grammar homePageSystemGrammar = new Grammar(grammarBuilder);
        return homePageSystemGrammar;
    }*/

    //TODO secure grammars on loading and deleting
    
    public void LoadNumberGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.NumberGrammar]);
    }

    public void LoadHomePageSystemGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.HomePageSystemGrammar]);
    }

    public void LoadCalendarGrammar1()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.CalendarGrammar1]);
    }

    public void LoadCalendarGrammar2()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.CalendarGrammar2]);
    }
    
    public void LoadRoomSelectGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.RoomSelectGrammar]);
    }

    public void UnloadCalendarGrammar1()
    {
        _sre.UnloadGrammar(_grammarsDict[GrammarName.CalendarGrammar1]);
    }

    public void UnloadAllGrammar()
    {
        _sre.UnloadAllGrammars();
    }

    private void UnloadGrammar(Grammar grammar)
    {
        if (_sre.Grammars.Contains(grammar))
        {
            _sre.UnloadGrammar(grammar);
        }
    }

    public AudioState GetAudioState()
    {
        return _sre.AudioState;
    }

    public void StopSRE()
    {
        _sre.RecognizeAsyncCancel();
        _sre.RecognizeAsyncStop();
        _tts.SpeakAsync("Wyłączono funkcję rozpoznawania mowy");
    }

    public void StartSRE()
    {
        _sre.RecognizeAsync(RecognizeMode.Multiple);
        _tts.SpeakAsync("Włączono funkcję rozpoznawania mowy");
    }
}