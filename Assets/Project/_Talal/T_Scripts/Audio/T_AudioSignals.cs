using System;
using UnityEngine;

/// <summary>Music track the Audio Manager should play. Requested by Game-flow / menus.</summary>
public enum MusicId
{
    MainMenu,
    Pause,
    Hub,
    Act1,
    Act2,
    Act3,
    Results,
    WinEnding,
    LoseEnding
}

/// <summary>A discrete audience reaction. Triggered by the Crowd-reaction system (Tier 2).</summary>
public enum CrowdReactionId
{
    Boo1,
    Boo2,
    Cheer1,
    Cheer2,
    Applause,
    Gasp,
    Laugh
}

/// <summary>A UI feedback sound. Raised by the UI system.</summary>
public enum UiSoundId
{
    Navigate,
    Confirm,
    Back,
    PauseOpen,
    PauseClose,
    StarReveal
}

/// <summary>An Impresario (clown-wizard) vocal sting.</summary>
public enum ImpresarioId
{
    Stinger,
    Laugh
}

/// <summary>Mixer volume categories; names match the AudioMixer's exposed parameters.</summary>
public enum AudioCategory
{
    Master,
    Music,
    Sfx,
    Crowd,
    Ui
}

/// <summary>
/// Static event hub for *audio-request* signals — "play this sound / set this audio state" — as opposed to
/// the gameplay facts on <see cref="T_GameSignals"/>. Raised by UI, Game-flow, the Crowd system, Dialogue and
/// the Impresario; consumed by the Audio Manager. Same conventions as <see cref="T_GameSignals"/>: subscribe
/// in OnEnable, unsubscribe in OnDisable; <see cref="ResetAll"/> clears everything on Play.
/// </summary>
public static class T_AudioSignals
{
    /// <summary>Play (crossfade to) a music track.</summary>
    public static event Action<MusicId> MusicRequested;
    /// <summary>Stop the current music.</summary>
    public static event Action MusicStopRequested;
    /// <summary>Current crowd mood 0–1; drives the ambient bed blend.</summary>
    public static event Action<float> CrowdMoodChanged;
    /// <summary>Play a one-shot audience reaction.</summary>
    public static event Action<CrowdReactionId> CrowdReactionRequested;
    /// <summary>Play a UI feedback sound.</summary>
    public static event Action<UiSoundId> UiSoundRequested;
    /// <summary>Play one dialogue text blip.</summary>
    public static event Action DialogueBlipRequested;
    /// <summary>Play an Impresario vocal sting.</summary>
    public static event Action<ImpresarioId> ImpresarioRequested;
    /// <summary>Set a mixer category volume, 0–1 linear.</summary>
    public static event Action<AudioCategory, float> VolumeChangeRequested;

    public static void RequestMusic(MusicId track) => MusicRequested?.Invoke(track);
    public static void StopMusic() => MusicStopRequested?.Invoke();
    public static void SetCrowdMood(float mood01) => CrowdMoodChanged?.Invoke(mood01);
    public static void PlayCrowdReaction(CrowdReactionId reaction) => CrowdReactionRequested?.Invoke(reaction);
    public static void PlayUi(UiSoundId sound) => UiSoundRequested?.Invoke(sound);
    public static void PlayDialogueBlip() => DialogueBlipRequested?.Invoke();
    public static void PlayImpresario(ImpresarioId sound) => ImpresarioRequested?.Invoke(sound);
    public static void SetVolume(AudioCategory category, float volume01) => VolumeChangeRequested?.Invoke(category, volume01);

    /// <summary>Clears every subscription on Play (guards against stale handlers if domain reload is off).</summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetAll()
    {
        MusicRequested = null;
        MusicStopRequested = null;
        CrowdMoodChanged = null;
        CrowdReactionRequested = null;
        UiSoundRequested = null;
        DialogueBlipRequested = null;
        ImpresarioRequested = null;
        VolumeChangeRequested = null;
    }
}
