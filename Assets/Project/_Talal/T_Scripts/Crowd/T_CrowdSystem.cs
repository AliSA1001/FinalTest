using UnityEngine;

/// <summary>
/// Crowd reaction system (Tier 2) for "Act to Wish" — the audience's live mood and reactions, which are the
/// scoring feedback and the game's identity. It listens to score/gameplay events on <see cref="T_GameSignals"/>,
/// maintains a mood (0-1), and drives Audio via <see cref="T_AudioSignals"/> (the ambient bed + cheers/boos).
/// It references no system directly — only the two event buses — so it slots cleanly between Score and Audio.
/// </summary>
public class T_CrowdSystem : MonoBehaviour
{
    [Header("Mood")]
    [Range(0f, 1f)] [SerializeField] private float _neutralMood = 0.5f;
    [Tooltip("How fast mood drifts back toward neutral, per second.")]
    [SerializeField] private float _decayPerSecond = 0.15f;
    [Tooltip("Only push SetCrowdMood to Audio when mood moves at least this much.")]
    [SerializeField] private float _moodEpsilon = 0.01f;

    [Header("Mood gains (scaled by the combo multiplier)")]
    [SerializeField] private float _parryGain = 0.04f;
    [SerializeField] private float _hitGain = 0.02f;
    [SerializeField] private float _killGain = 0.10f;
    [SerializeField] private float _collectableGain = 0.03f;

    [Header("Mood losses")]
    [SerializeField] private float _damageDrop = 0.20f;
    [SerializeField] private float _deathDrop = 0.40f;

    [Header("Reactions")]
    [Tooltip("Minimum seconds between crowd one-shot reactions, so they don't spam.")]
    [SerializeField] private float _reactionCooldown = 1.5f;
    [Tooltip("When mood rises across this, the crowd cheers.")]
    [Range(0f, 1f)] [SerializeField] private float _cheerThreshold = 0.8f;
    [Tooltip("Combo multiples that trigger a gasp (10 = at 10, 20, …). 0 = off.")]
    [SerializeField] private int _gaspComboMilestone = 10;

    // runtime state
    private float _mood;
    private float _multiplier = 1f;
    private float _lastSentMood;
    private float _lastReactionTime = -999f;
    private bool _wasAboveCheer;

    public float CurrentMood => _mood;

    private void OnEnable()
    {
        _mood = _neutralMood;
        _lastSentMood = -1f; // force the first push

        T_GameSignals.ScoreEventLanded += OnScoreEvent;
        T_GameSignals.ComboChanged += OnComboChanged;
        T_GameSignals.PlayerDamaged += OnPlayerDamaged;
        T_GameSignals.PlayerDied += OnPlayerDied;
        T_GameSignals.ActStarted += OnActStarted;
        T_GameSignals.ActFinished += OnActFinished;
    }

    private void OnDisable()
    {
        T_GameSignals.ScoreEventLanded -= OnScoreEvent;
        T_GameSignals.ComboChanged -= OnComboChanged;
        T_GameSignals.PlayerDamaged -= OnPlayerDamaged;
        T_GameSignals.PlayerDied -= OnPlayerDied;
        T_GameSignals.ActStarted -= OnActStarted;
        T_GameSignals.ActFinished -= OnActFinished;
    }

    private void Start()
    {
        PushMood(true);
    }

    private void Update()
    {
        // Drift back toward neutral over time.
        _mood = Mathf.MoveTowards(_mood, _neutralMood, _decayPerSecond * Time.deltaTime);
        PushMood(false);

        // Cheer when mood rises across the high threshold.
        bool aboveCheer = _mood >= _cheerThreshold;
        if (aboveCheer && !_wasAboveCheer) TryReaction(RandomCheer());
        _wasAboveCheer = aboveCheer;
    }

    // ---- inbound score / gameplay events ----

    private void OnScoreEvent(ScoreEventType type, int points)
    {
        float gain;
        switch (type)
        {
            case ScoreEventType.Parry: gain = _parryGain; break;
            case ScoreEventType.Hit: gain = _hitGain; break;
            case ScoreEventType.Kill: gain = _killGain; break;
            case ScoreEventType.Collectable: gain = _collectableGain; break;
            default: gain = 0f; break;
        }
        AddMood(gain * Mathf.Max(1f, _multiplier));

        if (type == ScoreEventType.Kill) TryReaction(RandomCheer());
    }

    private void OnComboChanged(int combo, float multiplier)
    {
        _multiplier = multiplier;
        if (_gaspComboMilestone > 0 && combo > 0 && combo % _gaspComboMilestone == 0)
        {
            TryReaction(CrowdReactionId.Gasp);
        }
    }

    private void OnPlayerDamaged()
    {
        AddMood(-_damageDrop);
        TryReaction(RandomBoo());
    }

    private void OnPlayerDied()
    {
        AddMood(-_deathDrop);
        TryReaction(RandomBoo());
    }

    private void OnActStarted(int actIndex)
    {
        _mood = _neutralMood;
        _wasAboveCheer = _mood >= _cheerThreshold;
        PushMood(true);
    }

    private void OnActFinished(int actIndex)
    {
        TryReaction(CrowdReactionId.Applause);
    }

    // ---- mood + reactions ----

    private void AddMood(float delta)
    {
        _mood = Mathf.Clamp01(_mood + delta);
        PushMood(false);
    }

    private void PushMood(bool force)
    {
        if (force || Mathf.Abs(_mood - _lastSentMood) >= _moodEpsilon)
        {
            _lastSentMood = _mood;
            T_AudioSignals.SetCrowdMood(_mood);
        }
    }

    private void TryReaction(CrowdReactionId reaction)
    {
        if (Time.time - _lastReactionTime < _reactionCooldown) return;
        _lastReactionTime = Time.time;
        T_AudioSignals.PlayCrowdReaction(reaction);
    }

    private CrowdReactionId RandomCheer() => Random.value < 0.5f ? CrowdReactionId.Cheer1 : CrowdReactionId.Cheer2;
    private CrowdReactionId RandomBoo() => Random.value < 0.5f ? CrowdReactionId.Boo1 : CrowdReactionId.Boo2;
}
