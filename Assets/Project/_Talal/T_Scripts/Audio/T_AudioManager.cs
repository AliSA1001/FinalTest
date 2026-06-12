using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>Identifier for a one-shot sound effect in the Audio Manager's library.</summary>
public enum SfxId
{
    PerfectParry,
    NormalParry,
    PlayerDamage,
    EnemyDamage,
    EnemyDeath,
    PlayerDeath,
    EnemyTelegraph,
    PlayerSwing,
    Pickup,
    Jump,
    Interact,
    JokerSign,
    ComboUp,
    ScoreTick,
    EnergyBoltFire,
    RadialPulse,
    ImpresarioStinger,
    ImpresarioLaugh,
    DialogueBlip
}

/// <summary>A playable sound: one or more interchangeable clips plus playback settings. Filled in the Inspector.</summary>
[System.Serializable]
public class T_Sound
{
    public AudioClip[] variants;
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Random pitch is chosen between x and y. Leave both at 1 for no variation.")]
    public Vector2 pitchRange = new Vector2(1f, 1f);
}

// Inspector-friendly id→sound pairings (Unity can't serialize dictionaries directly).
[System.Serializable] public class SfxEntry { public SfxId id; public T_Sound sound; }
[System.Serializable] public class MusicEntry { public MusicId id; public T_Sound sound; }
[System.Serializable] public class CrowdEntry { public CrowdReactionId id; public T_Sound sound; }
[System.Serializable] public class UiEntry { public UiSoundId id; public T_Sound sound; }
[System.Serializable] public class SurfaceEntry { public SurfaceType surface; public T_Sound footstep; public T_Sound land; }

/// <summary>
/// Audio Manager (Tier-0 foundation) for "Act to Wish". One persistent, event-driven service: it subscribes
/// to <see cref="T_GameSignals"/> (gameplay facts) and <see cref="T_AudioSignals"/> (audio requests) and
/// plays the matching clips. Nothing calls it directly. Clips live in enum-keyed lists filled in the
/// Inspector; volume runs through an AudioMixer. AudioSources are created at runtime, so the only wiring is
/// the mixer, its groups, and the clip lists.
/// </summary>
public class T_AudioManager : MonoBehaviour
{
    [Header("Mixer (wire these)")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioMixerGroup _musicGroup;
    [SerializeField] private AudioMixerGroup _sfxGroup;
    [SerializeField] private AudioMixerGroup _crowdGroup;
    [SerializeField] private AudioMixerGroup _uiGroup;

    [Header("Exposed mixer volume parameters")]
    [SerializeField] private string _masterParam = "MasterVol";
    [SerializeField] private string _musicParam = "MusicVol";
    [SerializeField] private string _sfxParam = "SfxVol";
    [SerializeField] private string _crowdParam = "CrowdVol";
    [SerializeField] private string _uiParam = "UiVol";

    [Header("Clip library")]
    [SerializeField] private SfxEntry[] _sfx;
    [SerializeField] private MusicEntry[] _music;
    [SerializeField] private CrowdEntry[] _crowd;
    [SerializeField] private UiEntry[] _ui;
    [SerializeField] private SurfaceEntry[] _surfaces;
    [SerializeField] private T_Sound _crowdBedCalm;
    [SerializeField] private T_Sound _crowdBedRiled;

    [Header("Tuning")]
    [SerializeField] private float _musicFadeSeconds = 1.5f;
    [Range(0f, 1f)] [SerializeField] private float _bedVolume = 0.7f;
    [SerializeField] private int _sfxVoices = 8;

    // ----- runtime -----
    private static T_AudioManager _instance;

    private Dictionary<SfxId, T_Sound> _sfxMap;
    private Dictionary<MusicId, T_Sound> _musicMap;
    private Dictionary<CrowdReactionId, T_Sound> _crowdMap;
    private Dictionary<UiSoundId, T_Sound> _uiMap;
    private Dictionary<SurfaceType, SurfaceEntry> _surfaceMap;

    private AudioSource _musicA, _musicB, _activeMusic;
    private AudioSource _bedCalm, _bedRiled, _crowdOneShot, _uiSource, _dialogueSource;
    private AudioSource[] _sfxSources;
    private int _sfxIndex;

    private Coroutine _musicFade;
    private MusicId _currentMusic;
    private bool _hasMusic;
    private MusicId _resumeMusic;
    private float _resumeTime;
    private bool _hasResume;

    private int _lastCombo;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        BuildMaps();
        CreateSources();
    }

    private void Start()
    {
        LoadVolumes();
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    private void OnEnable()
    {
        if (_instance != null && _instance != this) return; // duplicate about to be destroyed

        T_GameSignals.ParryLanded += OnParry;
        T_GameSignals.PlayerDamaged += OnPlayerDamaged;
        T_GameSignals.PlayerDied += OnPlayerDied;
        T_GameSignals.EnemyKilled += OnEnemyKilled;
        T_GameSignals.EnemyDamaged += OnEnemyDamaged;
        T_GameSignals.CollectableCollected += OnPickup;
        T_GameSignals.ComboChanged += OnComboChanged;
        T_GameSignals.ScoreEventLanded += OnScoreEvent;
        T_GameSignals.EnemyAttackTelegraph += OnTelegraph;
        T_GameSignals.PlayerAttackSwing += OnSwing;
        T_GameSignals.PlayerJumped += OnJump;
        T_GameSignals.PlayerLanded += OnLand;
        T_GameSignals.Footstep += OnFootstep;
        T_GameSignals.InteractionPerformed += OnInteract;
        T_GameSignals.JokerCardSigned += OnJokerSigned;
        T_GameSignals.EnergyBoltFired += OnBoltFired;
        T_GameSignals.RadialPulseFired += OnPulse;

        T_AudioSignals.MusicRequested += PlayMusic;
        T_AudioSignals.MusicStopRequested += StopMusic;
        T_AudioSignals.CrowdMoodChanged += SetBed;
        T_AudioSignals.CrowdReactionRequested += OnCrowdReaction;
        T_AudioSignals.UiSoundRequested += OnUi;
        T_AudioSignals.DialogueBlipRequested += OnDialogueBlip;
        T_AudioSignals.ImpresarioRequested += OnImpresario;
        T_AudioSignals.VolumeChangeRequested += SetVolume;
    }

    private void OnDisable()
    {
        if (_instance != this) return;

        T_GameSignals.ParryLanded -= OnParry;
        T_GameSignals.PlayerDamaged -= OnPlayerDamaged;
        T_GameSignals.PlayerDied -= OnPlayerDied;
        T_GameSignals.EnemyKilled -= OnEnemyKilled;
        T_GameSignals.EnemyDamaged -= OnEnemyDamaged;
        T_GameSignals.CollectableCollected -= OnPickup;
        T_GameSignals.ComboChanged -= OnComboChanged;
        T_GameSignals.ScoreEventLanded -= OnScoreEvent;
        T_GameSignals.EnemyAttackTelegraph -= OnTelegraph;
        T_GameSignals.PlayerAttackSwing -= OnSwing;
        T_GameSignals.PlayerJumped -= OnJump;
        T_GameSignals.PlayerLanded -= OnLand;
        T_GameSignals.Footstep -= OnFootstep;
        T_GameSignals.InteractionPerformed -= OnInteract;
        T_GameSignals.JokerCardSigned -= OnJokerSigned;
        T_GameSignals.EnergyBoltFired -= OnBoltFired;
        T_GameSignals.RadialPulseFired -= OnPulse;

        T_AudioSignals.MusicRequested -= PlayMusic;
        T_AudioSignals.MusicStopRequested -= StopMusic;
        T_AudioSignals.CrowdMoodChanged -= SetBed;
        T_AudioSignals.CrowdReactionRequested -= OnCrowdReaction;
        T_AudioSignals.UiSoundRequested -= OnUi;
        T_AudioSignals.DialogueBlipRequested -= OnDialogueBlip;
        T_AudioSignals.ImpresarioRequested -= OnImpresario;
        T_AudioSignals.VolumeChangeRequested -= SetVolume;
    }

    // ---------- setup ----------

    private void BuildMaps()
    {
        _sfxMap = new Dictionary<SfxId, T_Sound>();
        if (_sfx != null) foreach (var e in _sfx) if (e != null) _sfxMap[e.id] = e.sound;

        _musicMap = new Dictionary<MusicId, T_Sound>();
        if (_music != null) foreach (var e in _music) if (e != null) _musicMap[e.id] = e.sound;

        _crowdMap = new Dictionary<CrowdReactionId, T_Sound>();
        if (_crowd != null) foreach (var e in _crowd) if (e != null) _crowdMap[e.id] = e.sound;

        _uiMap = new Dictionary<UiSoundId, T_Sound>();
        if (_ui != null) foreach (var e in _ui) if (e != null) _uiMap[e.id] = e.sound;

        _surfaceMap = new Dictionary<SurfaceType, SurfaceEntry>();
        if (_surfaces != null) foreach (var e in _surfaces) if (e != null) _surfaceMap[e.surface] = e;
    }

    private void CreateSources()
    {
        _musicA = CreateSource("Music A", _musicGroup, true);
        _musicB = CreateSource("Music B", _musicGroup, true);
        _activeMusic = _musicA;

        _bedCalm = CreateSource("Bed Calm", _crowdGroup, true);
        _bedRiled = CreateSource("Bed Riled", _crowdGroup, true);
        _crowdOneShot = CreateSource("Crowd OneShot", _crowdGroup, false);

        _uiSource = CreateSource("UI", _uiGroup, false);
        _dialogueSource = CreateSource("Dialogue", _sfxGroup, false);

        int voices = Mathf.Max(1, _sfxVoices);
        _sfxSources = new AudioSource[voices];
        for (int i = 0; i < voices; i++) _sfxSources[i] = CreateSource("SFX " + i, _sfxGroup, false);
    }

    private AudioSource CreateSource(string sourceName, AudioMixerGroup group, bool loop)
    {
        var go = new GameObject(sourceName);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = group;
        src.playOnAwake = false;
        src.loop = loop;
        src.spatialBlend = 0f; // 2D
        return src;
    }

    // ---------- gameplay handlers ----------

    private void OnParry(ParryQuality quality) => PlaySfx(quality == ParryQuality.Perfect ? SfxId.PerfectParry : SfxId.NormalParry);
    private void OnPlayerDamaged() => PlaySfx(SfxId.PlayerDamage);
    private void OnPlayerDied() => PlaySfx(SfxId.PlayerDeath);
    private void OnEnemyKilled() => PlaySfx(SfxId.EnemyDeath);
    private void OnEnemyDamaged() => PlaySfx(SfxId.EnemyDamage);
    private void OnPickup() => PlaySfx(SfxId.Pickup);
    private void OnScoreEvent(ScoreEventType type, int points) => PlaySfx(SfxId.ScoreTick);
    private void OnTelegraph() => PlaySfx(SfxId.EnemyTelegraph);
    private void OnSwing() => PlaySfx(SfxId.PlayerSwing);
    private void OnJump() => PlaySfx(SfxId.Jump);
    private void OnLand(SurfaceType surface) => PlaySurface(surface, true);
    private void OnFootstep(SurfaceType surface) => PlaySurface(surface, false);
    private void OnInteract() => PlaySfx(SfxId.Interact);
    private void OnJokerSigned() => PlaySfx(SfxId.JokerSign);
    private void OnBoltFired() => PlaySfx(SfxId.EnergyBoltFire);
    private void OnPulse() => PlaySfx(SfxId.RadialPulse);

    private void OnComboChanged(int combo, float multiplier)
    {
        // Rising "combo up" cue, pitched higher the longer the streak. Stays quiet on combo breaks.
        if (combo > _lastCombo && _sfxMap.TryGetValue(SfxId.ComboUp, out var sound))
        {
            float pitch = Mathf.Clamp(1f + combo * 0.03f, 1f, 2f);
            Play(sound, NextSfxSource(), pitch);
        }
        _lastCombo = combo;
    }

    // ---------- audio-request handlers ----------

    private void OnCrowdReaction(CrowdReactionId id) { if (_crowdMap.TryGetValue(id, out var s)) Play(s, _crowdOneShot); }
    private void OnUi(UiSoundId id) { if (_uiMap.TryGetValue(id, out var s)) Play(s, _uiSource); }
    private void OnDialogueBlip() { if (_sfxMap.TryGetValue(SfxId.DialogueBlip, out var s)) Play(s, _dialogueSource); }
    private void OnImpresario(ImpresarioId id) => PlaySfx(id == ImpresarioId.Laugh ? SfxId.ImpresarioLaugh : SfxId.ImpresarioStinger);

    // ---------- one-shot playback ----------

    private void PlaySfx(SfxId id)
    {
        if (_sfxMap.TryGetValue(id, out var sound)) Play(sound, NextSfxSource());
    }

    private void PlaySurface(SurfaceType surface, bool landing)
    {
        if (!_surfaceMap.TryGetValue(surface, out var entry) && !_surfaceMap.TryGetValue(SurfaceType.Default, out entry))
        {
            return;
        }
        Play(landing ? entry.land : entry.footstep, NextSfxSource());
    }

    private AudioSource NextSfxSource()
    {
        var src = _sfxSources[_sfxIndex];
        _sfxIndex = (_sfxIndex + 1) % _sfxSources.Length;
        return src;
    }

    private void Play(T_Sound sound, AudioSource source, float pitchOverride = -1f)
    {
        if (sound == null || source == null || sound.variants == null || sound.variants.Length == 0) return;
        AudioClip clip = sound.variants[Random.Range(0, sound.variants.Length)];
        if (clip == null) return;
        source.pitch = pitchOverride > 0f ? pitchOverride : Random.Range(sound.pitchRange.x, sound.pitchRange.y);
        source.PlayOneShot(clip, sound.volume);
    }

    // ---------- music ----------

    private void PlayMusic(MusicId id)
    {
        if (_hasMusic && id == _currentMusic && _activeMusic != null && _activeMusic.isPlaying) return;
        if (!_musicMap.TryGetValue(id, out var sound) || sound.variants == null || sound.variants.Length == 0) return;
        AudioClip clip = sound.variants[Random.Range(0, sound.variants.Length)];
        if (clip == null) return;

        // Remember where to resume from when diverting to the pause track.
        if (id == MusicId.Pause && _hasMusic && _activeMusic != null)
        {
            _resumeMusic = _currentMusic;
            _resumeTime = _activeMusic.time;
            _hasResume = true;
        }

        AudioSource from = _activeMusic;
        AudioSource to = (_activeMusic == _musicA) ? _musicB : _musicA;

        to.clip = clip;
        to.volume = 0f;
        to.Play();

        // Resume the previous track at its saved position (e.g. unpausing back into an act).
        if (_hasResume && id == _resumeMusic)
        {
            to.time = Mathf.Min(_resumeTime, Mathf.Max(0f, clip.length - 0.05f));
            _hasResume = false;
        }

        if (_musicFade != null) StopCoroutine(_musicFade);
        _musicFade = StartCoroutine(CrossfadeMusic(from, to, sound.volume));

        _activeMusic = to;
        _currentMusic = id;
        _hasMusic = true;
    }

    private void StopMusic()
    {
        if (_musicFade != null) { StopCoroutine(_musicFade); _musicFade = null; }
        if (_musicA != null) _musicA.Stop();
        if (_musicB != null) _musicB.Stop();
        _hasMusic = false;
    }

    private IEnumerator CrossfadeMusic(AudioSource from, AudioSource to, float targetVolume)
    {
        float t = 0f;
        float fromStart = from != null ? from.volume : 0f;
        while (t < _musicFadeSeconds)
        {
            t += Time.unscaledDeltaTime; // works while paused (timeScale 0)
            float k = _musicFadeSeconds > 0f ? Mathf.Clamp01(t / _musicFadeSeconds) : 1f;
            if (from != null) from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(0f, targetVolume, k);
            yield return null;
        }
        to.volume = targetVolume;
        if (from != null) from.Stop();
        _musicFade = null;
    }

    // ---------- crowd ambient bed ----------

    private void SetBed(float mood01)
    {
        mood01 = Mathf.Clamp01(mood01);
        StartBed(_bedCalm, _crowdBedCalm);
        StartBed(_bedRiled, _crowdBedRiled);
        if (_bedCalm != null) _bedCalm.volume = (1f - mood01) * _bedVolume;
        if (_bedRiled != null) _bedRiled.volume = mood01 * _bedVolume;
    }

    private void StartBed(AudioSource source, T_Sound sound)
    {
        if (source == null || source.isPlaying) return;
        if (sound == null || sound.variants == null || sound.variants.Length == 0 || sound.variants[0] == null) return;
        source.clip = sound.variants[0];
        source.volume = 0f;
        source.Play();
    }

    // ---------- volume ----------

    private void SetVolume(AudioCategory category, float volume01)
    {
        volume01 = Mathf.Clamp01(volume01);
        string param = ParamFor(category);
        if (_mixer != null && !string.IsNullOrEmpty(param))
        {
            _mixer.SetFloat(param, LinearToDecibels(volume01));
        }
        PlayerPrefs.SetFloat(PrefKey(category), volume01);
    }

    private void LoadVolumes()
    {
        foreach (AudioCategory c in System.Enum.GetValues(typeof(AudioCategory)))
        {
            SetVolume(c, PlayerPrefs.GetFloat(PrefKey(c), 1f));
        }
    }

    private string ParamFor(AudioCategory category)
    {
        switch (category)
        {
            case AudioCategory.Master: return _masterParam;
            case AudioCategory.Music: return _musicParam;
            case AudioCategory.Sfx: return _sfxParam;
            case AudioCategory.Crowd: return _crowdParam;
            case AudioCategory.Ui: return _uiParam;
            default: return null;
        }
    }

    private static string PrefKey(AudioCategory category) => "vol_" + category;

    // Unity mixer volumes are in decibels; convert a 0–1 linear slider value.
    private static float LinearToDecibels(float linear)
    {
        return linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    }
}
