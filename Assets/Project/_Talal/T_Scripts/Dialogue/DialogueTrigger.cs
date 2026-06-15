using UnityEngine;

/// <summary>
/// NPC/object hook that starts a conversation when interacted with. A pure <see cref="IInteractable"/>: the
/// central <see cref="Interactor"/> handles detection, prompt and input, then calls <see cref="Interact"/>,
/// which asks the runner to start this trigger's dialogue. References no other system beyond the dialogue bus.
/// </summary>
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData _dialogue;
    [SerializeField] private string _prompt = "Talk";

    public string Prompt => _prompt;
    public Vector3 Position => transform.position;

    private void OnEnable() => InteractionRegistry.Register(this);
    private void OnDisable() => InteractionRegistry.Unregister(this);

    public void Interact()
    {
        if (_dialogue != null) DialogueEvents.RequestStart(_dialogue);
    }
}
