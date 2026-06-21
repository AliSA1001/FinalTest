using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && CheckpointManager.hasCheckpoint)
        {
            CharacterController cc =
                player.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
            }

            player.transform.position =
                CheckpointManager.checkpointPosition;

            if (cc != null)
            {
                cc.enabled = true;
            }
        }

        HeartsHealthVisual health =
            FindObjectOfType<HeartsHealthVisual>();

        if (health != null)
        {
            health.FullHeal();
        }
    }
}