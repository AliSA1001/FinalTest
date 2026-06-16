using UnityEngine;

public class HealPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HeartsHealthVisual heartsHealthVisual =
                FindObjectOfType<HeartsHealthVisual>();

            heartsHealthVisual.Heal1();

            Destroy(gameObject);
        }
    }
}