using System;
using UnityEngine;

/// <summary>
/// Static event hub for the Interaction system. The <see cref="Interactor"/> raises the prompt signal and a UI
/// view (<see cref="InteractionPromptUI"/>) consumes it. Subscribe in OnEnable, unsubscribe in OnDisable.
/// </summary>
public static class InteractionEvents
{
    /// <summary>The interact prompt changed. (prompt text, visible)</summary>
    public static event Action<string, bool> PromptChanged;

    public static void RaisePromptChanged(string prompt, bool visible) => PromptChanged?.Invoke(prompt, visible);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAll()
    {
        PromptChanged = null;
    }
}
