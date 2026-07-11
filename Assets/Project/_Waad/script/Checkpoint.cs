using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Light checkpointLight;
    public GameObject savePopup;
    public GameObject checkpointEffectPrefab;

    private bool activated = false;

    private void Start()
    {
        if (savePopup != null)
        {
            savePopup.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;

            // حفظ مكان الريسبون أمام الشيك بوينت
            CheckpointManager.checkpointPosition =
                transform.position + transform.forward * 2f;

            CheckpointManager.hasCheckpoint = true;

            if (checkpointLight != null)
            {
                checkpointLight.color = Color.green;
            }

            if (savePopup != null)
            {
                savePopup.SetActive(true);
                Invoke(nameof(HidePopup), 2f);
            }

            Debug.Log("Checkpoint Saved!");
        }
        checkpointEffectPrefab.SetActive(true);
    }

    private void HidePopup()
    {
        if (savePopup != null)
        {
            savePopup.SetActive(false);
        }
    }
}