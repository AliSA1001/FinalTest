using UnityEngine;

/// <summary>
/// Minimal interactable for verifying the Interaction system on its own (no dialogue needed). Implements
/// <see cref="IInteractable"/>, registers itself, and logs when interacted with. Drop it on any object
/// (e.g. a capsule), walk up, and press the interact key — you should see the log in the Console. Also a handy
/// template for real interactables like pickups.
/// </summary>
public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt = "Test";

    public string Prompt => _prompt;
    public Vector3 Position => transform.position;

    private void OnEnable() => InteractionRegistry.Register(this);
    private void OnDisable() => InteractionRegistry.Unregister(this);

    public void Interact()
    {
        Debug.Log($"[Interaction] Interacted with '{name}' - the Interaction system works end to end.");
    }
}
