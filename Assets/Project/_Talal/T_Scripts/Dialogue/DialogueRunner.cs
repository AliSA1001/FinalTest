using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Walks a <see cref="DialogueData"/> node by node. Thin MonoBehaviour: owns no UI and references no other
/// system — it listens on <see cref="DialogueEvents"/> for start/choice requests and raises started/line/ended
/// events for the view and any listeners. Linear lines advance on the confirm input; choice lines wait for a
/// selection.
/// </summary>
public class DialogueRunner : MonoBehaviour
{
    [Tooltip("Confirm / advance action (e.g. UI/Submit from InputSystem_Actions).")]
    [SerializeField] private InputActionReference _advance;

    private DialogueData _data;
    private DialogueNode _current;
    private bool _running;

    private void OnEnable()
    {
        DialogueEvents.StartRequested += StartDialogue;
        DialogueEvents.ChoiceSelected += OnChoiceSelected;
        if (_advance != null && _advance.action != null)
        {
            _advance.action.performed += OnAdvance;
            _advance.action.Enable();
        }
    }

    private void OnDisable()
    {
        DialogueEvents.StartRequested -= StartDialogue;
        DialogueEvents.ChoiceSelected -= OnChoiceSelected;
        if (_advance != null && _advance.action != null)
        {
            _advance.action.performed -= OnAdvance;
            _advance.action.Disable();
        }
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null || _running) return;
        _data = data;
        _running = true;
        DialogueEvents.RaiseDialogueStarted(data.dialogueId);
        Show(data.StartNode);
    }

    private void Show(DialogueNode node)
    {
        _current = node;
        if (_current == null)
        {
            End();
            return;
        }
        DialogueEvents.RaiseDialogueLineShown(_current);
    }

    private void OnAdvance(InputAction.CallbackContext _)
    {
        if (!_running || _current == null) return;
        if (_current.HasChoices) return;            // choice nodes advance via selection, not confirm
        if (_current.isEnd)
        {
            End();
            return;
        }
        Show(_data.GetNode(_current.nextId));
    }

    private void OnChoiceSelected(int index)
    {
        if (!_running || _current == null || !_current.HasChoices) return;
        if (index < 0 || index >= _current.choices.Count) return;
        Show(_data.GetNode(_current.choices[index].nextId));
    }

    private void End()
    {
        string id = _data != null ? _data.dialogueId : null;
        _running = false;
        _current = null;
        DialogueEvents.RaiseDialogueEnded(id);
    }
}
