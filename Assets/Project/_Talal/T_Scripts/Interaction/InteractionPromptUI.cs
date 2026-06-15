using UnityEngine;
using TMPro;

/// <summary>
/// Thin prompt display for the Interaction system. Driven entirely by <see cref="InteractionEvents"/> — it
/// shows a short hint while the player is near an interactable. The real UI manager can replace it later.
/// Put this on an always-active object (e.g. the Canvas) and point <see cref="_root"/> at the child to toggle.
/// </summary>
public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _label;
    [Tooltip("Label format; {0} = the interactable's prompt. e.g. \"Press E - {0}\". Empty = just the prompt.")]
    [SerializeField] private string _format = "Press E - {0}";

    private void Awake()
    {
        if (_root != null) _root.SetActive(false);
    }

    private void OnEnable()
    {
        InteractionEvents.PromptChanged += OnPromptChanged;
    }

    private void OnDisable()
    {
        InteractionEvents.PromptChanged -= OnPromptChanged;
    }

    private void OnPromptChanged(string prompt, bool visible)
    {
        if (_root != null) _root.SetActive(visible);
        if (visible && _label != null)
        {
            _label.text = string.IsNullOrEmpty(_format) ? prompt : string.Format(_format, prompt);
        }
    }
}
