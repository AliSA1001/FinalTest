using UnityEngine;

/// <summary>
/// Shows a tutorial hint panel while the player stands inside this trigger volume, and hides it on exit.
/// No animation — a plain show/hide, matching the other prompts.
///
/// Setup: put this on an empty GameObject that has a Collider with "Is Trigger" enabled, then assign the
/// <see cref="hintPanel"/> (a UI object in the Canvas). Tag the player to match <see cref="playerTag"/>.
/// </summary>
[RequireComponent(typeof(Collider))]
public class T_TutorialTrigger : MonoBehaviour
{
    [Tooltip("The hint panel (in the Canvas) shown while the player is inside this trigger.")]
    [SerializeField] private GameObject hintPanel;

    [Tooltip("Tag of the player that triggers the hint.")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Show only the first time the player enters. Off = shows every time they are inside.")]
    [SerializeField] private bool showOnce = false;

    private bool _shown;
    private T_UIFade _fade;

    private void Start()
    {
        if (hintPanel == null) return;

        // If the panel has a T_UIFade, keep it active and let the fader hold it hidden (alpha 0);
        // otherwise fall back to a plain SetActive hide.
        _fade = hintPanel.GetComponent<T_UIFade>();
        if (_fade == null) hintPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hintPanel == null || !other.CompareTag(playerTag)) return;
        if (showOnce && _shown) return;

        _shown = true;
        ShowPanel();
    }

    private void OnTriggerExit(Collider other)
    {
        if (hintPanel == null || !other.CompareTag(playerTag)) return;
        HidePanel();
    }

    private void ShowPanel()
    {
        if (_fade != null) _fade.Show();
        else hintPanel.SetActive(true);
    }

    private void HidePanel()
    {
        if (_fade != null) _fade.Hide();
        else hintPanel.SetActive(false);
    }
}
