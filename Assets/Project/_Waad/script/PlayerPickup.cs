using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private GameObject pickupText;

    private CarryableItem nearbyItem;
    private CarryableItem carriedItem;

    private void Start()
    {
        pickupText.SetActive(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (nearbyItem == null)
            return;

        if (carriedItem != null)
            return;

        nearbyItem.PickUp(itemHolder, transform);
        carriedItem = nearbyItem;

        pickupText.SetActive(false);
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (carriedItem == null)
            return;

        carriedItem.Drop();
        carriedItem = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        CarryableItem item = other.GetComponentInParent<CarryableItem>();

        if (item != null)
        {
            nearbyItem = item;

            if (carriedItem == null)
            {
                pickupText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarryableItem item = other.GetComponentInParent<CarryableItem>();

        if (item == nearbyItem)
        {
            nearbyItem = null;
            pickupText.SetActive(false);
        }
    }
}