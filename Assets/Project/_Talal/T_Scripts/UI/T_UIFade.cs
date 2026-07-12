using System.Collections;
using UnityEngine;

/// <summary>
/// Fades a UI panel in/out via its CanvasGroup alpha instead of switching the GameObject off — so it can
/// animate on the way out too. The object stays active; visibility is the alpha.
///
/// Setup: add this to a panel (a CanvasGroup is auto-added) and keep the panel ACTIVE in the scene. Call
/// Show() / Hide(); both are idempotent. Fades use unscaled time so they still play while the game is paused.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class T_UIFade : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float fadeOutDuration = 0.2f;

    private CanvasGroup _group;
    private Coroutine _routine;
    private bool _visible;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false; // a hint must never eat clicks
        _visible = false;
    }

    /// <summary>Fade in. Idempotent.</summary>
    public void Show()
    {
        if (_visible) return;
        _visible = true;
        StartFade(1f, fadeInDuration);
    }

    /// <summary>Fade out. Idempotent.</summary>
    public void Hide()
    {
        if (!_visible) return;
        _visible = false;
        StartFade(0f, fadeOutDuration);
    }

    private void StartFade(float target, float duration)
    {
        if (_routine != null) StopCoroutine(_routine);

        if (isActiveAndEnabled) _routine = StartCoroutine(FadeTo(target, duration));
        else _group.alpha = target; // disabled: snap to target
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = _group.alpha;
        float dur = Mathf.Max(0.0001f, duration);
        float t = 0f;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            _group.alpha = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / dur));
            yield return null;
        }

        _group.alpha = target;
        _routine = null;
    }
}
