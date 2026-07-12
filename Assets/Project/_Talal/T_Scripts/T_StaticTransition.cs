using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Full-screen "old TV" scene transition, built entirely in code — no prefab or Canvas to wire.
/// TV static fades in over the game, holds, then black closes over it; the target scene loads behind the
/// black and the black fades away to reveal it. Runs on its own DontDestroyOnLoad object so the effect
/// survives the scene load and can fade the new scene in.
///
/// Call from anywhere:  <c>T_StaticTransition.Play("MySceneName");</c>
/// </summary>
public class T_StaticTransition : MonoBehaviour
{
    private string _scene;
    private float _staticIn, _staticHold, _fadeToBlack, _fadeIn, _opacity;
    private int _noiseRes;
    private AudioClip _sound;
    private bool _staticActive;

    /// <summary>Spawns the transition and loads <paramref name="sceneName"/> once the screen is black.</summary>
    public static T_StaticTransition Play(string sceneName, float staticIn = 0.25f, float staticHold = 0.6f,
        float fadeToBlack = 0.8f, float fadeIn = 0.6f, int noiseResolution = 320, float staticOpacity = 1f,
        AudioClip staticSound = null)
    {
        var go = new GameObject("T_StaticTransition", typeof(RectTransform));
        var t = go.AddComponent<T_StaticTransition>();
        t._scene = sceneName;
        t._staticIn = staticIn;
        t._staticHold = staticHold;
        t._fadeToBlack = fadeToBlack;
        t._fadeIn = fadeIn;
        t._noiseRes = Mathf.Max(16, noiseResolution);
        t._opacity = Mathf.Clamp01(staticOpacity);
        t._sound = staticSound;
        DontDestroyOnLoad(go);
        t.StartCoroutine(t.Run());
        return t;
    }

    private IEnumerator Run()
    {
        // ---- build the overlay on this (persistent) object ----
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue; // draw above everything
        var cg = gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = true; // swallow UI clicks mid-transition

        int w = _noiseRes;
        int h = Mathf.Max(9, Mathf.RoundToInt(_noiseRes * 9f / 16f));
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Repeat
        };

        var staticImg = NewFullScreenChild("Static").AddComponent<RawImage>();
        StretchFull(staticImg.rectTransform);
        staticImg.texture = tex;
        staticImg.color = new Color(1f, 1f, 1f, 0f);
        staticImg.raycastTarget = false;

        var blackImg = NewFullScreenChild("Black").AddComponent<Image>();
        StretchFull(blackImg.rectTransform);
        blackImg.color = new Color(0f, 0f, 0f, 0f);
        blackImg.raycastTarget = false;

        AudioSource audio = null;
        if (_sound != null)
        {
            audio = gameObject.AddComponent<AudioSource>();
            audio.clip = _sound;
            audio.loop = true;
            audio.Play();
        }

        // keep regenerating the noise while the static is on screen
        _staticActive = true;
        StartCoroutine(AnimateStatic(tex));

        // 1) static fades in over the game
        yield return Fade(a => SetAlpha(staticImg, a * _opacity), _staticIn);
        // 2) hold on the static
        yield return Wait(_staticHold);
        // 3) black closes over the static
        yield return Fade(a => SetAlpha(blackImg, a), _fadeToBlack);

        // static is fully covered now — stop drawing/regenerating it
        _staticActive = false;
        staticImg.enabled = false;
        if (audio != null) audio.Stop();

        // 4) load the new scene behind the black
        var op = SceneManager.LoadSceneAsync(_scene);
        while (op != null && !op.isDone) yield return null;

        // 5) fade the black away to reveal the new scene
        yield return Fade(a => SetAlpha(blackImg, 1f - a), _fadeIn);

        Destroy(tex);
        Destroy(gameObject);
    }

    private IEnumerator AnimateStatic(Texture2D tex)
    {
        var px = new Color32[tex.width * tex.height];
        while (_staticActive)
        {
            for (int i = 0; i < px.Length; i++)
            {
                byte v = (byte)UnityEngine.Random.Range(0, 256);
                px[i] = new Color32(v, v, v, 255);
            }
            tex.SetPixels32(px);
            tex.Apply(false);
            yield return null;
        }
    }

    private GameObject NewFullScreenChild(string childName)
    {
        var go = new GameObject(childName, typeof(RectTransform));
        go.transform.SetParent(transform, false);
        return go;
    }

    private static void SetAlpha(Graphic g, float a)
    {
        var c = g.color;
        c.a = a;
        g.color = c;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Runs apply(0..1) over duration using unscaled time (works even if the game is paused).
    private IEnumerator Fade(Action<float> apply, float duration)
    {
        if (duration <= 0f) { apply(1f); yield break; }
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            apply(Mathf.Clamp01(t / duration));
            yield return null;
        }
        apply(1f);
    }

    private IEnumerator Wait(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
