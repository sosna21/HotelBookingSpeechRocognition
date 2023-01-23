using System.Windows;
using Autofac;
using Autofac.Features.ResolveAnything;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.TTS;
using SWPSD_PROJEKT.UI.Stores;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IContainer Container { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            //autofac config
            var builder = new ContainerBuilder();
            builder.RegisterType<UnitOfWork>().SingleInstance();
            builder.RegisterType<SpeechRecognition>().SingleInstance();
            builder.RegisterType<SpeechSynthesis>().SingleInstance();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            Container = builder.Build();
            
            //multi view config
            NavigationStore navigationStore = new NavigationStore();
            RoomStore roomStore = new RoomStore();
            ReservationDataStore reservationDataStore = new ReservationDataStore();
            navigationStore.CurrentViewModel = new RoomSelectViewModel(navigationStore, roomStore, reservationDataStore);
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(navigationStore)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}