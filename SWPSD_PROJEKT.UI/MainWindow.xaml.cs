using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Autofac;
using FontAwesome.WPF;
using Microsoft.Speech.Recognition;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.DialogDriver;

namespace SWPSD_PROJEKT.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private SpeechRecognition _SRE;
        private DialogControl _dialogControl;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // var container = (Application.Current as App)!.Container;
            // var test = container.Resolve<Test>();
            // test.AddRoom().ConfigureAwait(false).GetAwaiter().GetResult();

            var container = (Application.Current as App)!.Container;
            _dialogControl = container.Resolve<DialogControl>();
            _SRE = container.Resolve<SpeechRecognition>();
            _SRE.AddAudioStateChangeEvent(SRE_AudioStateChanged);
        }
        
        private void SRE_AudioStateChanged(AudioState audioState)
        {
            if (audioState == AudioState.Speech)
            {
                SpeechIcon.Foreground = Brushes.Green;
            }
            else if (audioState == AudioState.Silence)
                SpeechIcon.Foreground = (Brush) new BrushConverter().ConvertFrom("#FFFF7400");
            else
                SpeechIcon.Foreground = Brushes.Red;
        }

        private void SpeechIcon_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (_SRE.GetAudioState() == AudioState.Stopped)
            {
                _SRE.StartSRE();
                SpeechIcon.Icon = FontAwesomeIcon.Microphone;
            }
            else
            {
                _SRE.StopSRE();
                SpeechIcon.Icon = FontAwesomeIcon.MicrophoneSlash;
            }
        }
    }
}