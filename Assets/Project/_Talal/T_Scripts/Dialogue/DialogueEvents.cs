using System;
using UnityEngine;

/// <summary>
/// Static event hub for the dialogue system — the only thing the runner, view, NPC trigger and listeners
/// share, so none of them reference each other. Subscribe in OnEnable, unsubscribe in OnDisable;
/// <see cref="ResetAll"/> clears everything on Play.
/// </summary>
public static class DialogueEvents
{
    // ----- requests (raised by NPC trigger / quick-start / the view's buttons) -----

    /// <summary>Asks the runner to start a conversation.</summary>
    public static event Action<DialogueData> StartRequested;
    /// <summary>The player picked choice index N on the current node.</summary>
    public static event Action<int> ChoiceSelected;

    // ----- broadcasts (raised by the runner; consumed by the view and any listeners) -----

    /// <summary>A conversation began. Argument is the DialogueData's dialogueId.</summary>
    public static event Action<string> DialogueStarted;
    /// <summary>A line is now showing (speaker, text, choices).</summary>
    public static event Action<DialogueNode> DialogueLineShown;
    /// <summary>A conversation finished. Argument is the dialogueId, e.g. "HeraldIntro".</summary>
    public static event Action<string> DialogueEnded;

    public static void RequestStart(DialogueData data) => StartRequested?.Invoke(data);
    public static void SelectChoice(int index) => ChoiceSelected?.Invoke(index);
    public static void RaiseDialogueStarted(string id) => DialogueStarted?.Invoke(id);
    public static void RaiseDialogueLineShown(DialogueNode node) => DialogueLineShown?.Invoke(node);
    public static void RaiseDialogueEnded(string id) => DialogueEnded?.Invoke(id);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAll()
    {
        StartRequested = null;
        ChoiceSelected = null;
        DialogueStarted = null;
        DialogueLineShown = null;
        DialogueEnded = null;
    }
}
