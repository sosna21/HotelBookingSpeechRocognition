using SWPSD_PROJEKT.TTS;

namespace SWPSD_PROJEKT.DialogDriver;

public class DialogControl : IDialogControl
{
    private readonly SpeechSynthesis TTS;

    public DialogControl(SpeechSynthesis synthesis)
    {
        TTS = synthesis;
    }

    public void UseTTS()
    {
        TTS.SpeakAsync("Powiedz te wiadomość");
    }
}