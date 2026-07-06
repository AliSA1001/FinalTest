using UnityEngine;

/// <summary>
/// Integration bridge: connects one enemy's combat components to Talal's event bus without touching any
/// teammate code. It subscribes to the C# events the enemy already exposes and re-raises them onto
/// <see cref="T_GameSignals"/>, where Score / Crowd / Audio pick them up automatically.
///
/// Raises:
///   * <see cref="T_GameSignals.AttackHitLanded"/> + <see cref="T_GameSignals.EnemyDamaged"/> when the enemy
///     takes a hit (<see cref="EnemyHealth.OnHit"/>) — score + "enemy hurt" SFX.
///   * <see cref="T_GameSignals.ParryLanded"/> when the player's parry connects
///     (<see cref="A_EnemyCheckForParry.OnParry"/>).
///   * <see cref="T_GameSignals.EnemyKilled"/> when this enemy is destroyed in-play.
///
/// Setup: drop this on the enemy prefab root. It auto-finds <see cref="EnemyHealth"/> and
/// <see cref="A_EnemyCheckForParry"/> in its children; if either lives somewhere unexpected, wire the
/// override field in the Inspector. References only the event bus and (read-only) the enemy's own events.
/// </summary>
public class T_EnemyEventBridge : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to auto-find on this object / its children.")]
    [SerializeField] private EnemyHealth _health;
    [Tooltip("Optional. Leave empty to auto-find on this object / its children.")]
    [SerializeField] private A_EnemyCheckForParry _parryCheck;

    // Set true once the app is tearing down, so OnDestroy doesn't mistake a quit for a kill.
    private static bool _appQuitting;

    // Clear the static flag on every Play, so a leftover quit from a previous session (when domain reload
    // is disabled) can't permanently suppress kill signals.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _appQuitting = false;

    private void Awake()
    {
        if (_health == null) _health = GetComponentInChildren<EnemyHealth>(true);
        if (_parryCheck == null) _parryCheck = GetComponentInChildren<A_EnemyCheckForParry>(true);
    }

    private void OnEnable()
    {
        if (_health != null) _health.OnHit += HandleEnemyHit;
        if (_parryCheck != null) _parryCheck.OnParry += HandleParry;
    }

    private void OnDisable()
    {
        if (_health != null) _health.OnHit -= HandleEnemyHit;
        if (_parryCheck != null) _parryCheck.OnParry -= HandleParry;
    }

    private void OnApplicationQuit() => _appQuitting = true;

    // The player's attack connected with this enemy: score the hit, and cue the "enemy hurt" SFX.
    // On the killing blow this still fires (Hit + Kill), which is intended — the kill just adds its bonus.
    private void HandleEnemyHit()
    {
        T_GameSignals.RaiseAttackHitLanded();
        T_GameSignals.RaiseEnemyDamaged();
    }

    private void HandleParry() => T_GameSignals.RaiseParryLanded();

    private void OnDestroy()
    {
        // EnemyHealth destroys this GameObject only when it dies, so an in-play destroy == a kill.
        // Ignore destroys caused by leaving Play mode, changing scenes, or quitting.
        if (_appQuitting || !gameObject.scene.isLoaded) return;
        T_GameSignals.RaiseEnemyKilled();
    }
}
