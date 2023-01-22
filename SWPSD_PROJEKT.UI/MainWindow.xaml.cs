using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Autofac;
using FontAwesome.WPF;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using SWPSD_PROJEKT.ASR;
using SWPSD_PROJEKT.DialogDriver;
using SWPSD_PROJEKT.TTS;

namespace SWPSD_PROJEKT.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private SpeechRecognition _sre;
        private SpeechSynthesis _tts;
        private DialogControl _dialogControl;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var container = (Application.Current as App)!.Container;
            var test = container.Resolve<Test>();
            test.AddRoom().ConfigureAwait(false).GetAwaiter().GetResult();

            // var container = (Application.Current as App)!.Container;
            // _dialogControl = container.Resolve<DialogControl>();
            // _sre = container.Resolve<SpeechRecognition>();
            // _tts = container.Resolve<SpeechSynthesis>();
            // _sre.AddAudioStateChangeEvent(SRE_AudioStateChanged);
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
            if (_sre.GetAudioState() == AudioState.Stopped)
            {
                _sre.StartSRE();
                SpeechIcon.Icon = FontAwesomeIcon.Microphone;
            }
            else
            {
                _sre.StopSRE();
                SpeechIcon.Icon = FontAwesomeIcon.MicrophoneSlash;
            }
        }

        private void SynthesisIcon_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (_tts.GetState() == SynthesizerState.Paused)
            {
                _tts.ResumeSynthesis();
                SynthesisIcon.Foreground = (Brush) new BrushConverter().ConvertFrom("#FFFF7400");
            }
            else
            {
                _tts.TerminateSynthesis();
                _tts.StopSynthesis();
                SynthesisIcon.Foreground = Brushes.Red;
            }
        }
    }
}