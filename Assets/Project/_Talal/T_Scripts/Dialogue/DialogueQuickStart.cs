#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// EDITOR-ONLY quick tester: press the key to start a dialogue without an NPC, collider or Interaction system.
/// Compiled out of player builds; delete it whenever. Add it to any GameObject and assign a DialogueData.
/// </summary>
public class DialogueQuickStart : MonoBehaviour
{
    [SerializeField] private DialogueData _dialogue;
    [SerializeField] private Key _startKey = Key.T;

    private void Update()
    {
        if (_dialogue == null) return;
        Keyboard kb = Keyboard.current;
        if (kb != null && kb[_startKey].wasPressedThisFrame)
        {
            DialogueEvents.RequestStart(_dialogue);
        }
    }
}
#endif
