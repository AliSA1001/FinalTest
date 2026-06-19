using UnityEngine;

public class HealPickup : MonoBehaviour
{
    public bool destroyAfterTime = false;

    private void Start()
    {
        if (destroyAfterTime)
        {
            Destroy(gameObject, 10f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HeartsHealthVisual heartsHealthVisual =
                FindObjectOfType<HeartsHealthVisual>();

            heartsHealthVisual.Heal1();

            Destroy(gameObject);
        }
    }
}