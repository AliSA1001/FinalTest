using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads a scene the moment the player walks into this trigger — no interaction / key press needed.
/// Put this on a GameObject with a Collider that has "Is Trigger" enabled.
/// </summary>
[RequireComponent(typeof(Collider))]
public class T_SceneLoadOnEnter : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Exact name of the scene to load. It must be added to File > Build Settings.")]
    [SerializeField] private string sceneToLoad;

    [Header("Tag Settings")]
    [Tooltip("Only a collider with this tag triggers the load.")]
    [SerializeField] private string playerTag = "Player";

    private bool _loading; // guards against firing twice

    private void OnTriggerEnter(Collider other)
    {
        if (_loading || !other.CompareTag(playerTag)) return;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("T_SceneLoadOnEnter: Scene name is empty!", this);
            return;
        }

        _loading = true;
        SceneManager.LoadScene(sceneToLoad);
    }
}
