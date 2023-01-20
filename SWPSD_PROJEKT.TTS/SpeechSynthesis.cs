using Microsoft.Speech.Synthesis;

namespace SWPSD_PROJEKT.TTS;

public class SpeechSynthesis
{
    private SpeechSynthesizer TTS { get; }

    public SpeechSynthesis()
    {
        TTS = new();
        TTS.SetOutputToDefaultAudioDevice();
        TTS.SelectVoice("Microsoft Server Speech Text to Speech Voice (pl-PL, Paulina)");
    }
    
    public void Speak(string text)
    {
        TTS.Speak(text);
    }
    public void SpeakAsync(string text)
    {
        TTS.SpeakAsync(text);
    }
    public void SpeakAsyncLowConfidence()
    {
        TTS.SpeakAsync("Proszę powtórzyć");
    }
}