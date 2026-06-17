using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.checkpointPosition = transform.position;
            CheckpointManager.hasCheckpoint = true;

            Debug.Log("Checkpoint Saved!");
        }
    }
}