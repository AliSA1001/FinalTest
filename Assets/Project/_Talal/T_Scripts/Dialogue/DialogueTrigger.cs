using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// NPC-side hook that starts a conversation. Implements <see cref="IInteractable"/> so the real Interaction
/// system can drive it later; for now a minimal built-in detector (trigger collider + interact input) makes
/// the NPC work on its own. It only raises <see cref="DialogueEvents.RequestStart"/> — no reference to the
/// runner or any other system.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData _dialogue;
    [SerializeField] private string _prompt = "Talk";
    [Tooltip("Optional world-space prompt object shown while the player is in range.")]
    [SerializeField] private GameObject _promptObject;
    [Tooltip("Tag of the player object.")]
    [SerializeField] private string _playerTag = "Player";
    [Tooltip("Interact action (e.g. Player/Interact). Used by the built-in detector.")]
    [SerializeField] private InputActionReference _interact;

    private bool _playerInRange;

    public string Prompt => _prompt;

    public void Interact()
    {
        if (_dialogue != null) DialogueEvents.RequestStart(_dialogue);
    }

    private void Awake()
    {
        if (_promptObject != null) _promptObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_interact != null && _interact.action != null)
        {
            _interact.action.performed += OnInteractInput;
            _interact.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (_interact != null && _interact.action != null)
        {
            _interact.action.performed -= OnInteractInput;
            _interact.action.Disable();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;
        _playerInRange = true;
        if (_promptObject != null) _promptObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;
        _playerInRange = false;
        if (_promptObject != null) _promptObject.SetActive(false);
    }

    private void OnInteractInput(InputAction.CallbackContext _)
    {
        if (_playerInRange) Interact();
    }
}
