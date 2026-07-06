using UnityEngine;

/// <summary>
/// Integration bridge: connects the player's "took a hit" event to Talal's event bus without touching any
/// teammate code. Subscribes to <see cref="A_EnemyHitCollider.OnTookHit"/> (raised when an enemy's attack
/// lands on the player and the player isn't invincible) and re-raises <see cref="T_GameSignals.PlayerDamaged"/>,
/// which breaks the combo, applies the Score damage penalty, cues the crowd boo, and plays the hurt SFX.
///
/// Setup: drop this on the player root. It auto-finds <see cref="A_EnemyHitCollider"/> in its children; wire
/// the override field only if it lives somewhere unexpected. References only the event bus and the player's
/// own hit event.
///
/// Note: this covers Ali's collider-based damage. Waad's <c>EnemyAttack</c> damages the player by calling
/// HeartsHealthVisual directly with no event, so hits from that path won't reach here without a teammate edit.
/// </summary>
public class T_PlayerEventBridge : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to auto-find on this object / its children.")]
    [SerializeField] private A_EnemyHitCollider _hitCollider;

    private void Awake()
    {
        if (_hitCollider == null) _hitCollider = GetComponentInChildren<A_EnemyHitCollider>(true);
    }

    private void OnEnable()
    {
        if (_hitCollider != null) _hitCollider.OnTookHit += HandlePlayerHit;
    }

    private void OnDisable()
    {
        if (_hitCollider != null) _hitCollider.OnTookHit -= HandlePlayerHit;
    }

    // enemyType (1 = small, 2 = big) isn't used yet; PlayerDamaged is a flat signal. Kept for future tuning.
    private void HandlePlayerHit(int enemyType) => T_GameSignals.RaisePlayerDamaged();
}
