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
/// Ground surface the player is on, carried by <see cref="T_GameSignals.Footstep"/> and
/// <see cref="T_GameSignals.PlayerLanded"/> so Audio can pick the right footstep / landing clip.
/// </summary>
public enum SurfaceType
{
    Default,
    Stage,
    Wood,
    Stone,
    Grass,
    Metal,
    Carpet
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

    // ---------- INBOUND: additional gameplay signals (raised by Combat / Enemy / Movement / Interaction; consumed by Audio and others) ----------

    /// <summary>An enemy took damage but survived.</summary>
    public static event Action EnemyDamaged;
    /// <summary>An enemy is winding up an attack (telegraph) — the cue to parry.</summary>
    public static event Action EnemyAttackTelegraph;
    /// <summary>The player swung an attack (whoosh), regardless of whether it connects.</summary>
    public static event Action PlayerAttackSwing;
    /// <summary>The player jumped.</summary>
    public static event Action PlayerJumped;
    /// <summary>The player landed, on the given surface.</summary>
    public static event Action<SurfaceType> PlayerLanded;
    /// <summary>A footstep occurred, on the given surface (raised by movement / animation).</summary>
    public static event Action<SurfaceType> Footstep;
    /// <summary>The player performed an interaction (hub objects, the Impresario).</summary>
    public static event Action InteractionPerformed;
    /// <summary>The player signed the Joker card — the "deal struck" beat.</summary>
    public static event Action JokerCardSigned;
    /// <summary>Act 2: an energy bolt was fired.</summary>
    public static event Action EnergyBoltFired;
    /// <summary>Act 2: a radial pulse was released (swarm clear).</summary>
    public static event Action RadialPulseFired;

    public static void RaiseEnemyDamaged() => EnemyDamaged?.Invoke();
    public static void RaiseEnemyAttackTelegraph() => EnemyAttackTelegraph?.Invoke();
    public static void RaisePlayerAttackSwing() => PlayerAttackSwing?.Invoke();
    public static void RaisePlayerJumped() => PlayerJumped?.Invoke();
    public static void RaisePlayerLanded(SurfaceType surface) => PlayerLanded?.Invoke(surface);
    public static void RaiseFootstep(SurfaceType surface) => Footstep?.Invoke(surface);
    public static void RaiseInteractionPerformed() => InteractionPerformed?.Invoke();
    public static void RaiseJokerCardSigned() => JokerCardSigned?.Invoke();
    public static void RaiseEnergyBoltFired() => EnergyBoltFired?.Invoke();
    public static void RaiseRadialPulseFired() => RadialPulseFired?.Invoke();

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

        EnemyDamaged = null;
        EnemyAttackTelegraph = null;
        PlayerAttackSwing = null;
        PlayerJumped = null;
        PlayerLanded = null;
        Footstep = null;
        InteractionPerformed = null;
        JokerCardSigned = null;
        EnergyBoltFired = null;
        RadialPulseFired = null;

        ScoreChanged = null;
        ComboChanged = null;
        ScoreEventLanded = null;
        ActRated = null;
        RunResultReady = null;
    }
}
