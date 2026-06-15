using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks every active <see cref="IInteractable"/> in the scene. Interactables register on enable and
/// unregister on disable; the <see cref="Interactor"/> reads the list to find the nearest target — no physics
/// or scene lookups needed.
/// </summary>
public static class InteractionRegistry
{
    private static readonly List<IInteractable> _interactables = new List<IInteractable>();

    public static IReadOnlyList<IInteractable> All => _interactables;

    public static void Register(IInteractable interactable)
    {
        if (interactable != null && !_interactables.Contains(interactable))
        {
            _interactables.Add(interactable);
        }
    }

    public static void Unregister(IInteractable interactable)
    {
        _interactables.Remove(interactable);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAll()
    {
        _interactables.Clear();
    }
}
