using UnityEngine;

/// <summary>
/// Something the player can interact with. The <see cref="Interactor"/> finds the nearest registered
/// interactable and calls <see cref="Interact"/>; implementers register with <see cref="InteractionRegistry"/>.
/// </summary>
public interface IInteractable
{
    /// <summary>Short verb shown on the prompt, e.g. "Talk".</summary>
    string Prompt { get; }

    /// <summary>World position, used to find the nearest interactable.</summary>
    Vector3 Position { get; }

    /// <summary>Perform the interaction.</summary>
    void Interact();
}
