using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player-side interaction driver. Each frame it finds the nearest registered <see cref="IInteractable"/>
/// within range (and an optional facing cone), shows/hides the prompt via <see cref="InteractionEvents"/>, and
/// on the interact key calls the target.
///
/// The interact key is read directly from <see cref="Keyboard.current"/>, so it works regardless of which
/// input map / action asset the player's movement uses. (Switch to an InputActionReference later if you want
/// rebinding or gamepad support routed through your own map.)
/// </summary>
public class Interactor : MonoBehaviour
{
    [Tooltip("Key that triggers interaction. Read directly, independent of the player's input map.")]
    [SerializeField] private Key _interactKey = Key.E;
    [Tooltip("Maximum distance to an interactable.")]
    [SerializeField] private float _range = 3f;
    [Tooltip("Max angle (degrees) between the player's forward and the interactable. 360 = any direction.")]
    [SerializeField] private float _maxFacingAngle = 360f;

    private IInteractable _current;

    private void OnDisable()
    {
        if (_current != null) SetCurrent(null);
    }

    private void Update()
    {
        IInteractable best = FindNearest();
        if (best != _current) SetCurrent(best);

        if (_current != null
            && Keyboard.current != null
            && Keyboard.current[_interactKey].wasPressedThisFrame)
        {
            _current.Interact();
            T_GameSignals.RaiseInteractionPerformed(); // lets Audio play the interact SFX
        }
    }

    private IInteractable FindNearest()
    {
        IReadOnlyList<IInteractable> all = InteractionRegistry.All;
        IInteractable best = null;
        float bestSqr = _range * _range;
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        bool checkFacing = _maxFacingAngle < 360f;
        float cosHalf = Mathf.Cos(_maxFacingAngle * 0.5f * Mathf.Deg2Rad);

        for (int i = 0; i < all.Count; i++)
        {
            IInteractable candidate = all[i];
            if (candidate == null) continue;

            Vector3 to = candidate.Position - origin;
            float sqr = to.sqrMagnitude;
            if (sqr > bestSqr) continue;

            if (checkFacing && sqr > 0.0001f)
            {
                Vector3 dir = to / Mathf.Sqrt(sqr);
                if (Vector3.Dot(forward, dir) < cosHalf) continue;
            }

            bestSqr = sqr;
            best = candidate;
        }
        return best;
    }

    private void SetCurrent(IInteractable interactable)
    {
        _current = interactable;
        if (_current != null)
        {
            InteractionEvents.RaisePromptChanged(_current.Prompt, true);
        }
        else
        {
            InteractionEvents.RaisePromptChanged(string.Empty, false);
        }
    }
}
