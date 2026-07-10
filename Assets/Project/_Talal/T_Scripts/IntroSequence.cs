using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroSequence : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI phraseText;
    [SerializeField] private RectTransform topEyelid;
    [SerializeField] private RectTransform bottomEyelid;

    [Header("Settings")]
    [SerializeField] private string[] phrases = new string[8];
    [SerializeField] private float phraseDuration = 6f;
    [SerializeField] private float textFadeDuration = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource backgroundAudioSource;

    private void Start()
    {
        if (phraseText == null || topEyelid == null || bottomEyelid == null)
        {
            Debug.LogError("IntroSequence: Please assign all UI elements in the inspector!");
            return;
        }

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Play();
        }

        phraseText.color = new Color(phraseText.color.r, phraseText.color.g, phraseText.color.b, 0f);

        for (int i = 0; i < phrases.Length; i++)
        {
            if (string.IsNullOrEmpty(phrases[i])) continue;

            phraseText.text = phrases[i];
            yield return StartCoroutine(FadeText(0f, 1f, textFadeDuration));
            yield return new WaitForSeconds(phraseDuration - (textFadeDuration * 2f));
            yield return StartCoroutine(FadeText(1f, 0f, textFadeDuration));
            yield return new WaitForSeconds(0.5f);
        }

        // Trigger the updated, organic eye-waking sequence
        yield return StartCoroutine(RealisticWakeUpSequence());
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color textColor = phraseText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            phraseText.color = textColor;
            yield return null;
        }

        textColor.a = endAlpha;
        phraseText.color = textColor;
    }

    private IEnumerator RealisticWakeUpSequence()
    {
        // --- STEP 1: The Initial Peek (Eyes open 30% out of grogginess) ---
        yield return StartCoroutine(AnimateEyelids(1f, 0.7f, 1.2f)); 
        yield return new WaitForSeconds(0.4f); // Dwell slightly, blurry/confused

        // --- STEP 2: The Reflex Blink (Squeeze eyes tightly shut quickly) ---
        yield return StartCoroutine(AnimateEyelids(0.7f, 1.0f, 0.15f)); 
        yield return new WaitForSeconds(0.15f); // Kept shut for a split second

        // --- STEP 3: The Final Heavy Open (Slowly force them fully open) ---
        yield return StartCoroutine(AnimateEyelids(1.0f, 0.0f, 2.8f));

        // Disable UI objects to clean up memory/rendering
        topEyelid.gameObject.SetActive(false);
        bottomEyelid.gameObject.SetActive(false);
        phraseText.gameObject.SetActive(false);
    }

    // Helper coroutine that handles scaling the eyelids over time between two target percentages
    private IEnumerator AnimateEyelids(float startScaleY, float endScaleY, float duration)
    {
        float elapsed = 0f;
        Vector2 scaleBuffer = new Vector2(1f, startScaleY);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Using SmoothStep provides a natural physical dampening curve (acceleration/deceleration)
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentY = Mathf.Lerp(startScaleY, endScaleY, smoothT);
            
            scaleBuffer.y = currentY;
            topEyelid.localScale = scaleBuffer;
            bottomEyelid.localScale = scaleBuffer;

            yield return null;
        }

        scaleBuffer.y = endScaleY;
        topEyelid.localScale = scaleBuffer;
        bottomEyelid.localScale = scaleBuffer;
    }
}