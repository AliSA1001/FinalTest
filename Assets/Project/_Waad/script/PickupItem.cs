using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    private bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp)
            return;

        if (!other.CompareTag("Player"))
            return;

        pickedUp = true;

        PlayerInventory inventory =
            other.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        inventory.PickupItem(itemPrefab);

        Destroy(gameObject);
    }
}