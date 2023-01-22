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

    public SynthesizerState GetState()
    {
        return TTS.State;
    }
    public void Speak(string text)
    {
        if(TTS.State == SynthesizerState.Paused) return;
        TTS.Speak(text);
    }
    public void SpeakAsync(string text)
    {
        if(TTS.State == SynthesizerState.Paused) return;
        TTS.SpeakAsync(text);
    }
    public void SpeakAsyncLowConfidence()
    {
        TTS.SpeakAsync("Proszę powtórzyć");
    }
    public void TerminateSynthesis()
    {
        TTS.SpeakAsyncCancelAll();
    }
    public void StopSynthesis()
    {
        TTS.Pause();
    }
    
    public void ResumeSynthesis()
    {
        TTS.Resume();
    }

}