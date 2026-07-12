using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Waits until every listed monster is defeated, then removes a barrier and/or switches other objects on.
///
/// Monsters "die" by being destroyed (EnemyHealth calls Destroy on death), so this simply watches the list and,
/// once every entry is gone (null), turns the barrier off and any "activate" objects on. No enemy-script changes.
///
/// Setup: put this on any object. Drag the arena's monsters into <see cref="monsters"/>. Optionally set a
/// <see cref="barrier"/> to hide and/or objects in <see cref="activateWhenCleared"/> to reveal. You can also
/// hook a sound or VFX to <see cref="onCleared"/> in the Inspector.
/// </summary>
public class T_ArenaBarrier : MonoBehaviour
{
    [Header("Fight")]
    [Tooltip("Every monster that must be defeated to open the barrier. Drag the object that disappears when it dies (the one with EnemyHealth).")]
    [SerializeField] private List<GameObject> monsters = new List<GameObject>();

    [Header("When cleared")]
    [Tooltip("Object to turn OFF once all monsters are defeated — e.g. the wall / collider blocker. Optional.")]
    [SerializeField] private GameObject barrier;

    [Tooltip("Objects to turn ON once all monsters are defeated — reward, portal, exit, next path... They should start DISABLED in the scene. Optional.")]
    [SerializeField] private List<GameObject> activateWhenCleared = new List<GameObject>();

    [Header("Checking")]
    [Tooltip("Seconds between checks. It is only null-checks, so keep it small.")]
    [SerializeField] private float checkInterval = 0.25f;

    [Header("On cleared (optional)")]
    [Tooltip("Fires once when the last monster dies — hook a sound, VFX, door open, etc.")]
    public UnityEvent onCleared;

    // The monsters that were actually assigned (ignores empty Inspector slots), captured at Start.
    private readonly List<GameObject> _live = new List<GameObject>();
    private float _timer;
    private bool _opened;

    private void Start()
    {
        foreach (GameObject m in monsters)
            if (m != null) _live.Add(m);

        if (_live.Count == 0)
            Debug.LogWarning("T_ArenaBarrier: no monsters assigned — the barrier will stay up.", this);
    }

    private void Update()
    {
        if (_opened || _live.Count == 0) return;

        _timer += Time.deltaTime;
        if (_timer < checkInterval) return;
        _timer = 0f;

        if (AllDefeated()) OpenBarrier();
    }

    private bool AllDefeated()
    {
        foreach (GameObject m in _live)
            if (m != null) return false; // a destroyed object reads as null; non-null == still alive
        return true;
    }

    private void OpenBarrier()
    {
        _opened = true;

        if (barrier != null) barrier.SetActive(false);

        foreach (GameObject go in activateWhenCleared)
            if (go != null) go.SetActive(true);

        onCleared?.Invoke();
    }
}
