using UnityEngine;

public class HealPickup : MonoBehaviour
{
    public bool destroyAfterTime = false;

    private void Start()
    {
        if (destroyAfterTime)
        {
            Destroy(transform.root.gameObject, 10f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Working: " + other.name);

        if (other.CompareTag("Player"))
        {
            HeartsHealthVisual heartsHealthVisual =
                FindObjectOfType<HeartsHealthVisual>();

            if (heartsHealthVisual != null)
            {
                heartsHealthVisual.Heal1();
            }

            Destroy(transform.root.gameObject);
        }
    }
}