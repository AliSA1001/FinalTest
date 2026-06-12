using UnityEngine;

/// <summary>
/// Score/Rating system (Tier-0 foundation) for "Act to Wish".
///
/// Listens for score-relevant gameplay signals on <see cref="T_GameSignals"/>, turns them into points
/// scaled by a combo multiplier, tracks a per-act star rating, and raises score signals for UI / Crowd /
/// Audio / Game-flow. It references no other system directly — all communication is through the event bus.
///
/// Attach one instance to a persistent "Systems" GameObject and tune the serialized values.
/// </summary>
public class T_ScoreSystem : MonoBehaviour
{
    [Header("Point values (before combo multiplier)")]
    [SerializeField] private int _parryPoints = 100;
    [SerializeField] private int _perfectParryPoints = 250;
    [SerializeField] private int _hitPoints = 25;
    [SerializeField] private int _killPoints = 150;
    [SerializeField] private int _collectablePoints = 50;

    [Header("Combo")]
    [Tooltip("Multiplier added per combo step, e.g. 0.1 => x1.0, x1.1, x1.2 …")]
    [SerializeField] private float _comboStep = 0.1f;
    [Tooltip("Hard cap on the combo multiplier.")]
    [SerializeField] private float _maxMultiplier = 4f;
    [Tooltip("Seconds without a scoring action before the combo resets. 0 = never decays.")]
    [SerializeField] private float _comboResetSeconds = 4f;

    [Header("Penalties")]
    [Tooltip("Points lost when the player takes a hit (also breaks the combo).")]
    [SerializeField] private int _damagePenalty = 80;
    [Tooltip("Points lost when the player dies and resets to a checkpoint.")]
    [SerializeField] private int _deathPenalty = 200;

    [Header("Rating")]
    [Tooltip("Ascending score thresholds; one star per threshold reached. {1000,2500,5000} => up to 3 stars.")]
    [SerializeField] private int[] _starThresholds = { 1000, 2500, 5000 };
    [Tooltip("How many acts make up a full run.")]
    [SerializeField] private int _actCount = 3;
    [Tooltip("Total stars across all acts needed to reach the winning ending.")]
    [SerializeField] private int _passThreshold = 6;

    // Runtime state for the act in progress.
    private int _currentScore;
    private int _combo;
    private float _multiplier = 1f;
    private float _comboTimer;

    // Per-act star results, filled as acts finish and summed for the run total.
    private int[] _actStars;
    private int _actsRated;

    public int CurrentScore => _currentScore;
    public int Combo => _combo;
    public float Multiplier => _multiplier;
    public int CurrentStars => StarsFor(_currentScore);

    private void Awake()
    {
        _actStars = new int[Mathf.Max(1, _actCount)];
    }

    private void OnEnable()
    {
        T_GameSignals.ParryLanded += HandleParry;
        T_GameSignals.AttackHitLanded += HandleHit;
        T_GameSignals.EnemyKilled += HandleKill;
        T_GameSignals.CollectableCollected += HandleCollectable;
        T_GameSignals.PlayerDamaged += HandlePlayerDamaged;
        T_GameSignals.PlayerDied += HandlePlayerDied;
        T_GameSignals.ActStarted += HandleActStarted;
        T_GameSignals.ActFinished += HandleActFinished;
    }

    private void OnDisable()
    {
        T_GameSignals.ParryLanded -= HandleParry;
        T_GameSignals.AttackHitLanded -= HandleHit;
        T_GameSignals.EnemyKilled -= HandleKill;
        T_GameSignals.CollectableCollected -= HandleCollectable;
        T_GameSignals.PlayerDamaged -= HandlePlayerDamaged;
        T_GameSignals.PlayerDied -= HandlePlayerDied;
        T_GameSignals.ActStarted -= HandleActStarted;
        T_GameSignals.ActFinished -= HandleActFinished;
    }

    private void Update()
    {
        // Optional combo decay. Cheap early-out; no lookups per the GDD's Update rule.
        if (_comboResetSeconds <= 0f || _combo == 0)
        {
            return;
        }

        _comboTimer += Time.deltaTime;
        if (_comboTimer >= _comboResetSeconds)
        {
            ResetCombo();
        }
    }

    // ---------- Inbound signal handlers ----------

    private void HandleParry(ParryQuality quality)
    {
        if (quality == ParryQuality.Perfect)
        {
            Award(ScoreEventType.PerfectParry, _perfectParryPoints);
        }
        else
        {
            Award(ScoreEventType.Parry, _parryPoints);
        }
    }

    private void HandleHit() => Award(ScoreEventType.Hit, _hitPoints);
    private void HandleKill() => Award(ScoreEventType.Kill, _killPoints);
    private void HandleCollectable() => Award(ScoreEventType.Collectable, _collectablePoints);

    private void HandlePlayerDamaged()
    {
        ResetCombo();
        if (_damagePenalty != 0)
        {
            ChangeScore(-_damagePenalty);
        }
    }

    private void HandlePlayerDied()
    {
        ResetCombo();
        if (_deathPenalty != 0)
        {
            ChangeScore(-_deathPenalty);
        }
    }

    private void HandleActStarted(int actIndex)
    {
        int delta = -_currentScore;
        _currentScore = 0;
        ResetCombo();
        T_GameSignals.RaiseScoreChanged(_currentScore, delta);
    }

    private void HandleActFinished(int actIndex)
    {
        int stars = StarsFor(_currentScore);

        if (_actStars != null && actIndex >= 0 && actIndex < _actStars.Length)
        {
            _actStars[actIndex] = stars;
        }
        _actsRated++;

        T_GameSignals.RaiseActRated(actIndex, stars, _currentScore);

        if (_actsRated >= _actCount)
        {
            int total = TotalStars();
            T_GameSignals.RaiseRunResultReady(total, total >= _passThreshold);
        }
    }

    // ---------- Scoring core ----------

    private void Award(ScoreEventType type, int basePoints)
    {
        _combo++;
        _comboTimer = 0f;
        _multiplier = Mathf.Min(_maxMultiplier, 1f + (_combo - 1) * _comboStep);
        T_GameSignals.RaiseComboChanged(_combo, _multiplier);

        int points = Mathf.RoundToInt(basePoints * _multiplier);
        ChangeScore(points);
        T_GameSignals.RaiseScoreEventLanded(type, points);
    }

    private void ChangeScore(int delta)
    {
        int previous = _currentScore;
        _currentScore = Mathf.Max(0, _currentScore + delta);
        T_GameSignals.RaiseScoreChanged(_currentScore, _currentScore - previous);
    }

    private void ResetCombo()
    {
        _comboTimer = 0f;
        if (_combo == 0 && Mathf.Approximately(_multiplier, 1f))
        {
            return;
        }
        _combo = 0;
        _multiplier = 1f;
        T_GameSignals.RaiseComboChanged(_combo, _multiplier);
    }

    // ---------- Rating (pure) ----------

    /// <summary>Stars earned for a given score = number of ascending thresholds reached.</summary>
    private int StarsFor(int score)
    {
        if (_starThresholds == null)
        {
            return 0;
        }

        int stars = 0;
        for (int i = 0; i < _starThresholds.Length; i++)
        {
            if (score >= _starThresholds[i])
            {
                stars++;
            }
        }
        return stars;
    }

    private int TotalStars()
    {
        int total = 0;
        if (_actStars != null)
        {
            for (int i = 0; i < _actStars.Length; i++)
            {
                total += _actStars[i];
            }
        }
        return total;
    }
}
