using UnityEngine;

public class FullHealPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HeartsHealthVisual heartsHealthVisual =
                FindObjectOfType<HeartsHealthVisual>();

            heartsHealthVisual.Heal4();

            Destroy(gameObject);
        }
    }
}