using System;
using UnityEngine;

/// <summary>
/// Minimal event hub for Act-1 reactions the dialogue can trigger. The real objective UI and combat tutorial
/// subscribe to these; the dialogue system itself never references them.
/// </summary>
public static class Act1Events
{
    /// <summary>Set the current objective text.</summary>
    public static event Action<string> ObjectiveSet;
    /// <summary>Request that the parry tutorial prompt be shown.</summary>
    public static event Action ParryTutorialRequested;

    public static void SetObjective(string objective) => ObjectiveSet?.Invoke(objective);
    public static void RequestParryTutorial() => ParryTutorialRequested?.Invoke();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAll()
    {
        ObjectiveSet = null;
        ParryTutorialRequested = null;
    }
}

/// <summary>
/// EXAMPLE Act-1 flow listener — a placeholder for whoever owns the Act sequencer / objective system. It shows
/// the decoupled wiring the GDD wants: when the intro conversation ends, set the objective and ask for the
/// parry tutorial, entirely through events. Move this into the Act-1 system once that exists.
/// </summary>
public class Act1IntroResponder : MonoBehaviour
{
    [SerializeField] private string _introDialogueId = "HeraldIntro";
    [SerializeField] private string _objective = "Reach the high tower and free the princess";

    private void OnEnable() => DialogueEvents.DialogueEnded += OnDialogueEnded;
    private void OnDisable() => DialogueEvents.DialogueEnded -= OnDialogueEnded;

    private void OnDialogueEnded(string dialogueId)
    {
        if (dialogueId != _introDialogueId) return;
        Act1Events.SetObjective(_objective);
        Act1Events.RequestParryTutorial();
        Debug.Log($"[Act1] Intro done -> objective \"{_objective}\" set, parry tutorial requested.");
    }
}
