using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Required for the New Input System

public class SceneTriggerInteract : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("The whole prompt panel (background + keycap + label) to show/hide.")]
    [SerializeField] private GameObject interactPrompt;

    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    [Header("Tag Settings")]
    [SerializeField] private string playerTag = "Player";

    [Header("New Input System Settings")]
    [Tooltip("Pick the Input Action you want to use for interaction (e.g., standard 'Interact' or 'Use' binding).")]
    [SerializeField] private InputAction interactAction;

    private bool isPlayerInside = false;

    private void OnEnable()
    {
        // Enable the input action when the script turns on
        interactAction.Enable();
    }

    private void OnDisable()
    {
        // Disable the input action when the script turns off to prevent memory leaks
        interactAction.Disable();
    }

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // WasPressedThisFrame() handles single press actions natively in the new system
        if (isPlayerInside && interactAction.WasPressedThisFrame())
        {
            TriggerSceneLoad();
        }
    }

    private void TriggerSceneLoad()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("SceneTriggerInteract: Scene name is empty!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}