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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HeartsHealthVisual heartsHealthVisual =
                FindObjectOfType<HeartsHealthVisual>();

            heartsHealthVisual.Heal1();

            Destroy(gameObject);
        }
    }
}