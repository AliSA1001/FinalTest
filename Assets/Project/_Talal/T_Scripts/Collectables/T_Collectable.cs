using UnityEngine;

/// <summary>
/// A score collectable, auto-collected on touch. When the player enters its trigger it raises
/// <see cref="T_GameSignals.CollectableCollected"/> — which Score turns into points (+combo), Audio into a
/// pickup SFX, and Crowd into a mood bump — then removes itself. References only the event bus.
///
/// Setup: give the object a Collider with <b>Is Trigger</b> enabled, and tag the player to match
/// <see cref="_playerTag"/>.
/// </summary>
[RequireComponent(typeof(Collider))]
public class T_Collectable : MonoBehaviour
{
    [Tooltip("Tag of the player object that can collect this.")]
    [SerializeField] private string _playerTag = "Player";
    [Tooltip("Rotation speed of the collectable, in degrees per second.")]
    [SerializeField] private float _rotationSpeed = 100f;

    private bool _collected;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected || !other.CompareTag(_playerTag)) return;

        _collected = true;
        T_GameSignals.RaiseCollectableCollected();
        Destroy(gameObject);
    }
    public void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
