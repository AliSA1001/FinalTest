using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OutroSequence : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI phraseText;
    [SerializeField] private RectTransform topEyelid;
    [SerializeField] private RectTransform bottomEyelid;

    [Header("Settings")]
    [SerializeField] private string[] phrases = new string[8];
    [SerializeField] private float phraseDuration = 6f;
    [SerializeField] private float textFadeDuration = 1f;

    [Header("Scene Transition")]
    [Tooltip("Type the exact name of the scene you want to load next.")]
    [SerializeField] private string nextSceneName;

    [Header("Audio")]
    [SerializeField] private AudioSource backgroundAudioSource;

    private void Start()
    {
        if (phraseText == null || topEyelid == null || bottomEyelid == null)
        {
            Debug.LogError("OutroSequence: Please assign all UI elements in the inspector!");
            return;
        }

        StartCoroutine(PlayOutroSequence());
    }

    private IEnumerator PlayOutroSequence()
    {
        // 1. Immediately cut the screen to black
        Vector2 fullyClosed = new Vector2(1f, 1f);
        topEyelid.localScale = fullyClosed;
        bottomEyelid.localScale = fullyClosed;

        // Start background audio instantly in the darkness
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Play();
        }

        // Clear and hide text elements initially
        phraseText.text = "";
        phraseText.color = new Color(phraseText.color.r, phraseText.color.g, phraseText.color.b, 0f);

        // A small beat of empty darkness before words start (0.5 seconds)
        yield return new WaitForSeconds(0.5f);

        // 2. Play the Phrases over the black screen
        for (int i = 0; i < phrases.Length; i++)
        {
            if (string.IsNullOrEmpty(phrases[i])) continue;

            phraseText.text = phrases[i];
            
            // Fade Text In
            yield return StartCoroutine(FadeText(0f, 1f, textFadeDuration));
            
            // Wait for display time
            yield return new WaitForSeconds(phraseDuration - (textFadeDuration * 2f));
            
            // Fade Text Out
            yield return StartCoroutine(FadeText(1f, 0f, textFadeDuration));
            
            // Gap between phrases
            yield return new WaitForSeconds(0.5f);
        }

        // 3. Smoothly change to the next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
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
}