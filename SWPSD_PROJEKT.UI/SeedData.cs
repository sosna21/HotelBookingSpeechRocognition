using System.Threading.Tasks;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.DialogDriver.Model;

namespace SWPSD_PROJEKT.UI;

public class SeedData
{
    private readonly UnitOfWork _unitOfWork;

    public SeedData(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void AddInitialData()
    {
        _unitOfWork.Repository<Room>().Add(new Room
        {
            Name = "Pokój dwuosobowy", Capacity = 2,
            Description =
                "Pokój hotelowy jest mały i skromny.\n Posiada on łóżka z miękkimi materacami, mały stolik z krzesłem, szafkę oraz telewizor z dostępem do kilku kanałów. Pokój nie jest klimatyzowany, jednak posiada okno, które można otworzyć w celu wentylacji. Na wyposażeniu jest także małe biurko, idealne do pisania notatek. Łazienka jest skromna, posiada prysznic, umywalkę i WC. Ręczniki są dostępne tylko na życzenie. Dostęp do internetu jest dostępny tylko w holu hotelowym za dodatkową opłatą.",
            PricePerNight = 125
        });
        _unitOfWork.Repository<Room>().Add(new Room
        {
            Name = "Pokój trzyosobowy", Capacity = 3,
            Description =
                "Pokój hotelowy jest skromny ale czysty i przytulny.\n Posiada on łóżka z miękkimi materacami, mały stolik z krzesłem, szafkę oraz telewizor z dostępem do podstawowych kanałów. Pokój jest nieklimatyzowany ale posiada okno, dzięki któremu jest dobrze nasłoneczniony. Na wyposażeniu jest także małe biurko, idealne do pisania notatek. Łazienka jest wyposażona w prysznic, umywalkę WC oraz ręczniki. Bezpłatny dostęp do internetu jest dostępny w całym pokoju.",
            PricePerNight = 225
        });
        _unitOfWork.Repository<Room>().Add(new Room
        {
            Name = "Pokój czteroosobowy", Capacity = 4,
            Description =
                "Pokój hotelowy jest przestronny i elegancki.\n Posiada on łóżka z wysokiej jakości pościelą, stolik z krzesłami, szafę oraz telewizor z dostępem do różnych kanałów. Pokój jest klimatyzowany i posiada duże okna, z których roztacza się piękny widok na miasto. Na wyposażeniu jest również biurko, idealne do pracy lub napisania pocztówki. Łazienka jest w pełni wyposażona, posiada prysznic, umywalkę, WC oraz ręczniki. Bezpłatny bezprzewodowy dostęp do internetu jest dostępny w całym pokoju.",
            PricePerNight = 325
        });
        _unitOfWork.Repository<Room>().Add(new Room
        {
            Name = "Apartament czteroosobowy", Capacity = 4,
            Description =
                "Apartament hotelowy jest przestronny i luksusowy.\n Posiada on łóżka z satynową pościelą, elegancki stolik z krzesłami, szafę oraz telewizor z dostępem do różnych kanałów z płatnymi filmami i programami. Apartament jest klimatyzowany i posiada panoramiczne okna z widokiem na morze. Na wyposażeniu jest również sejf i zestaw do parzenia kawy i herbaty. Łazienka jest w pełni wyposażona, posiada wanna z hydromasażem, suszarkę do włosów, ręczniki frotte oraz kosmetyki. Bezpłatny bezprzewodowy dostęp do internetu jest dostępny w całym apatamencie.",
            PricePerNight = 525
        });
        
        _unitOfWork.Repository<Facility>().Add(new Facility {Name = "DoubleBed", Price = 100});
        _unitOfWork.Repository<Facility>().Add(new Facility {Name = "Breakfast", Price = 50});
        _unitOfWork.Repository<Facility>().Add(new Facility {Name = "Pets", Price = 100});
        _unitOfWork.Repository<Facility>().Add(new Facility {Name = "AlcoholBar", Price = 100});
        _unitOfWork.Repository<Facility>().Add(new Facility {Name = "ExtraBedForChild", Price = 100});
        _unitOfWork.Complete();
    }
    
}