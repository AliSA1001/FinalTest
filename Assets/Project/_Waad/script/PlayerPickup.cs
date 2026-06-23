using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;

    private CarryableItem nearbyItem;
    private CarryableItem carriedItem;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (nearbyItem != null && carriedItem == null)
        {
            nearbyItem.PickUp(itemHolder, transform);
            carriedItem = nearbyItem;
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (carriedItem != null)
        {
            carriedItem.Drop();
            carriedItem = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarryableItem item = other.GetComponent<CarryableItem>();

        if (item != null)
        {
            nearbyItem = item;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarryableItem item = other.GetComponent<CarryableItem>();

        if (item == nearbyItem)
        {
            nearbyItem = null;
        }
    }
}