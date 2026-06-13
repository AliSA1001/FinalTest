/// <summary>
/// Implemented by things the player can interact with. The future Interaction system will detect the target
/// and call <see cref="Interact"/>; until it exists, <see cref="DialogueTrigger"/> drives this itself.
/// </summary>
public interface IInteractable
{
    /// <summary>Short verb shown on the prompt, e.g. "Talk".</summary>
    string Prompt { get; }

    /// <summary>Perform the interaction.</summary>
    void Interact();
}
