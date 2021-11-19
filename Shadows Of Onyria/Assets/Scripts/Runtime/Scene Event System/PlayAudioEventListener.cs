using DoaT;

public sealed class PlayAudioEventListener : BaseSceneEventListener
{
    public AudioCue cue;

    public override void OnEventTriggered(params object[] parameters)
    {
        if (!CanReact) return;
        AudioSystem.PlayCue(cue);
    }
}
