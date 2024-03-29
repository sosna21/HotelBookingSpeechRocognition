﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using SWPSD_PROJEKT.TTS;

namespace SWPSD_PROJEKT.ASR;

public class SpeechRecognition
{
    private readonly SpeechRecognitionEngine _sre;
    private Dictionary<GrammarName, Grammar> _grammarsDict;
    private readonly SpeechSynthesis _tts;


    public SpeechRecognition(SpeechSynthesis tts)
    {
        _tts = tts;
        _sre = new SpeechRecognitionEngine(new CultureInfo("pl-PL"));
        _sre.SetInputToDefaultAudioDevice();

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
    private void InitializeGrammars()
    {
        _grammarsDict = new Dictionary<GrammarName, Grammar>();
        var homePageSystemGrammar = CreateHomePageSystemGrammar();
        var calendarGrammar1 = CreateCalendarGrammar("rootData1");
        var calendarGrammar2 = CreateCalendarGrammar("rootData2");
        var roomSelectGrammar = CreateRoomSelectGrammar();
        var roomDescriptionGrammar = CreateRoomDescriptionGrammar();
        var nameSelectGrammar = CreateNameSelectGrammar();
        var surnameSelectGrammar = CreateSurnameGrammar();
        var telephoneGrammar = CreateTelGrammar();
        var userDataGrammar = CreateUserDataGrammar();
        var facilityGrammar = CreateFacilitiesGrammar();
        var bedSelectGrammar = CreateBedSelectGrammar();
        var creditCardGrammar = CreateCreditCardGrammar();
        var summaryOrderGrammar = CreateSummaryOrderGrammar();

        _grammarsDict.Add(GrammarName.HomePageSystemGrammar, homePageSystemGrammar);
        _grammarsDict.Add(GrammarName.CalendarGrammar1, calendarGrammar1);
        _grammarsDict.Add(GrammarName.CalendarGrammar2, calendarGrammar2);
        _grammarsDict.Add(GrammarName.RoomSelectGrammar, roomSelectGrammar);
        _grammarsDict.Add(GrammarName.RoomDescriptionGrammar, roomDescriptionGrammar);
        _grammarsDict.Add(GrammarName.NameSelectGrammar, nameSelectGrammar);
        _grammarsDict.Add(GrammarName.SurnameSelectGrammar, surnameSelectGrammar);
        _grammarsDict.Add(GrammarName.TelephoneGrammar, telephoneGrammar);
        _grammarsDict.Add(GrammarName.UserDataGrammar, userDataGrammar);
        _grammarsDict.Add(GrammarName.FacilityGrammar, facilityGrammar);
        _grammarsDict.Add(GrammarName.BedSelectGrammar, bedSelectGrammar);
        _grammarsDict.Add(GrammarName.CreditCardGrammar, creditCardGrammar);
        _grammarsDict.Add(GrammarName.SummaryOrderGrammar, summaryOrderGrammar);
    }

    private Grammar CreateCreditCardGrammar()
    {
        SrgsRule ruleLiczebnik = GetSrgsRuleNumbers();

        SrgsRule rootRule = new SrgsRule("rootCreditCard");
        rootRule.Scope = SrgsRuleScope.Public;

        for (int i = 0; i < 16; i++)
        {
            rootRule.Add(new SrgsRuleRef(ruleLiczebnik, $"LICZBA{i}"));
        }

        SrgsDocument docCreditCardNumber = new SrgsDocument();
        docCreditCardNumber.Root = rootRule;
        docCreditCardNumber.Culture = new CultureInfo("pl-PL");
        docCreditCardNumber.Rules.Add(new SrgsRule[]
            {
                rootRule,
                ruleLiczebnik
            }
        );

        Grammar gramatyka = new Grammar(docCreditCardNumber, "rootCreditCard");
        return gramatyka;
    }

    
    private Grammar CreateBedSelectGrammar()
    {
        SrgsRule bedOption = new SrgsRule("BedOption");
        
        SrgsOneOf bedType = new SrgsOneOf(new SrgsItem[]
        {
            new SrgsItem("pojedyncze"),
            new SrgsItem("podwójne"),
        });
        bedOption.Add(bedType);

        SrgsRule rootSelectBed = new SrgsRule("rootSelectBed");
        rootSelectBed.Scope = SrgsRuleScope.Public;
        rootSelectBed.Add(new SrgsRuleRef(bedOption, "BedOption"));

        SrgsDocument docFacility = new SrgsDocument();
        docFacility.Root = bedOption;
        docFacility.Culture = new CultureInfo("pl-PL");
        docFacility.Rules.Add(new SrgsRule[]
        {
            rootSelectBed, bedOption
        });

        Grammar grammar = new Grammar(docFacility, "rootSelectBed");
        return grammar;
    }
    
    private Grammar CreateFacilitiesGrammar()
    {
        SrgsRule selectOption = new SrgsRule("FacilityOption");
        // selectOption.Scope = SrgsRuleScope.Public;

        SrgsOneOf facitility = new SrgsOneOf(new SrgsItem[]
        {
            new SrgsItem("Śniadanie tak"),
            new SrgsItem("Śniadanie nie"),
            new SrgsItem("Zwierzęta tak"),
            new SrgsItem("Zwierzęta nie"),
            new SrgsItem("Barek z alkoholem tak"),
            new SrgsItem("Barek z alkoholem nie"),
            new SrgsItem("Dostawka dla dziecka tak"),
            new SrgsItem("Dostawka dla dziecka nie"),
        });
        selectOption.Add(facitility);

        SrgsRule rootSelectFacility = new SrgsRule("rootSelectFacility");
        rootSelectFacility.Scope = SrgsRuleScope.Public;
        rootSelectFacility.Add(new SrgsRuleRef(selectOption, "FacilityOption"));

        SrgsDocument docFacility = new SrgsDocument();
        docFacility.Root = selectOption;
        docFacility.Culture = new CultureInfo("pl-PL");
        docFacility.Rules.Add(new SrgsRule[]
        {
            rootSelectFacility, selectOption
        });

        Grammar grammar = new Grammar(docFacility, "rootSelectFacility");
        return grammar;
    }


    private Grammar CreateUserDataGrammar()
    {
        SrgsRule userDataSelectOption = new SrgsRule("UserDataSelect");
        SrgsOneOf field = new SrgsOneOf(new SrgsItem[]
        {
            new SrgsItem("Imię"),
            new SrgsItem("Nazwisko"),
            new SrgsItem("Telefon"),
            new SrgsItem("Numer karty płatniczej"),
            new SrgsItem("Wyczyść dane"),
            new SrgsItem("Wybierz łóżko"),
            new SrgsItem("Wybierz udogodnienia"),
            new SrgsItem("Dalej"),
            new SrgsItem("Wstecz"),
            new SrgsItem("Pomoc")
        });
        userDataSelectOption.Add(field);

        SrgsRule rootUserDataSelect = new SrgsRule("rootUserDataSelect");
        rootUserDataSelect.Scope = SrgsRuleScope.Public;
        rootUserDataSelect.Add(new SrgsRuleRef(userDataSelectOption, "UserDataSelect"));

        SrgsDocument docUserDataOpction = new SrgsDocument();
        docUserDataOpction.Root = userDataSelectOption;
        docUserDataOpction.Culture = new CultureInfo("pl-PL");
        docUserDataOpction.Rules.Add(userDataSelectOption, rootUserDataSelect);

        Grammar gramatyka = new Grammar(docUserDataOpction, "rootUserDataSelect");
        return gramatyka;
    }

    private Grammar CreateTelGrammar()
    {
        SrgsRule ruleLiczebnik = GetSrgsRuleNumbers();

        SrgsRule rootRule = new SrgsRule("rootTel");
        rootRule.Scope = SrgsRuleScope.Public;

        for (int i = 0; i < 9; i++)
        {
            rootRule.Add(new SrgsRuleRef(ruleLiczebnik, $"LICZBA{i}"));
        }

        SrgsDocument docTel = new SrgsDocument();
        docTel.Root = rootRule;
        docTel.Culture = new CultureInfo("pl-PL");
        docTel.Rules.Add(new SrgsRule[]
            {
                rootRule,
                ruleLiczebnik
            }
        );

        Grammar gramatyka = new Grammar(docTel, "rootTel");
        return gramatyka;
    }

    private SrgsRule GetSrgsRuleNumbers()
    {
        SrgsRule ruleLiczebnik = new SrgsRule("Liczba");
        SrgsOneOf liczebnik = new SrgsOneOf(new SrgsItem[]
        {
            new SrgsItem("zero"),
            new SrgsItem("jeden"),
            new SrgsItem("dwa"),
            new SrgsItem("trzy"),
            new SrgsItem("cztery"),
            new SrgsItem("pięć"),
            new SrgsItem("sześć"),
            new SrgsItem("siedem"),
            new SrgsItem("osiem"),
            new SrgsItem("dziewięć")
        });
        ruleLiczebnik.Add(liczebnik);
        return ruleLiczebnik;
    }

    private Grammar CreateSurnameGrammar()
    {
        SrgsRule ruleSurname = new SrgsRule("rootSurnameSelect");
        ruleSurname.Scope = SrgsRuleScope.Public;

        var surnames = File.ReadAllLines("Assets/Nazwiska.txt");

        SrgsOneOf surname = new SrgsOneOf(surnames.Select(n => new SrgsItem(n)).ToArray());
        ruleSurname.Add(surname);

        SrgsDocument docSurnameSelect = new SrgsDocument();
        docSurnameSelect.Root = ruleSurname;
        docSurnameSelect.Culture = new CultureInfo("pl-PL");
        docSurnameSelect.Rules.Add(ruleSurname);

        Grammar gramatyka = new Grammar(docSurnameSelect, "rootSurnameSelect");
        return gramatyka;
    }

    private Grammar CreateNameSelectGrammar()
    {
        SrgsRule ruleName = new SrgsRule("rootNameSelect");
        ruleName.Scope = SrgsRuleScope.Public;

        var names = File.ReadAllLines("Assets/Imiona.txt");

        SrgsOneOf name = new SrgsOneOf(names.Select(n => new SrgsItem(n)).ToArray());
        ruleName.Add(name);

        SrgsDocument docNameSelect = new SrgsDocument();
        docNameSelect.Root = ruleName;
        docNameSelect.Culture = new CultureInfo("pl-PL");
        docNameSelect.Rules.Add(ruleName);

        Grammar gramatyka = new Grammar(docNameSelect, "rootNameSelect");
        return gramatyka;
    }

    private Grammar CreateRoomSelectGrammar()
    {
        SrgsRule option = new SrgsRule("Opcja");

        SrgsOneOf options = new SrgsOneOf(new SrgsItem[]
        {
            new("Pokój dwuosobowy"),
            new("Pokój trzyosobowy"),
            new("Pokój czteroosobowy"),
            new("Apartament czteroosobowy"),
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
            { rootRoomSelect, option }
        );

        Grammar gramatyka = new Grammar(docWeight, "rootRoomSelect");
        return gramatyka;
    }

    private Grammar CreateRoomDescriptionGrammar()
    {
        SrgsRule option = new SrgsRule("Opcja");

        SrgsOneOf options = new SrgsOneOf(new SrgsItem[]
        {
            new("Czytaj opis"),
            new("Zarezerwuj"),
            new("Pomoc"),
            new("Wstecz")
        });
        option.Add(options);

        SrgsRule rootRoomDescription = new SrgsRule("rootRoomDescription");
        rootRoomDescription.Scope = SrgsRuleScope.Public;
        rootRoomDescription.Add(new SrgsRuleRef(option, "Opcja"));

        SrgsDocument docWeight = new SrgsDocument();
        docWeight.Root = rootRoomDescription;
        docWeight.Culture = new CultureInfo("pl-PL");
        docWeight.Rules.Add(new SrgsRule[]
            { rootRoomDescription, option }
        );

        Grammar gramatyka = new Grammar(docWeight, "rootRoomDescription");
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
            { rootNumber, rulenumber }
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
            { rootBirthDate, ruleDay, ruleMonth, ruleYear }
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
            new("Kontynuuj"),
            new("Wyczyść"),
            new("Wstecz"),
            new("Pomoc")
        });
        optionRule.Add(options);

        SrgsRule systemRule = new SrgsRule("homePageSystemGrammar");
        systemRule.Scope = SrgsRuleScope.Public;

        systemRule.Add(new SrgsItem(0, 1, "Wprowadź"));
        systemRule.Add(new SrgsItem(0, 1, "Pole"));
        systemRule.Add(new SrgsRuleRef(optionRule, "Opcja"));


        SrgsDocument systemGrammar = new SrgsDocument();
        systemGrammar.Root = systemRule;
        systemGrammar.Culture = new CultureInfo("pl-PL");

        systemGrammar.Rules.Add(systemRule, optionRule);

        Grammar gramatyka = new Grammar(systemGrammar, "homePageSystemGrammar");
        return gramatyka;
    }
    private Grammar CreateSummaryOrderGrammar()
    {
        SrgsRule option = new SrgsRule("Opcja");

        SrgsOneOf options = new SrgsOneOf(new SrgsItem[]
        {
            new("Powrót do głównej"),
            new("Pomoc"),
        });
        option.Add(options);

        SrgsRule rootSummaryOrder = new SrgsRule("rootSummaryOrder");
        rootSummaryOrder.Scope = SrgsRuleScope.Public;
        rootSummaryOrder.Add(new SrgsRuleRef(option, "Opcja"));

        SrgsDocument docWeight = new SrgsDocument();
        docWeight.Root = rootSummaryOrder;
        docWeight.Culture = new CultureInfo("pl-PL");
        docWeight.Rules.Add(new SrgsRule[]
            { rootSummaryOrder, option }
        );

        Grammar gramatyka = new Grammar(docWeight, "rootSummaryOrder");
        return gramatyka;
    }

    //TODO secure grammars on loading and deleting

    public void LoadFacilitiesGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.FacilityGrammar]);
    }
    
    public void UnloadFacilitiesGrammar()
    {
        _sre.UnloadGrammar(_grammarsDict[GrammarName.FacilityGrammar]);
    }
    
    public void LoadUserDataGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.UserDataGrammar]);
    }

    public void LoadNameSelectGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.NameSelectGrammar]);
    }

    public void UnloadNameSelectGrammar()
    {
        _sre.UnloadGrammar(_grammarsDict[GrammarName.NameSelectGrammar]);
    }
    public void LoadSurnameSelectGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.SurnameSelectGrammar]);
    }
    
    public void UnloadSurnameSelectGrammar()
    {
        _sre.UnloadGrammar(_grammarsDict[GrammarName.SurnameSelectGrammar]);
    }

    public void LoadTelephoneGrammar()
    {
        _sre.LoadGrammarAsync((_grammarsDict[GrammarName.TelephoneGrammar]));
    }
    
    public void UnloadTelephoneGrammar()
    {
        _sre.UnloadGrammar((_grammarsDict[GrammarName.TelephoneGrammar]));
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

    public void LoadRoomDescriptionGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.RoomDescriptionGrammar]);
    }
    
    public void LoadCreditCardGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.CreditCardGrammar]);
    }
    
    public void UnloadCreditCardGrammar()
    {
        _sre.UnloadGrammar(_grammarsDict[GrammarName.CreditCardGrammar]);
    }

    public void LoadBedSelectGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.BedSelectGrammar]);
    }

    public void UnloadBedSelectGrammar()
    {
        _sre.UnloadGrammar(_grammarsDict[GrammarName.BedSelectGrammar]);
    }
    
    public void LoadSummaryOrderGrammar()
    {
        _sre.LoadGrammarAsync(_grammarsDict[GrammarName.SummaryOrderGrammar]);
    }

    public void UnloadSummaryOrderGrammar()
    {
        _sre.UnloadGrammar(_grammarsDict[GrammarName.SummaryOrderGrammar]);
    }
    
    public void UnloadAllGrammar()
    {
        _sre.UnloadAllGrammars();
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