using System;
using UnityEngine;

/// <summary>
/// Quality tier of a parry. Set by the Combat system when it raises
/// <see cref="T_GameSignals.ParryLanded"/>; a perfect parry scores more and (via Crowd) draws a bigger cheer.
/// </summary>
public enum ParryQuality
{
    Good,
    Perfect
}

/// <summary>
/// The kind of scoring action that landed. Carried by <see cref="T_GameSignals.ScoreEventLanded"/> so
/// listeners (Crowd, Audio, UI) can react to *what* happened, not just the new running total.
/// </summary>
public enum ScoreEventType
{
    Parry,
    PerfectParry,
    Hit,
    Kill,
    Collectable
}

/// <summary>
/// Static event bus for the score-relevant gameplay signals in "Act to Wish".
///
/// Architecture rule (GDD): systems communicate through events, never direct references.
///   * INBOUND signals are raised by other systems (Combat, enemies, collectables, the Act sequencer)
///     and consumed by the Score/Rating system.
///   * OUTBOUND signals are raised by Score and consumed by UI, Crowd, Audio and Game-flow.
///
/// This bus currently lives in Talal's folder but is shared infrastructure: other developers raise the
/// inbound signals and subscribe to the outbound ones. No system references another directly.
///
/// A static bus keeps global subscriptions, so always subscribe in OnEnable and unsubscribe in OnDisable.
/// <see cref="ResetAll"/> additionally clears everything on Play, guarding against stale handlers when the
/// Editor's "Enter Play Mode (no domain reload)" option is enabled.
/// </summary>
public static class T_GameSignals
{
    // ---------- INBOUND: raised by other systems, consumed by Score ----------

    /// <summary>A parry succeeded, with its quality tier.</summary>
    public static event Action<ParryQuality> ParryLanded;
    /// <summary>An offensive attack connected with an enemy.</summary>
    public static event Action AttackHitLanded;
    /// <summary>An enemy was killed.</summary>
    public static event Action EnemyKilled;
    /// <summary>A collectable was picked up.</summary>
    public static event Action CollectableCollected;
    /// <summary>The player took damage (breaks the combo).</summary>
    public static event Action PlayerDamaged;
    /// <summary>The player died and reset to a checkpoint (rating penalty).</summary>
    public static event Action PlayerDied;
    /// <summary>An act began. Argument is the zero-based act index.</summary>
    public static event Action<int> ActStarted;
    /// <summary>An act finished. Argument is the zero-based act index.</summary>
    public static event Action<int> ActFinished;

    public static void RaiseParryLanded(ParryQuality quality) => ParryLanded?.Invoke(quality);
    public static void RaiseAttackHitLanded() => AttackHitLanded?.Invoke();
    public static void RaiseEnemyKilled() => EnemyKilled?.Invoke();
    public static void RaiseCollectableCollected() => CollectableCollected?.Invoke();
    public static void RaisePlayerDamaged() => PlayerDamaged?.Invoke();
    public static void RaisePlayerDied() => PlayerDied?.Invoke();
    public static void RaiseActStarted(int actIndex) => ActStarted?.Invoke(actIndex);
    public static void RaiseActFinished(int actIndex) => ActFinished?.Invoke(actIndex);

    // ---------- OUTBOUND: raised by Score, consumed by UI / Crowd / Audio / Game-flow ----------

    /// <summary>The running act score changed. (newTotal, appliedDelta)</summary>
    public static event Action<int, int> ScoreChanged;
    /// <summary>The combo changed. (comboCount, multiplier)</summary>
    public static event Action<int, float> ComboChanged;
    /// <summary>A scoring action was registered. (type, pointsAwarded) — for crowd/audio reactions.</summary>
    public static event Action<ScoreEventType, int> ScoreEventLanded;
    /// <summary>An act was rated. (actIndex, stars, finalScore)</summary>
    public static event Action<int, int, int> ActRated;
    /// <summary>All acts done; aggregate result ready. (totalStars, passed)</summary>
    public static event Action<int, bool> RunResultReady;

    public static void RaiseScoreChanged(int newTotal, int appliedDelta) => ScoreChanged?.Invoke(newTotal, appliedDelta);
    public static void RaiseComboChanged(int combo, float multiplier) => ComboChanged?.Invoke(combo, multiplier);
    public static void RaiseScoreEventLanded(ScoreEventType type, int points) => ScoreEventLanded?.Invoke(type, points);
    public static void RaiseActRated(int actIndex, int stars, int finalScore) => ActRated?.Invoke(actIndex, stars, finalScore);
    public static void RaiseRunResultReady(int totalStars, bool passed) => RunResultReady?.Invoke(totalStars, passed);

    /// <summary>
    /// Clears every subscription. Runs automatically on Play so no static handler can survive from a
    /// previous session when domain reload is disabled. Subscribers must still unsubscribe in OnDisable.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetAll()
    {
        ParryLanded = null;
        AttackHitLanded = null;
        EnemyKilled = null;
        CollectableCollected = null;
        PlayerDamaged = null;
        PlayerDied = null;
        ActStarted = null;
        ActFinished = null;

        ScoreChanged = null;
        ComboChanged = null;
        ScoreEventLanded = null;
        ActRated = null;
        RunResultReady = null;
    }
}
